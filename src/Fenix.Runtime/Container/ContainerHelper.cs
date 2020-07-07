using Fenix.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Fenix
{
    public class ContainerHelper
    {
        public static void Run(Container container)
        {
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

            try
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(0);
                        OneThreadSynchronizationContext.Instance.Update();
                        container.Update();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
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
