 
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using DataModel.Shared.Message;
using MemoryPack;
using Module.Shared; 
using Service.Message;

namespace Module.User;

public class RpcModule : EntityModule
{
    public async Task Call(ProtoCode code, byte[] data, IChannel channel)
    {
        if (self.GetRpcMethods().TryGetValue(code, out var del))
        {
            var reqType = TypeService.Instance.GetReqType(code);
            var rspType = TypeService.Instance.GetRspType(code);
            object param = MemoryPackSerializer.Deserialize(reqType, data) as IMessage;
            //IMessage result = await func(param) as IMessage;
            //IMessage result =  func.DynamicInvoke(param);
            //Func<IMessage, Task<IMessage>> f = x => func.DynamicInvoke(x);
            
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
            channel.SendMsg(result as IMessage);
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