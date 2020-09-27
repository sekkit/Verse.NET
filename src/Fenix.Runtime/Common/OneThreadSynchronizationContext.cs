using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Fenix.Common
{
	public class OneThreadSynchronizationContext : SynchronizationContext
	{
		public static OneThreadSynchronizationContext Instance { get; } = new OneThreadSynchronizationContext();

		private readonly int selfThreadId = Thread.CurrentThread.ManagedThreadId;

		// 
		// 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
		// 
		private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

		private Action a;

		public void Update()
		{
			while (true)
			{
				if (!this.queue.TryDequeue(out a))
					return;
				try
				{
					a();
				}
				catch(Exception ex)
                {
					Log.Error(ex);
                }
			}
		}

        public override void Post(SendOrPostCallback callback, object state)
        {
            if (Thread.CurrentThread.ManagedThreadId == this.selfThreadId)
            {
				try
				{
					callback(state);
				}
				catch(Exception ex)
                {
					Log.Error(ex);
				}
                return;
            }

            this.queue.Enqueue(() => { callback(state); });
        }
    }
}
