using Fenix;
using Fenix.Common.Utils;
using MessagePack;
using Server;
using System;
using System.Net;
using System.Reflection;
using System.Threading;

namespace Client.App
{
    class Program
    {
        static void Main(string[] args)
        {
            

            /*
            var ep = new System.Net.IPEndPoint(IPAddress.Parse(Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet)), 11111);

            KcpHostServer ks = KcpHostServer.Create(ep);
            ks.OnReceive += (ch, buffer) =>
            {
                Console.WriteLine(string.Format("KCP {0} {1}", ch.user().Channel.Id.AsShortText(), buffer.ToArray()));
                ch.writeMessage(DotNetty.Buffers.Unpooled.WrappedBuffer(BitConverter.GetBytes(1)));
            };

            KcpHostClient kc = KcpHostClient.Create(ep);
            kc.OnReceive += (ch, buffer) =>
            {
                Console.WriteLine(string.Format("KCP {0} {1}", ch.user().Channel.Id.AsShortText(), buffer.ToArray()));
                ch.writeMessage(DotNetty.Buffers.Unpooled.WrappedBuffer(BitConverter.GetBytes(1)));
            };
            kc.Send(BitConverter.GetBytes(1));

            TcpHostServer s = TcpHostServer.Create(ep);
            s.Receive += (ch, buffer) =>
            {
                Console.WriteLine(string.Format("TCP {0} {1}", ch.Id.AsShortText(), buffer.ToArray()));
                ch.WriteAndFlushAsync(DotNetty.Buffers.Unpooled.WrappedBuffer(BitConverter.GetBytes(1)));
            };
            
            TcpHostClient c = TcpHostClient.Create(new System.Net.IPEndPoint(IPAddress.Parse(Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet)), 11111));
            c.Receive += (ch, buffer) =>
            {
                Console.WriteLine(string.Format("TCP {0} {1}", ch.Id.AsShortText(), buffer.ToArray()));
                ch.WriteAndFlushAsync(DotNetty.Buffers.Unpooled.WrappedBuffer(BitConverter.GetBytes(1)));
            };
            c.Send(BitConverter.GetBytes(1));

            while(true)
            {
                Thread.Sleep(1);
            }

            return;
            */
       
            Global.Init(new Assembly[] { typeof(Program).Assembly });

            var localAddr = Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);

            var c = Host.CreateClient(localAddr, 17777);
            if (c == null)
            {
                Console.WriteLine("无法连接服务器");
                return;
            } 

            c.GetService<LoginServiceRef>("Login.App", localAddr, 17777).rpc_login("username", "password", (code) =>
            {
                Console.WriteLine("hello");
            });

            HostHelper.Run(c);
            
        }
    }
}
