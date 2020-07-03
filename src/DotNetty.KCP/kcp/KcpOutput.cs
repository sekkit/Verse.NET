using DotNetty.Buffers;

namespace DotNetty.KCP.Base
{
    public interface KcpOutput
    {
        void outPut(IByteBuffer data, Kcp kcp);
    }
}