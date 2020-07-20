/*
 * RpcCommand
 */

using DotNetty.Common.Utilities;
using Fenix;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using static Fenix.Common.RpcUtil;

public class RpcCommand
{
    public ulong Id;

    public uint FromActorId;

    public uint ToActorId;

    public uint FromContainerId;

    public uint ToContainerId;

    public IMessage Msg;

    public Api RpcType;

    public uint ProtoCode;

    protected RpcModule mInvoker;

    protected RpcCommand()
    {
        this.Id = Basic.GenID64();
    }
    
    public static RpcCommand Create(
        uint fromContainerId,
        uint toContainerId,
        uint fromActorId,
        uint toActorId, 
        uint protoCode,
        IMessage msg,
        RpcModule invoker)
    {
        var obj             = new RpcCommand();
        obj.Msg             = msg;
        obj.FromContainerId = fromContainerId;
        obj.ToContainerId   = toContainerId;
        obj.FromActorId     = fromActorId;
        obj.ToActorId       = toActorId;
        obj.mInvoker        = invoker;
        obj.RpcType         = invoker.GetRpcType(protoCode);
        obj.ProtoCode       = protoCode;
        //Type type         = Global.TypeManager.GetMessageType(obj.ProtoCode);
        //obj.mMsgObj       = MessagePackSerializer.Deserialize(type, obj.packet.Payload);
        return obj;
    }

    public T ToMessage<T>() where T : IMessage
    {
        return this.Msg as T;
        //Type type = Global.TypeManager.GetMessageType(this.ProtoCode);
        //var msgObj    = MessagePackSerializer.Deserialize(type, this.msg.Pack());
        //return (T)msgObj;
    }

    public void Call()
    {
        if(Global.IsServer)
        {
            if (RpcType == Api.ServerApi)
                this.mInvoker.CallLocalMethod(this.ProtoCode, this.Msg);
            else if (RpcType == Api.ServerOnly)
                this.mInvoker.CallLocalMethod(this.ProtoCode, this.Msg); 
        }
        else
        {
            if (RpcType == Api.ClientApi)
                this.mInvoker.CallLocalMethod(this.ProtoCode, this.Msg);
        }
    }

    public void Callback(byte[] cbMsg)
    {
        //SpawnActorMsg.Callback cb_msg;
        //var peer = NetManager.Instance.GetPeerById(this.fromPeerId);
        //SpawnActorMsg _msg = new SpawnActorMsg();
        //_msg.callback = cbMsg;
        //this.msg.Payload = MessagePackSerializer.Serialize(_msg);

        //int payloadSz = this.msg.Payload.Length;
        //byte[] bytes = new byte[8 + 4 + payloadSz];

        //Array.Copy(BitConverter.GetBytes(this.msg.Id), 0, bytes, 0, 8);
        //Array.Copy(BitConverter.GetBytes(this.msg.ProtocolId), 0, bytes, 8, 4);
        //Array.Copy(BitConverter.GetBytes(this.msg.Id), 0, bytes, 12, payloadSz);  

        //peer.Send(bytes);
    }
}


