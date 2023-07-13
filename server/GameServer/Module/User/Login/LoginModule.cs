using DataModel.Shared.Message;
using Module.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Service.Db;
using Service.Entity;
using Service.Login;

namespace Module.User.Login;


public class LoginModule : EntityModule
{
    [ServerApi(ProtoCode.LOGIN)]
    public async Task<LoginRsp> rpcLogin(LoginReq req)
    {
        var result = await LoginService.Instance.Login(req.Username, req.Password);
        if (result.Item1 == LoginCode.Ok)
        {
            var user = DbService.Instance.LoadUserFromDb(result.Item2);
            self.User = user;
            
            self.SetUid(result.Item2);
            self.AddModule<UserModule>();
            self.AddModule<ItemModule>();
            self.AddModule<TestModule>();
            //TODO:
            //...
            EntityService.Instance.RegisterEntity(self);
            
            using (var ms = new MemoryStream())
            {
                using (var datawriter = new BsonDataWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(datawriter, self.User);
                } 
                return new LoginRsp
                {
                    Uid = result.Item2,
                    RetCode = (int)result.Item1,
                    UserBytes = ms.ToArray(),
                };
            }
        }
        
        return new LoginRsp
        {
            Uid = result.Item2,
            RetCode = (int)result.Item1,
            UserBytes = null,
        };
    } 
    
    public override void Start()
    {
        self.AddTimer(3000, 5000, true, () =>
        {
            Shared.Log.Info("Hello");
            self.Get<RpcModule>().Notify(ProtoCode.ON_TEST, new TestNtf(){ TestMsg = "sekkit"});
        });
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