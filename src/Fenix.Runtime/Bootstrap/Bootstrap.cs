using Fenix;
using Fenix.Config;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text; 
 
namespace Fenix
{
    public class Bootstrap
    {
        public static void Start(Assembly[] asmList, List<RuntimeConfig> cfgList, bool isMultiProcess=false)
        {
            Global.Init(asmList);
            Host c = null;
            if (!isMultiProcess) 
                c = Host.Create("App", "0.0.0.0", 17777, false);
            
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
                        c = Host.Create(cfg.AppName, "0.0.0.0", cfg.Port, false);
                    foreach (var aName in cfg.DefaultActorNames)
                        c.CreateActor(aName, aName);
                }
            } 

            HostHelper.Run(c);
        }
    }
}
