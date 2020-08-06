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
             
            Host host = null;
            if (!isMultiProcess || cfgList == null)
            { 
                var cfg = Global.Config;
                string localAddrV4 = "";
                if (cfg.InternalIp == "auto")
                    localAddrV4 = Basic.GetLocalIPv4(NetworkInterfaceType.Ethernet);
                host = Host.Create("App", localAddrV4, cfg.ExternalIp, 17777, false);

                foreach (var aName in cfg.DefaultActorNames) 
                    host.CreateActorLocally(aName, aName);
            }
            else
            {
                foreach (var cfg in cfgList)
                {
                    string appName = Environment.GetEnvironmentVariable("AppName");
                    if (!isMultiProcess)
                    {
                        foreach (var aName in cfg.DefaultActorNames)
                            host.CreateActorLocally(aName, aName);
                    }
                    else
                    {
                        if (appName != cfg.AppName)
                            continue;
                        if (host == null)
                        {
                            //if(cfg.InternalIp == "auto")
                            //var localAddrV4 = Basic.GetLocalIPv4(NetworkInterfaceType.Ethernet);
                            host = Host.Create(cfg.AppName, cfg.InternalIp, cfg.ExternalIp, cfg.Port, false);
                        }
                        foreach (var aName in cfg.DefaultActorNames)
                            host.CreateActorLocally(aName, aName);
                    }
                }
            }

            HostHelper.RunThread(host);
        }
    }
}
