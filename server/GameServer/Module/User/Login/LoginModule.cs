using DataModel.Shared.Message;
using Module.Shared;
using Service.Entity;
using Service.Login;

namespace Module.User.Login;


public class LoginModule : EntityModule
{
    [ServerApi(ProtoCode.LOGIN)]
    public async Task<LoginRsp> rpcLogin(LoginReq req)
    {
        var result = await LoginService.Instance.Login(req.Username, req.Password);
        var rsp = new LoginRsp
        {
            Uid = result.Item2,
            RetCode = (int)result.Item1
        };
        if (result.Item1 == LoginCode.Ok)
        {
            self.SetUid(result.Item2);
            self.AddModule<ItemModule>();
            self.AddModule<TestModule>();
            //TODO:
            //...
            EntityService.Instance.RegisterEntity(self);
        }
        return rsp;
    } 
    
    public override void Start()
    {
    }

    public override void Update()
    {
        
    }

    public override void LateUpdate()
    {
        
    }

    public override void FrameFinishedUpdate()
    {
        
    }
    
    public override void Destroy()
    {
        EntityService.Instance.UnregisterEntity(self);
    }
}