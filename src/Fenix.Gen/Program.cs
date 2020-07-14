using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Fenix.Gen
{
    class Program
    {

		static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

		static void Main(string[] args)
        {
            var rootFolder = Directory.GetCurrentDirectory();
            var runtimeDll = File.ReadAllBytes(Path.Combine(rootFolder, "Fenix.Runtime.dll"));
            var appDll = File.ReadAllBytes(Path.Combine(rootFolder, "Server.App.dll"));
            Assembly asmRuntime = Assembly.Load(runtimeDll);
            Assembly asmApp = Assembly.Load(runtimeDll);

			assemblies[asmRuntime.ManifestModule.ScopeName] = asmRuntime;
			assemblies[asmApp.ManifestModule.ScopeName] = asmApp;

			foreach (Assembly value in assemblies.Values)
			{
				foreach (Type type in value.GetTypes())
				{
					if (type.IsAbstract)
					{
						continue;
					}

					Console.WriteLine(type.Name);

					//object[] objects = type.GetCustomAttributes(typeof(), true);
					//if (objects.Length == 0)
					//{
					//	continue;
					//}

					//foreach (BaseAttribute baseAttribute in objects)
					//{
					//	this.types.Add(baseAttribute.AttributeType, type);
					//}
				}
			}

		}
    }
}
