using Fenix;
using Fenix.Common.Utils;
using Fenix.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text; 
 
namespace Fenix
{
    public class Bootstrap
    {
        public static void Start(Assembly[] asmList, List<RuntimeConfig> cfgList, Action init, bool isMultiProcess=false)
        {
            Global.Init(asmList);

            init();

            var localAddrV4 = Basic.GetLocalIPv4(NetworkInterfaceType.Ethernet);

            Host c = null;
            if (!isMultiProcess) 
                c = Host.Create("App", localAddrV4, 17777, false);
            
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
                        c = Host.Create(cfg.AppName, localAddrV4, cfg.Port, false);
                    foreach (var aName in cfg.DefaultActorNames)
                        c.CreateActor(aName, aName);
                }
            }

            HostHelper.Run(c);
        }
    }
}
