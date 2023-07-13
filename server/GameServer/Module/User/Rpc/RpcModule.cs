 
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
            var reqType = ProtocolProvider.Instance.GetReqType(code);
            //var rspType = ProtocolProvider.Instance.GetRspType(code);
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

            if (result != null && (param as Msg).RpcId != 0) //has no callback
            {
                (result as Msg).RpcId = (param as Msg).RpcId;
                channel.Reply(code, result as Msg);
            } 
        }
        else
        {
            Module.Shared.Log.Error(string.Format("Invalid code {0}", code));
            //channel.Reply(ProtoCode.VOID, new VoidMsg());
        }
    }

    public async Task Notify(ProtoCode code, Msg msg)
    {
        self.GetChannel()?.Notify(code, new string[]{ self.Uid }, msg);
    }
    
    public async Task NotifyAll(ProtoCode code, Msg msg)
    {
        self.GetChannel()?.NotifyAll(code, msg);
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