using System;
using System.Threading.Tasks;

namespace Module.Shared
{
    using DataModel.Shared.Message;
 
#if UNITY_5_6_OR_NEWER
    public interface IChannel
    {
        void InvokeWithCb<Rsp>(ProtoCode code, Msg msg, Action<Rsp> callback) where Rsp: Msg;
        
        Task<Rsp> InvokeWithCb<Rsp>(ProtoCode code, Msg msg) where Rsp: Msg;

        void Invoke(ProtoCode code, Msg msg);
    }
#else
    public interface IChannel
    {
        string GetChannelId();
        
        Task Reply(ProtoCode code, Msg msg);

        Task Notify(ProtoCode code, string[] uids, Msg msg);

        Task NotifyAll(ProtoCode code, Msg msg);
    }
#endif
}