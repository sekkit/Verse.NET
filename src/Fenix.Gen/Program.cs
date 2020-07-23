
using Shared;
using System;
using System.IO;
using System.Reflection;

namespace Fenix
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootFolder = Directory.GetCurrentDirectory();
            var clientDll = File.ReadAllBytes(Path.Combine(rootFolder, "../../../../../src/Client.App/bin/Debug/netcoreapp3.1/Client.App.dll"));
            var serverDll = File.ReadAllBytes(Path.Combine(rootFolder, "../../../../../bin/netcoreapp3.1/Server.App.dll"));
            Assembly asmClientApp = Assembly.Load(clientDll);
            Assembly asmServerApp = Assembly.Load(serverDll);
            //Assembly asmServerApp = typeof(Server.UModule.Avatar).Assembly;// Assembly.Load(serverDll);

            //Assembly asmRuntime = typeof(Host).Assembly;
            //Assembly asmServerApp = typeof(Server.UModule.Avatar).Assembly;
            //Assembly asmClientApp = typeof(Client.Avatar).Assembly;

            //Assembly asmApp = typeof(ErrCode).Assembly;

            string sharedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Shared/Gen");
            string clientPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Client.App");
            string serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Server.App");

            Gen.Autogen(asmServerApp, true, sharedPath, clientPath, serverPath); 
            Gen.Autogen(asmClientApp, false, sharedPath, clientPath, serverPath);

            var p = new ProtocolCode();
            p.Validate();

            //Gen.Autogen(asmApp, sharedPath, clientPath, serverPath);
        }
    }
}
