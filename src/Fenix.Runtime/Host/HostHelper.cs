using Fenix.Common;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading;

namespace Fenix
{
    public class HostHelper
    {
        static Thread th;

        public static void Run(Host host)
        {
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

            Loop(host);
        }

        public static void RunThread(Host host)
        {
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

            th = new Thread(new ParameterizedThreadStart(Loop));
            th.Start(host);
        }

        static void Loop(object host)
        {
            var h = (Host)host;
             
            while (true)
            {
                try
                {
                    if (h == null || h.IsAlive == false)
                        return; 
                    Thread.Sleep(100);
                    Update(h);
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
            host.Destroy();
            host = null;
            th = null;
        }
    }
}
