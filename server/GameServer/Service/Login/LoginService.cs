using DataModel.Shared.Message;
using Module.Shared;
using Service.Entity;

namespace Service.Login;

public class LoginService : Singleton<LoginService>, ILifecycle
{ 
    public async Task<Tuple<LoginCode, string>> Login(string username, string password)
    {
        if (EntityService.Instance.HasEntity("123456"))
        {
            return new Tuple<LoginCode, string>(LoginCode.Fail, "123456");
        }
        //get user from redis  
        return new Tuple<LoginCode, string>(LoginCode.Ok, "123456");
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