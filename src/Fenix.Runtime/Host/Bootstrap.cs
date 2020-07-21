using Fenix;
using Fenix.Config;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text; 
 
namespace Fenix.Host
{
    public class Bootstrap
    {
        public static void Start(Assembly[] asmList, List<RuntimeConfig> cfgList, bool isMultiProcess=false)
        {
            Global.Init(asmList);
            Container c = null;
            if (!isMultiProcess) 
                c = Container.Create("App", 17777);
            
            if(cfgList != null)
            foreach(var cfg in cfgList)
            {
                string appName = Environment.GetEnvironmentVariable("AppName");

                if (!isMultiProcess)
                { 
                    foreach(var aName in cfg.DefaultActorNames)
                        c.CreateActor(aName, aName);
                }
                else
                {
                    if (appName != cfg.AppName)
                        continue;
                    if (c == null)
                        c = Container.Create(cfg.AppName, cfg.Port);
                    foreach (var aName in cfg.DefaultActorNames)
                        c.CreateActor(aName, aName);
                }
            } 

            ContainerHelper.Run(c);
        }
    }
}
