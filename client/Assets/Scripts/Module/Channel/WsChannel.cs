using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Shared.Message;
using MemoryPack; 
using Module.Shared;
using UnityEngine.Assertions;
using UnityWebSocket;
using WebSocketSharp;
using CloseEventArgs = UnityWebSocket.CloseEventArgs;
using ErrorEventArgs = UnityWebSocket.ErrorEventArgs;
using MessageEventArgs = UnityWebSocket.MessageEventArgs;
using WebSocket = UnityWebSocket.WebSocket;
using WebSocketState = UnityWebSocket.WebSocketState;

namespace Module.Channel
{
    public class WsChannel: Singleton<WsChannel>, IChannel, ILifecycle
    {
        // public static WsChannel Instance = new();
        //
        //track request sent from client to server
        private ConcurrentDictionary<ulong, DateTime> _req2DtDic = new();
        private ConcurrentDictionary<ulong, Action<Msg>> _rpcCallbackDic = new();
        private ConcurrentDictionary<ulong, TaskCompletionSource<Msg>> _rpcCbAsyncDic = new();
         
        private IWebSocket _socket;

        public string ServerUrl { get; set; } = "ws://127.0.0.1:4649/lacg";

        public void Connect()
        {
            Close();
            
            _socket = new WebSocket(ServerUrl);
            _socket.OnOpen += Socket_OnOpen;
            _socket.OnMessage += Socket_OnMessage;
            _socket.OnClose += Socket_OnClose;
            _socket.OnError += Socket_OnError;
            Log.Info(string.Format("Connecting..."));
            _socket.ConnectAsync();
        }

        public void Close()
        {
            if (_socket != null && _socket.ReadyState != WebSocketState.Closed)
            { 
                _socket.CloseAsync();
                _socket = null;
            }
        }
        
        private async void Socket_OnOpen(object sender, OpenEventArgs e)
        {
            Log.Info($"Connected: {ServerUrl}");
            var req = new LoginReq()
            {
                Username = "",
                Password = ""
            };  
            
            // Remote<LoginRsp>(ProtoCode.LOGIN, req, (rsp) =>
            // {
            //     Log.Info(rsp.Uid);
            // });

            var result = await InvokeWithCb<LoginRsp>(ProtoCode.LOGIN, req);  
            Log.Info(result.Uid);
        }
         
        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                Log.Info($"Receive Bytes ({e.RawData.Length})");
                if (e.RawData.Length < 4)
                {
                    return;
                }
            
