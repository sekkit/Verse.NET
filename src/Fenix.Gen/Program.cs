using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Fenix
{
    class Program
    {
		//static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>(); 

		static void Main(string[] args)
        {
            var rootFolder = Directory.GetCurrentDirectory();
            var runtimeDll = File.ReadAllBytes(Path.Combine(rootFolder, "Fenix.Runtime.dll"));
            var appDll = File.ReadAllBytes(Path.Combine(rootFolder, "Server.App.dll"));
            Assembly asmRuntime = Assembly.Load(runtimeDll);
            Assembly asmApp = Assembly.Load(appDll);
            ;
			Gen.ProcessApi(asmRuntime, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../src/Server.App/Shared"));
			Gen.ProcessApi(asmApp, "");
		}
    }
}
