using DotNetty.Buffers; 
using DotNetty.TCP;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class TcpContainerClient: ITcpClientListener
    {
        public event Action<byte[], NetPeer, int> OnReceive;

        public event Action<Exception, NetPeer> OnException;

        public event Action<NetPeer> OnClose;

        public void OnConnect(IChannel channel)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnect(IChannel channel)
        {
            throw new NotImplementedException();
        }

        void ITcpClientListener.OnReceive(IChannel channel, IByteBuffer buffer)
        {
            throw new NotImplementedException();
        }

        void ITcpClientListener.OnClose(IChannel channel)
        {
            throw new NotImplementedException();
        }

        void ITcpClientListener.OnException(IChannel channel)
        {
            throw new NotImplementedException();
        }

        public async void Setup(string ip, int port)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = ip;
            channelConfig.Port = port;
            channelConfig.UseLibuv = true;
            

        }
 
    }
}
