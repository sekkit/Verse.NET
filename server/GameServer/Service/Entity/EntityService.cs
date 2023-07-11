using System.Collections;
using System.Collections.Concurrent;
using Module.Shared; 

namespace Service.Entity;

public class EntityService : Singleton<EntityService>, ILifecycle
{
    private ConcurrentDictionary<string, Module.Shared.Entity> _entities = new(); 

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
    }

    public void UnregisterEntity(Module.Shared.Entity entity)
    {
        _entities.TryRemove(entity.Uid, out var _);
    }
}