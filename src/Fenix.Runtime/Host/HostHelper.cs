using Fenix.Common;
using System;
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

            try
            {
                while (true)
                {
                    try
                    {
                        if (h == null || h.IsAlive == false)
                            return; 
                        Thread.Sleep(1);
                        OneThreadSynchronizationContext.Instance.Update();
                        h.Update();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void Stop(Host host)
        { 
            host.Shutdown();
            host = null;
            th = null;
        }
    }
}
