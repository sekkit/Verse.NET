using DotNetty.KCP.thread;
using DotNetty.Common.Concurrency;
using DotNetty.Common.Utilities;

namespace DotNetty.KCP
{
    public interface IScheduleTask:ITimerTask,IRunnable
    {
        
    }
}