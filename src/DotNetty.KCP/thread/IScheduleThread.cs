using System;
using DotNetty.Common.Utilities;

namespace DotNetty.KCP.thread
{
    public interface IScheduleThread
    {
        void schedule(IScheduleTask scheduleTask,TimeSpan timeSpan);


        void stop();
    }
}