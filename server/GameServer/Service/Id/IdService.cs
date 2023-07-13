using Module.Shared;

namespace Service.Id;

public class IdService : Singleton<IdService>, ILifecycle
{
     
    private static ulong _idNumber = 0;
 
    public static ulong GenNewSeqId()
    {
        if (_idNumber >= ulong.MaxValue)
        {
            _idNumber = 0;
        }
        return Interlocked.Increment (ref _idNumber); 
    }
    
    public void Start()
    {
        
    }

    public void Update()
    {
        
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
}