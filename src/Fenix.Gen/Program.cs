 
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Fenix
{
    class Program
    {
        static void Main(string[] args)
        {
#if !UNITY_5_3_OR_NEWER

            var rootFolder = Directory.GetCurrentDirectory();

            string rootPath = Path.Combine(Path.Combine(rootFolder, "../../../../../"));

            if (args.Length == 2)
                rootPath = args[1];

            string clientPath = Path.Combine(rootPath, "src/Client.App");
            string serverPath = Path.Combine(rootPath, "src/Server.App");

            string sharedClientPath = Path.GetFullPath(Path.Combine(rootPath, "src/Client.App/Gen"));
            string sharedServerPath = Path.GetFullPath(Path.Combine(rootPath, "src/Server.App/Gen"));
 
            if (args.Length != 2 || args.First() == "-r")
            {
                var asmRuntime = Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/Fenix.Runtime.dll"));
                Gen.AutogenHost(asmRuntime, Path.Combine(rootPath, "src/Fenix.Runtime/Common"),
                    Path.Combine(rootPath, "src/Client.App"),
                    Path.Combine(rootPath, "src/Server.App"));
            }
            else if (args.Length != 2 || args.First() == "-c")
            {
                var resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory(Path.Combine(rootPath, @"src/Client.App/bin/Debug/netcoreapp3.1/Client.App/netcoreapp3.1/"));
                //Assembly asmClientApp = Assembly.LoadFrom(Path.Combine(rootPath, @"src\Client.App\bin\Debug\netcoreapp3.1\Client.App\netcoreapp3.1\Client.App.dll"));
                var ad = AssemblyDefinition.ReadAssembly(
                    Path.Combine(rootPath, @"src/Client.App/bin/Debug/netcoreapp3.1/Client.App/netcoreapp3.1/Client.App.dll"),
                    new ReaderParameters() { AssemblyResolver = resolver });
                //Gen.AutogenActor(asmClientApp, false, sharedClientPath, sharedServerPath, clientPath, serverPath);
                Gen2.AutogenActor(ad, false, sharedClientPath, sharedServerPath, clientPath, serverPath);
            }
            else if (args.Length != 2 || args.First() == "-s")
            {
                Assembly asmServerApp = Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/Server.App.dll"));
                Gen.AutogenActor(asmServerApp, true, sharedClientPath, sharedServerPath, clientPath, serverPath);
            }
            
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
