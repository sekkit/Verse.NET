using DataModel.Shared.Model;
using Module.Shared;

namespace Service.Db;

public class DbService: Singleton<DbService>, ILifecycle
{
    public User LoadUserFromDb(string uid)
    {
        return new User()
        {
            Uid = uid,
            Nickname = "sekkit"
        };
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