using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.KCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Fenix.Common.Utils
{
    public static class Extention
    {
        /// <summary>
        /// 转为网络终结点IPEndPoint
        /// </summary>=
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static IPEndPoint ToIPEndPoint(this string str)
        {
            IPEndPoint iPEndPoint = null;
            try
            {
                string[] strArray = str.Split(':').ToArray();
                string addr = strArray[0];
                int port = Convert.ToInt32(strArray[1]);
                iPEndPoint = new IPEndPoint(IPAddress.Parse(addr), port);
            }
            catch
            {
                iPEndPoint = null;
            }

            return iPEndPoint;
        }

        public static string ToIPv4String(this IPEndPoint ep)
        {
            if (ep == null)
                return "";
            var ip = ep.Address.MapToIPv4().ToString().Trim();
#if !CLIENT
            if (ip == "0.0.0.0")
                ip = Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);
#endif
            return string.Format("{0}:{1}", ip, ep.Port);
        }

        public static string ToIPv4String(this EndPoint ep)
        {
            var newEp = (IPEndPoint)ep;
            var ip = newEp.Address.MapToIPv4().ToString().Trim();
#if !CLIENT
            if (ip == "0.0.0.0")
                ip = Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);
#endif
            return string.Format("{0}:{1}", ip, newEp.Port);
        }

        /// <summary>
        /// 获取IByteBuffer中的byte[]
        /// </summary>
        /// <param name="byteBuffer">IByteBuffer</param>
        /// <returns></returns>
        public static byte[] ToArray(this IByteBuffer byteBuffer)
        {
            int readableBytes = byteBuffer.ReadableBytes;
            if (readableBytes == 0)
            {
                return ArrayExtensions.ZeroBytes;
            }

            if (byteBuffer.HasArray)
            {
                return byteBuffer.Array.Slice(byteBuffer.ArrayOffset + byteBuffer.ReaderIndex, readableBytes);
            }

            var bytes = new byte[readableBytes];
            byteBuffer.GetBytes(byteBuffer.ReaderIndex, bytes);
            return bytes;
        }

        public static ulong GetUniqueId(this Ukcp ukcp)
        {
            return Basic.GenID64FromName(ukcp.user().Channel.Id.AsLongText() +
                ukcp.user().Channel.LocalAddress.ToIPv4String() +
                ukcp.user().RemoteAddress.ToIPv4String() + ukcp.getConv().ToString());
        }

    }
}
