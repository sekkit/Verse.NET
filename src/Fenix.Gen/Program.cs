 
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

            string rootPath = Path.Combine(Path.Combine(rootFolder, "../../"));

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

            //if (args.Length != 2)
            //   return; 
            string rootPath = Path.Combine(Path.Combine(rootFolder, "../../"));
            if(args.Length >= 2)
                rootPath = args[1]; 
            string unityPath = Path.Combine(rootPath, "Unity");
            string sharedClientPath = Path.GetFullPath(Path.Combine(unityPath, "Assets/Client.App/Gen"));
            string sharedServerPath = Path.GetFullPath(Path.Combine(rootPath, "src/Server.App/Gen"));
            
            string clientPath = Path.GetFullPath(Path.Combine(unityPath, "Assets/Client.App/"));
            string serverPath = Path.GetFullPath(Path.Combine(rootPath, "src/Server.App"));

            var resolver = new DefaultAssemblyResolver(); 
            resolver.AddSearchDirectory(Path.Combine(rootPath, @"Libs/Unity/"));
            resolver.AddSearchDirectory(Path.Combine(unityPath, @"Library/ScriptAssemblies/"));
            resolver.AddSearchDirectory(Path.Combine(rootPath, @"bin/netcoreapp3.1/"));

            AssemblyDefinition adClient = AssemblyDefinition.ReadAssembly(
               Path.Combine(unityPath, @"Library/ScriptAssemblies/Assembly-CSharp.dll"), 
                new ReaderParameters { AssemblyResolver = resolver });

            if (args.Length == 0 || (args.Length >= 2 && args.First() == "-c"))
            {
                //Assembly.LoadFrom(Path.Combine(rootPath, @"Libs/Unity/Fenix.Runtime.Unity.dll"));
                //var asmClientApp = Assembly.LoadFrom(Path.Combine(rootPath, @"Libs/Unity/Assembly-CSharp.dll"));
                //Gen.AutogenActor(asmClientApp, false, sharedClientPath, sharedServerPath, clientPath, serverPath);
                Gen2.AutogenActor(adClient, false, sharedClientPath, sharedServerPath, clientPath, serverPath);

            }
            
            if (args.Length == 0 || (args.Length >= 2 && args.First() == "-r"))
            {
                Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/MessagePack.dll"));
                var asmRuntime = Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/Fenix.Runtime.dll"));
                Gen.AutogenHost(asmRuntime, Path.GetFullPath(Path.Combine(rootPath, "src/Fenix.Runtime/Common")), clientPath, serverPath);
            }
            
            if (args.Length == 0 || (args.Length >= 2 && args.First() == "-s"))
            {
                AssemblyDefinition adServerApp = AssemblyDefinition.ReadAssembly(
                   Path.Combine(rootPath, @"bin/netcoreapp3.1/Server.App.dll"),
                    new ReaderParameters { AssemblyResolver = resolver });

                //Gen2.AutogenActor(adServerApp, true, sharedClientPath, sharedServerPath, clientPath, serverPath);

                var asmServerApp = Assembly.LoadFrom(Path.Combine(rootPath, "bin/netcoreapp3.1/Server.App.dll"));
                Gen.AutogenActor(asmServerApp, true, sharedClientPath, sharedServerPath, clientPath, serverPath);
            }

            //var p = new ProtocolCode();
            //p.Validate();
#endif
        }
    }
}
