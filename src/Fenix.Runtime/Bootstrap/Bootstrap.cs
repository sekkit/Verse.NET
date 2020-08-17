using Fenix;
using Fenix.Common;
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
        public static void StartMultiProcess(Assembly[] asmList, RuntimeConfig cfg, Action init)
        {
#if !CLIENT
            Global.Init(cfg, asmList);

            init();
             
            Host host = null;
          
            Environment.SetEnvironmentVariable("AppName", cfg.AppName);
            string appName = Environment.GetEnvironmentVariable("AppName");
         
            if (host == null)
            {
                //if(cfg.InternalIp == "auto")
                //var localAddrV4 = Basic.GetLocalIPv4(NetworkInterfaceType.Ethernet);
                host = Host.Create(cfg.AppName, cfg.InternalIP, cfg.ExternalIP, cfg.Port, false);
            }

            foreach (var aName in cfg.DefaultActorNames)
                host.CreateActorLocally(aName, aName); 

            HostHelper.RunThread(host);
#endif
        }

        public static void StartSingleProcess(Assembly[] asmList, List<RuntimeConfig> cfgList, Action init)
        {
#if !CLIENT
            Global.Init(null, asmList);

            init();

            Host host = null;
        
            string localAddrV4 = "";
            if (Global.Config.InternalIP == "auto")
                localAddrV4 = Basic.GetLocalIPv4(NetworkInterfaceType.Ethernet);

            host = Host.Create("App", localAddrV4, Global.Config.ExternalIP, 17777, false);
            foreach(var cfg in cfgList)
                foreach (var aName in cfg.DefaultActorNames)
                    host.CreateActorLocally(aName, aName); 

            HostHelper.RunThread(host);
#endif
        }
    }
}
