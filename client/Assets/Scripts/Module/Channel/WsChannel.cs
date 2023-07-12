using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using DataModel.Shared.Message;
using MemoryPack;
using Module.Shared;
using UnityEngine;
using UnityWebSocket;
using ErrorEventArgs = UnityWebSocket.ErrorEventArgs;

namespace Module.Channel
{
    public class WsChannel: Singleton<WsChannel>, IChannel
    {
        // public static WsChannel Instance = new();
        //
        //track request sent from client to server
        private ConcurrentDictionary<ulong, DateTime> _req2DtDic = new();
        private ConcurrentDictionary<ulong, Delegate> _rpcCallbackDic = new();

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
        
        private void Socket_OnOpen(object sender, OpenEventArgs e)
        {
            Log.Info($"Connected: {ServerUrl}");
            var req = new LoginReq()
            { 
                RpcId = MathHelper.GenLongID(),
                Username = "",
                Password = ""
            };  
            
            SendMsg(req);  
        }
         
        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                Log.Info($"Receive Bytes ({e.RawData.Length}): {e.RawData}");
                if (e.RawData.Length <= 4)
                {
                    return;
                }
            
                var byteParts = ByteHelper.SplitBytes(e.RawData, 4);
                using (var ms = new MemoryStream(byteParts.FirstOrDefault()))
                {
                    using (var reader = new EndianBinaryReader(EndianBitConverter.Little, ms))
                    {
                        uint protoCodeNum = reader.ReadUInt32();
                        ProtoCode code = Enum.Parse<ProtoCode>(protoCodeNum.ToString());

                        //entity.Get<RpcModule>().Call(protoCode, byteParts.LastOrDefault(), this);
                        var data = byteParts.LastOrDefault();
                        // if (_rpcCallbackDic.TryGetValue(code, out var del))
                        // {
                        //var reqType = TypeProvider.Instance.GetReqType(code);
                        var rspType = TypeProvider.Instance.GetRspType(code);
                        object param = MemoryPackSerializer.Deserialize(rspType, data) as Msg;
                        if (_rpcCallbackDic.TryGetValue((param as Msg).RpcId, out var del))
                        {
                            object result = del.DynamicInvoke(param);

                            // if (del.Method.ReturnType.IsSubclassOf(typeof(Task)))
                            // {
                            //     dynamic task = del.DynamicInvoke(param);
                            //     try
                            //     {
                            //         result = await task;
                            //     }
                            //     catch (Exception ex)
                            //     {
                            //         Shared.Log.Error(ex);
                            //         // Shared.Log.Error(Environment.StackTrace); 
                            //     }
                            // }
                            // else
                            // {
                            //     result = del.DynamicInvoke(param);
                            // }

                            SendMsg(result as Msg);
                        } 
                        else
                        {
                            Log.Error(string.Format("Invalid code {0}", code));
                            SendMsg(VoidMsg.Instance);
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
        public void SendMsg(Msg msg)
        {
            var type = msg.GetType();
            bool isRequest = type.Name.EndsWith("Req");  
            if (isRequest)
            {
                using (var ms = new MemoryStream())
                {
                    using (var bw = new EndianBinaryWriter(EndianBitConverter.Little, ms))
                    {
                        var code = type.GetCustomAttribute<ProtocolAttribute>().Code; 
                        bw.Write((uint)code);
                        bw.Write(msg.Pack());
                        _socket?.SendAsync(ms.ToArray());
                        _req2DtDic[msg.RpcId] = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}