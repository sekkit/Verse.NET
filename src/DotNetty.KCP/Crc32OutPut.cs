using System;
using DotNetty.KCP.Base;
using DotNetty.Buffers;

namespace DotNetty.KCP
{
    public class Crc32OutPut:KcpOutput
    {
        private readonly KcpOutput _output;
        private readonly int _headerOffset;

        public Crc32OutPut(KcpOutput output,int headerOffset) {
            _output = output;
            _headerOffset = headerOffset;
        }

        public void outPut(IByteBuffer data, Kcp kcp)
        {
            var checksum =Crc32.ComputeChecksum(data, _headerOffset + Ukcp.HEADER_CRC,
                data.ReadableBytes - _headerOffset - Ukcp.HEADER_CRC);
            data.SetUnsignedIntLE(_headerOffset, checksum);
            _output.outPut(data,kcp);
        }
    }
}