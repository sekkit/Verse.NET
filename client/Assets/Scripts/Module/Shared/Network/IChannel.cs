namespace Module.Shared
{
    using DataModel.Shared.Message;
 
#if UNITY_5_6_OR_NEWER
    public interface IChannel
    {
        void SendMsg(IMessage msg);
    }
#else
public interface IChannel
{
    void SendMsg(IMessage msg);

    void BroadcastMsg(string[] uids, IMessage msg);

    void BroadcastAll(IMessage msg);
}
#endif
}