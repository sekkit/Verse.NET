using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetty.TCP
{
    public interface ITcpListener
    {
        void OnConnect(IChannel channel);

        void OnDisconnect(IChannel channel);

        void OnReceive(IChannel channel, IByteBuffer buffer);

        void OnClose(IChannel channel);

        void OnException(IChannel channel);
    }
}
