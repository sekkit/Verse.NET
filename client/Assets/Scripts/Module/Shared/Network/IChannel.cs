namespace Module.Shared
{
    using DataModel.Shared.Message;
 
#if UNITY_5_6_OR_NEWER
    public interface IChannel
    {
        void SendMsg(Msg msg);
    }
#else
public interface IChannel
{
    void SendMsg(Msg msg);

    void BroadcastMsg(string[] uids, Msg msg);

    void BroadcastAll(Msg msg);
}
#endif
}