                //var byteParts = ByteHelper.SplitBytes(e.RawData, 4);
                using (var ms = new MemoryStream(e.RawData))//byteParts.FirstOrDefault()))
                {
                    using (var reader = new EndianBinaryReader(EndianBitConverter.Little, ms))
                    {
                        uint protoCodeNum = reader.ReadUInt32();
                        ProtoCode code = Enum.Parse<ProtoCode>(protoCodeNum.ToString());
                        var data = Ext.SubArray(e.RawData, 4, e.RawData.Length-4);

                        Type msgType = null;
                        msgType = ProtocolProvider.Instance.GetNtfType(code);
                        if (msgType != null) //is ntf
                        { 
                            object param = MemoryPackSerializer.Deserialize(msgType, data) as Msg;
                            ulong rpcId = (param as Msg).RpcId;
                            Assert.IsTrue(rpcId == 0);
                            
                            MainThreadSynchronizationContext.Instance.Post(() =>
                            {
                                ClientStub.Instance.Call(code, param as Msg);
                            });
                        }
                        else //is req|rsp
                        {
                            msgType = ProtocolProvider.Instance.GetRspType(code);
                            if (msgType != null)
                            {
                                object param = MemoryPackSerializer.Deserialize(msgType, data) as Msg; 
                                ulong rpcId = (param as Msg).RpcId;
                                _req2DtDic.TryRemove(rpcId, out var _);
                                if (_rpcCallbackDic.TryRemove(rpcId, out var del))
                                { 
                                    MainThreadSynchronizationContext.Instance.Post(() =>
                                    {
                                        del?.DynamicInvoke(param);
                                    }); 
                                } 
                                else if (_rpcCbAsyncDic.TryRemove(rpcId, out var tcs))
                                {
                                    MainThreadSynchronizationContext.Instance.Post(() =>
                                    {
                                        tcs?.SetResult(param as Msg);
                                    });
                                }
                                else
                                {
                                    Log.Error(string.Format("Invalid code {0}", code));
                                }
                            }
                        }  
                    }
                }
            }
            else if (e.IsText)
            {
                Log.Info($"Receive: {e.Data}");
            }
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            Log.Info($"Closed: StatusCode: {e.StatusCode}, Reason: {e.Reason}");
        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            Log.Error($"Error: {e.Message}");
        }
        
        //client side, can only send req to server 

        public async Task<TRsp> InvokeWithCb<TRsp>(ProtoCode code, Msg msg) where TRsp: Msg
        {
            var type = msg.GetType();
            bool isRequest = type.Name.EndsWith("Req");  
            var tcs = new TaskCompletionSource<Msg>();
            if (isRequest)
            {
                using (var ms = new MemoryStream())
                {
                    using (var bw = new EndianBinaryWriter(EndianBitConverter.Little, ms))
                    {
                        //var code = type.GetCustomAttribute<ProtocolAttribute>().Code; 
                        msg.RpcId = MathHelper.GenLongID();
                        bw.Write((uint)code);
                        bw.Write(msg.Pack());
                        _socket?.SendAsync(ms.ToArray());
                        _req2DtDic[msg.RpcId] = DateTime.UtcNow;
                        _rpcCbAsyncDic[msg.RpcId] = tcs;
                    }
                }
            }
            else
            {
                MainThreadSynchronizationContext.Instance.Post(() => { tcs.SetCanceled(); });
            }

            return (await tcs.Task) as TRsp;
        }

        public void InvokeWithCb<TRsp>(ProtoCode code, Msg msg, Action<TRsp> callback) where TRsp : Msg
        {
            var type = msg.GetType();
            bool isRequest = type.Name.EndsWith("Req");  
            if (isRequest)
            {
                using (var ms = new MemoryStream())
                {
                    using (var bw = new EndianBinaryWriter(EndianBitConverter.Little, ms))
                    {
                        //var code = type.GetCustomAttribute<ProtocolAttribute>().Code; 
                        msg.RpcId = MathHelper.GenLongID();
                        bw.Write((uint)code);
                        bw.Write(msg.Pack());
                        _socket?.SendAsync(ms.ToArray());
                        _req2DtDic[msg.RpcId] = DateTime.UtcNow;
                        _rpcCallbackDic[msg.RpcId] = (param)=>callback.Invoke(param as TRsp);
                    }
                }
            }
        }
        
        public void Invoke(ProtoCode code, Msg msg)
        {
            var type = msg.GetType();
            bool isRequest = type.Name.EndsWith("Req");  
            if (isRequest)
            {
                using (var ms = new MemoryStream())
                {
                    using (var bw = new EndianBinaryWriter(EndianBitConverter.Little, ms))
                    {
                        //var code = type.GetCustomAttribute<ProtocolAttribute>().Code; 
                        bw.Write((uint)code);
                        bw.Write(msg.Pack());
                        _socket?.SendAsync(ms.ToArray());
                        //_req2DtDic[msg.RpcId] = DateTime.UtcNow;
                        //_rpcCallbackDic[msg.RpcId] = (param)=>callback.Invoke();
                    }
                }
            }
        }

        public void Start()
        {
            
        }

        public void Update()
        {
            
        }

        public void LateUpdate()
        {
            foreach (var kv in _req2DtDic)
            {
                if ((DateTime.UtcNow - kv.Value).TotalSeconds > 30)
                {
                    _rpcCallbackDic.TryRemove(kv.Key, out var del);
                    del?.Invoke(null);
                    _rpcCbAsyncDic.TryRemove(kv.Key, out var tcs);
                    tcs?.SetException(new TimeoutException());
                    
                    _req2DtDic.TryRemove(kv.Key, out var _);
                }
            }
        }

        public void FrameFinishedUpdate()
        {
            
        }

        public void Destroy()
        {
            
        }
    }
}