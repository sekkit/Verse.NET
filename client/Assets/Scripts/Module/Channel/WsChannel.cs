using System.IO;
using DataModel.Shared.Message;
using Module.Shared;
using UnityEngine;
using UnityWebSocket;
using ErrorEventArgs = UnityWebSocket.ErrorEventArgs;

namespace Module.Channel
{
    public class WsChannel: MonoBehaviour, IChannel
    {
        public static WsChannel Instance = new();
        
        private IWebSocket _socket;

        public string ServerUrl { get; set; } = "ws://127.0.0.1/lacg";

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
            //{protoCode}{data}
            using (var ms = new MemoryStream())
            {
                using (var bw = new EndianBinaryWriter(EndianBitConverter.Little, ms))
                {
                    var req = new LoginReq()
                    {
                        Username = "",
                        Password = ""
                    };  
                    bw.Write((uint)ProtoCode.LOGIN);
                    bw.Write((req as IMessage).Pack());
                    _socket?.SendAsync(ms.ToArray());
                } 
            }
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                Log.Info($"Receive Bytes ({e.RawData.Length}): {e.Data}");
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

        private void OnApplicationQuit()
        {
            if (_socket != null && _socket.ReadyState != WebSocketState.Closed)
            {
                _socket.CloseAsync();
            }
        }
        
        public void SendMsg(IMessage msg)
        {
            _socket?.SendAsync(msg.Pack());
        }
        
    }
}