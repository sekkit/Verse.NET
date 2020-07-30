using System;

namespace DotNetty.KCP.thread
{
    public interface IExecutorPool
    {
         IMessageExecutor CreateMessageExecutor();

         void stop(bool stopImmediately);

         IMessageExecutor GetAutoMessageExecutor();


         void scheduleTask(IScheduleTask scheduleTask);

    }
}