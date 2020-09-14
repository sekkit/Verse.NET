using Fenix.Common;
using Fenix.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fenix
{
    public class HostHelper
    {
        static Thread hostThread;
        static Thread singleThread;

        public static void Run(Host host)
        {
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

            Loop(host);
        }

        public static void RunThread(Host host, List<RuntimeConfig> cfgList=null)
        {
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

            hostThread = new Thread(new ParameterizedThreadStart(Loop));
            hostThread.Start(new object[] { host, cfgList });

            singleThread = new Thread(new ThreadStart(Loop2));
            singleThread.Start(); 
        }

#if !CLIENT
        static bool RegisterHosts(Host host, List<RuntimeConfig> cfgList)
        {
            bool result = true;
            foreach (var otherCfg in cfgList)
            {    
                try
                { 
                    var hostRef = host.GetHost(otherCfg.AppName, otherCfg.InternalIP, otherCfg.Port);
                    var task = hostRef.SayHelloAsync();
                    task.Wait();
                    if (task.Result.code == DefaultErrCode.OK)
                    {
                        Log.Info("found host:", otherCfg.AppName, otherCfg.InternalIP, otherCfg.Port);
                    }
                    else
                    {
                        Log.Error("waiting for host:", otherCfg.AppName, otherCfg.InternalIP, otherCfg.Port);
                        result = false;
                    }              
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                } 
            };

            return result;
        }
#endif

        static void Loop(object param)
        {
            var paramList = (object[])param;

            var h = (Host)paramList[0];
            var cfgList = (List<RuntimeConfig>)paramList[1];

#if !CLIENT
            ThreadPool.QueueUserWorkItem((param2) =>
            {
                bool registered = false;
                var h2 = (Host)paramList[0];
                var cfgList2 = (List<RuntimeConfig>)paramList[1];
                while (!registered && !Global.Config.DuplexMode)
                {
                    if (cfgList2 != null && !registered)
                    {
                        registered = RegisterHosts(h2, cfgList2);
                        if (registered)
                            return;
                        Thread.Sleep(500);
                    }
                };
            }, param);

#endif

            while (true)
            {
                try
                {
                    if (h == null || h.IsAlive == false)
                        return; 
                    Thread.Sleep(10);
                    h.Update(); 
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }
        }

        static void Loop2()
        {
            while (true)
            {
                try
                { 
                    Thread.Sleep(1);
                    OneThreadSynchronizationContext.Instance.Update();
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }
        }

        public static void Update(Host host)
        { 
            OneThreadSynchronizationContext.Instance.Update();
            host.Update();
        }

        public static void Stop(Host host)
        {
            if (host != null)
            {
                host.Destroy();
                //host = null;
                Global.Host = null;
            }
            hostThread?.Abort();
            hostThread = null;
            singleThread?.Abort();
            singleThread = null;
        }
    }
}
