 
using System.Collections.Concurrent;
using DataModel.Shared.Message;
using MemoryPack;
using Module.Shared;
using Service.Message;

namespace Module.User;


public class RpcModule : EntityModule
{
    private ConcurrentDictionary<ProtoCode, Func<IMessage, Task<IMessage>>> _rpcMethods { get; set; } = new();
  
    public async Task Call(ProtoCode code, byte[] data, IChannel channel)
    {
        if (_rpcMethods.TryGetValue(code, out var func))
        {
            var reqType = TypeService.Instance.GetReqType(code);
            var rspType = TypeService.Instance.GetRspType(code);
            IMessage param = MemoryPackSerializer.Deserialize(reqType, data) as IMessage;
            IMessage result = await func(param);
            channel.SendMsg(result);
        }
        else
        {
            Module.Shared.Log.Error(string.Format("Invalid code {0}", code));
            channel.SendMsg(VoidMessage.Instance);
        } 
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