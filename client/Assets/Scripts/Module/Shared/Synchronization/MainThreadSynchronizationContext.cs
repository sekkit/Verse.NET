using System;
using System.Threading;

namespace Module.Shared
{
public class MainThreadSynchronizationContext: Singleton<MainThreadSynchronizationContext>, ILifecycle
{
    private readonly ThreadSynchronizationContext threadSynchronizationContext = new ThreadSynchronizationContext();

    public MainThreadSynchronizationContext()
    {
        SynchronizationContext.SetSynchronizationContext(this.threadSynchronizationContext);
    }

    public void Start()
    {
       
    }

    public void Update()
    {
        this.threadSynchronizationContext.Update();
    }

    public void LateUpdate()
    {
        
    }

    public void FrameFinishedUpdate()
    {
        
    }

    public void Destroy()
    {
        
    }

    public void Post(SendOrPostCallback callback, object state)
    {
        this.Post(() => callback(state));
    }
	
    public void Post(Action action)
    {
        this.threadSynchronizationContext.Post(action);
    }
}
}