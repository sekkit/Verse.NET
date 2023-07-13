using Module.Helper;
using Module.Shared;

namespace Service.Timer;

public class TimerService : Singleton<TimerService>, ILifecycle
{
    private volatile List<Module.Shared.Timer> _timers = new();
    
    public Module.Shared.Timer AddTimer(long delay, long interval, bool repeated, Action cb)
    {
        var timer = Module.Shared.Timer.Create(delay, interval, repeated, cb); 
        _timers.Add(timer);
        return timer;
    }
    
    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    public void LateUpdate()
    {
        foreach (var timer in _timers.ToArray())
        {
            if (timer.CheckDoneOrTimeout(TimeHelper.GetTimeStampMS()))
            {
                _timers.Remove(timer);
            }
        } 
    } 

    public void FrameFinishedUpdate()
    {
        
    }

    public void Destroy()
    {
        
    }
}