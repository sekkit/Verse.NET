using Module.Shared;

namespace Service.Id;

public class IdService : Singleton<IdService>, ILifecycle
{
    private DoubleMap<string, string> _uid2sid = new();
    
    private static ulong _idNumber = 0;

    public void RegisterUser(string uid, string sid)
    {
        _uid2sid.Add(uid, sid);
    }
 
    public void UnregisterUser(string uid)
    {
        _uid2sid.RemoveByKey(uid);
    }

    public string GetUidBySid(string uid) => _uid2sid.GetValueByKey(uid);

    public string GetSidByUid(string sid) => _uid2sid.GetKeyByValue(sid);
    
    public IEnumerable<string> GetSidByUids(string[] uids)
    {
        var result = new List<string>();
        foreach (var uid in uids)
        {
            string sid = _uid2sid.GetValueByKey(uid);
            if (sid != null)
            {
                result.Add(sid);
            }
            else
            {
                Log.Error(string.Format("{0} has no session", uid));
            }
        }

        return result;
    }

    public static ulong GenNewId()
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