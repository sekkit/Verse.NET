using System;
using DotNetty.Buffers;

namespace DotNetty.KCP
{
    public interface KcpListener
    {
        void handleConnect(Ukcp ukcp);

        /**
         * kcp message
         *
         * @param byteBuf the data
         * @param ukcp
         */
        void handleReceive(IByteBuffer byteBuf, Ukcp ukcp);

        /**
         *
         * kcp异常，之后此kcp就会被关闭
         *
         * @param ex 异常
         * @param ukcp 发生异常的kcp，null表示非kcp错误
         */
        void handleException(Ukcp ukcp, Exception ex);

        /**
         * 关闭
         *
         * @param ukcp
         */
        void handleClose(Ukcp ukcp);
    }
}