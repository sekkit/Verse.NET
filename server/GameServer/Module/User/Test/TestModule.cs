using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using DataModel.Shared.Message;
using MemoryPack;
using Module.Shared;  

namespace Module.User;

public class TestModule : EntityModule
{
    [ServerApi(ProtoCode.TEST_NTF)]
    public async Task rpcTestNtf(TestNtfReq req)
    {
        await self.Get<RpcModule>().Notify(ProtoCode.ON_TEST, new TestNtf() { TestMsg = "helloworld" });

        await self.Get<UserModule>().SyncField("nickname");
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
        
    }
}