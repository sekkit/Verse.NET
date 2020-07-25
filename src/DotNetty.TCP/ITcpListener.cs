using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetty.TCP
{
    public interface ITcpListener
    {
        void handleConnect(IChannel channel);

        void handleDisconnect(IChannel channel);

        void handleReceive(IChannel channel, IByteBuffer buffer);

        void handleClose(IChannel channel);

        void handleException(IChannel channel, Exception ex);
    }
}
