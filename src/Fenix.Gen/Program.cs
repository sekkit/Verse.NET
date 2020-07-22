using Shared.Protocol;
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
            //var runtimeDll = File.ReadAllBytes(Path.Combine(rootFolder, "Fenix.Runtime.dll"));
            //var appDll = File.ReadAllBytes(Path.Combine(rootFolder, "Server.App.dll"));
            //Assembly asmRuntime = Assembly.Load(runtimeDll);
            //Assembly asmApp = Assembly.Load(appDll);

            //Assembly asmRuntime = typeof(Host).Assembly;
            //Assembly asmServerApp = typeof(Server.UModule.Avatar).Assembly;
            //Assembly asmClientApp = typeof(Client.Avatar).Assembly;

            Assembly asmApp = typeof(ErrCode).Assembly;

            string sharedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Shared/Gen");
            string clientPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Shared/Client");
            string serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Shared/Server");

            //Gen.Autogen(asmServerApp, sharedPath, clientPath, serverPath);
            //Gen.Autogen(asmClientApp, sharedPath, clientPath, serverPath);

            Gen.Autogen(asmApp, sharedPath, clientPath, serverPath);
        }
    }
}
