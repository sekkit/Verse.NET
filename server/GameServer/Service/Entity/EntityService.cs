using System.Collections;
using System.Collections.Concurrent;
using DataModel.Shared.Message;
using Module.Shared;
using Module.User;
using WebSocketSharp.Server;

namespace Service.Entity;

public class EntityService : Singleton<EntityService>, ILifecycle
{
    private ConcurrentDictionary<string, Module.Shared.Entity> _entities = new(); 
    private DoubleMap<string, string> _uid2sid = new();
    
    public void RegisterChannel(string uid, string sid)
    {
        _uid2sid.Add(uid, sid);
    }
 
    public void UnregisterChannel(string uid)
    {
        if (_uid2sid.ContainsKey(uid))
        {
            _uid2sid.RemoveByKey(uid);
        }
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
                Log.Error(string.Format("{0} has no session id", uid));
            }
        }

        return result;
    }
    
    public void Start()
    {
        foreach (var entity in _entities.Values)
        {
            if(entity.IsEnabled) entity.Start();
        }
    }

    public void Update()
    {
        foreach (var entity in _entities.Values)
        {
            if(entity.IsEnabled) entity.Update();
        }
    }

    public void LateUpdate()
    {
        foreach (var entity in _entities.Values)
        {
            if(entity.IsEnabled) entity.LateUpdate();
        }
    }

    public void FrameFinishedUpdate()
    {
        foreach (var entity in _entities.Values)
        {
            if(entity.IsEnabled) entity.FrameFinishedUpdate();
        }
    }

    public void Destroy()
    {
        foreach (var entity in _entities.Values)
        {
            entity.Destroy();
        }
    }

    public void RegisterEntity(Module.Shared.Entity entity)
    {
        _entities.TryAdd(entity.Uid, entity);
        RegisterChannel(entity.Uid, entity.GetChannel().GetChannelId());
    }

    public void UnregisterEntity(Module.Shared.Entity entity)
    {
        _entities.TryRemove(entity.Uid, out var _);
        UnregisterChannel(entity.Uid);
    }

    public async Task NotifyAll(ProtoCode code, Msg msg)
    { 
        if (_entities.Count > 0)
        {
            await _entities.FirstOrDefault().Value?.GetChannel()?.NotifyAll(code, msg);
        }
    }
}