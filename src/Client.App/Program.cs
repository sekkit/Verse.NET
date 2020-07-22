using Fenix;
using Server;
using System;

namespace Client.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = ClientHost.Create("127.0.0.1", 17777);

            c.GetActorRef<AccountServiceRef>("").rpc_login("username", "password", (code) =>
            {

            });
        }
    }
}
