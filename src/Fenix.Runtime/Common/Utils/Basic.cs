using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using MessagePack;

namespace Fenix.Common.Utils
{
    public partial class Basic
    {
        public static UInt64 GenID64()
        {
            //ulong newID;
            //while ((newID = NewUid(Guid.NewGuid())) > ulong.MaxValue / 2);
            return NewUid(Guid.NewGuid());
        }

        static ulong NewUid(Guid guid)
        {
            var src = guid.ToByteArray();
            var buffer = new byte[8];
            Array.Copy(src, 0, buffer, 0, 8);
            ulong long1 = BitConverter.ToUInt64(buffer, 0);
            Array.Copy(src, 8, buffer, 0, 8);
            ulong long2 = BitConverter.ToUInt64(buffer, 0);

            ulong m = 0xc6a4a7935bd1e995;
            ulong h = m >> 4;
            const int r = 47;

            ulong k = long1;

            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            k = long2;

            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            h ^= h >> r;
            h *= m;
            h ^= h >> r;

            return h;
        }

        public static byte[] GenMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return hashBytes;
            }
        }

        public static uint GenID32FromName(string name)
        {
            var src = GenMd5(name);

            var buffer = new byte[8];
            Array.Copy(src, 0, buffer, 0, 8);
            ulong long1 = BitConverter.ToUInt64(buffer, 0);
            Array.Copy(src, 8, buffer, 0, 8);
            ulong long2 = BitConverter.ToUInt64(buffer, 0);

            ulong m = 0xc6a4a7935bd1e995;
            ulong h = m >> 4;
            const int r = 47;

            ulong k = long1;

            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            k = long2;

            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            h ^= h >> r;
            h *= m;
            h ^= h >> r;

            return (uint)(((h >> r) & 0xffffffff) ^ (h & 0xffffffff));
        }

        public static ulong GenID64FromName(string name)
        {
            if(name.StartsWith("U") && name.Length == 20)
            {
                //UID
                return ulong.Parse(name.Substring(1));
            }

            var src = GenMd5(name);

            var buffer = new byte[8];
            Array.Copy(src, 0, buffer, 0, 8);
            ulong long1 = BitConverter.ToUInt64(buffer, 0);
            Array.Copy(src, 8, buffer, 0, 8);
            ulong long2 = BitConverter.ToUInt64(buffer, 0);

            ulong m = 0xc6a4a7935bd1e995;
            ulong h = m >> 4;
            const int r = 47;

            ulong k = long1;

            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            k = long2;

            k *= m;
            k ^= k >> r;
            k *= m;

            h ^= k;
            h *= m;

            h ^= h >> r;
            h *= m;
            h ^= h >> r;

            return h;
        }

        public static string GetLocalIPv4(NetworkInterfaceType _type)
        {  
            // Checks your IP adress from the local network connected to a gateway. This to avoid issues with double network cards
            string output = "";  // default output
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) // Iterate over each network interface
            {  
                // Find the network interface which has been provided in the arguments, break the loop if found
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {   
                    // Fetch the properties of this adapter
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();
                    // Check if the gateway adress exist, if not its most likley a virtual network or smth
                    if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                    {   
                        // Iterate over each available unicast adresses
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {   
                            // If the IP is a local IPv4 adress
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {   
                                // we got a match!
                                output = ip.Address.ToString();
                                break;  
                                // break the loop!!
                            }
                        }
                    }
                }
                // Check if we got a result if so break this method
                if (output != "") { break; }
            }
            // Return results
            return output;
        }
        
        public static int GetAvailablePort(IPAddress ip) 
        {
            var l = new TcpListener(ip, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            Log.Info($"Available port found: {port}");
            return port;
        } 

        public static IPEndPoint ToAddress(string addr)
        {
            if (addr == null || addr == "")
                return null;
            var parts = addr.Split(':');
            return new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
        }

        public static string ToIP(string addr)
        {

            if (addr == null || addr == "")
                return null;
            var parts = addr.Split(':');
            return parts[0].Trim();
        }

        public static int ToPort(string addr)
        {
            if (addr == null || addr == "")
                return 0;
            var parts = addr.Split(':');
            return int.Parse(parts[1].Trim());
        } 
    }
}
