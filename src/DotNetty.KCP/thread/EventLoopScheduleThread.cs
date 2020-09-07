using System;
using DotNetty.Transport.Channels;

namespace DotNetty.KCP.thread
{
    public class EventLoopScheduleThread : IScheduleThread
    {
#if UNITY_5_3_OR_NEWER
        private readonly IEventLoop _eventLoop = new SingleThreadEventLoop();
#else
        private readonly IEventLoop _eventLoop = new SingleThreadEventLoop(new DefaultEventLoopGroup());
#endif
        public void schedule(IScheduleTask scheduleTask, TimeSpan timeSpan)
        {
            _eventLoop.Schedule(scheduleTask, timeSpan);
        }

        public void stop()
        {
            if (_eventLoop.IsShuttingDown)
            {
                return;
            }
            _eventLoop.ShutdownGracefullyAsync();
        }
    }
}