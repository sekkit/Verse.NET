using Fenix;
using Server;
using System;
using System.Reflection;

namespace Client.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.Init(new Assembly[] { typeof(Program).Assembly });

            var c = Host.CreateClient("127.0.0.1", 17777);

            c.GetService<AccountServiceRef>("Account.App", "127.0.0.1", 17777).rpc_login("username", "password", (code) =>
            {
                Console.WriteLine("hello");
            });

            HostHelper.Run(c);
        }
    }
}
