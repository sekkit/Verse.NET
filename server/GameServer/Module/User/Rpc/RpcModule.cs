 
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using DataModel.Shared.Message;
using MemoryPack;
using Module.Shared;  

namespace Module.User;

public class RpcModule : EntityModule
{
    public async Task Call(ProtoCode code, byte[] data, IChannel channel)
    {
        if (self.GetRpcMethods().TryGetValue(code, out var del))
        {
            var reqType = TypeProvider.Instance.GetReqType(code);
            var rspType = TypeProvider.Instance.GetRspType(code);
            object param = MemoryPackSerializer.Deserialize(reqType, data) as Msg;
            
            object result = null;
            if (del.Method.ReturnType.IsSubclassOf(typeof(Task)))
            { 
                dynamic task = del.DynamicInvoke(param);
                try
                {
                    result = await task;
                }
                catch (Exception ex)
                {
                    Shared.Log.Error(ex);
                    // Shared.Log.Error(Environment.StackTrace); 
                }
            }
            else
            {
                result = del.DynamicInvoke(param);
            }
            channel.SendMsg(result as Msg);
        }
        else
        {
            Module.Shared.Log.Error(string.Format("Invalid code {0}", code));
            channel.SendMsg(VoidMsg.Instance);
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