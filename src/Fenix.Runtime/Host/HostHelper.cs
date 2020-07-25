using DotNetty.Codecs.Http.WebSockets;
using Fenix.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Fenix
{
    public class HostHelper
    {
        public static void Run(Host host)
        {
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

            try
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1);
                        OneThreadSynchronizationContext.Instance.Update();
                        host.Update();
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
    }
}
