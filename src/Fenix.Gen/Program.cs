
//using Shared;
using System;
using System.IO;
using System.Reflection;

namespace Fenix
{
    class Program
    {
        static void Main(string[] args)
        {
#if !UNITY_5_3_OR_NEWER

            var rootFolder = Directory.GetCurrentDirectory();

            string rootPath = Path.Combine(Path.Combine(rootFolder, "../../../../../"));

            Assembly asmClientApp = Assembly.LoadFrom(Path.Combine(rootPath, @"src\Client.App\bin\Debug\netcoreapp3.1\Client.App\netcoreapp3.1\Client.App.dll"));
            Assembly asmServerApp = Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/Server.App.dll")); 

            Assembly asmRuntime = Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/Fenix.Runtime.dll")); 

            string clientPath = Path.Combine(rootPath, "src/Client.App");
            string serverPath = Path.Combine(rootPath, "src/Server.App");

            string sharedClientPath = Path.GetFullPath(Path.Combine(rootPath, "src/Client.App/Gen"));
            string sharedServerPath = Path.GetFullPath(Path.Combine(rootPath, "src/Server.App/Gen"));

            Gen.AutogenHost(asmRuntime, Path.Combine(rootPath, "src/Fenix.Runtime/Common"),
                Path.Combine(rootPath, "src/Client.App"),
                Path.Combine(rootPath, "src/Server.App")); 

            Gen.AutogenActor(asmServerApp, true, sharedClientPath, sharedServerPath, clientPath, serverPath); 
            Gen.AutogenActor(asmClientApp, false, sharedClientPath, sharedServerPath, clientPath, serverPath);

            //var p = new ProtocolCode();
            //p.Validate();

#else
            var rootFolder = Directory.GetCurrentDirectory();

            string rootPath = Path.Combine(Path.Combine(rootFolder, "../../../../../../../"));
            string unityPath = Path.Combine(rootPath, "Unity");

            var asmClientApp = Assembly.LoadFrom(Path.Combine(unityPath, "Assets/Plugins/Fenix/Client.App.dll"));
            var asmServerApp = Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/Server.App.dll"));
            var asmRuntime = Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/Fenix.Runtime.dll"));

            string sharedClientPath = Path.GetFullPath(Path.Combine(unityPath, "Assets/Scripts/Client/Gen"));
            string sharedServerPath = Path.GetFullPath(Path.Combine(rootPath, "src/Server.App/Gen"));

            string clientPath = Path.GetFullPath(Path.Combine(unityPath, "Assets/Scripts/Client"));
            string serverPath = Path.GetFullPath(Path.Combine(rootPath, "src/Server.App"));

            Gen.AutogenHost(asmRuntime, Path.GetFullPath(Path.Combine(rootPath, "src/Fenix.Runtime/Common")), clientPath, serverPath);
             
            Gen.AutogenActor(asmServerApp, true, sharedClientPath, sharedServerPath, clientPath, serverPath);

            Assembly.LoadFrom(Path.Combine(unityPath, "Assets/Plugins/Fenix/Fenix.Runtime.Unity.dll"));
            Assembly.LoadFrom(Path.Combine(unityPath, "Assets/Plugins/Fenix/Shared.Unity.dll"));

            Gen.AutogenActor(asmClientApp, false, sharedClientPath, sharedServerPath, clientPath, serverPath);
            
            //var p = new ProtocolCode();
            //p.Validate();
#endif
        }
    }
}
