/*
 * 
 */

using Fenix;
using Fenix.Common;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using static Fenix.Common.RpcUtil;

public class RpcCommand
{ 
    public uint fromPeerId;

    public Message msg;

    public Api RpcType;

    public uint ProtocolId => msg.ProtocolId;

    public ulong Id => msg.Id;

    protected object mMsgObj;

    protected RpcModule mInvoker;

    protected RpcCommand()
    {
    }

    public static RpcCommand Create(uint fromPeerId, Api apiType, Message msg, RpcModule invoker)
    {
        var obj = new RpcCommand();
        obj.msg = msg;
        obj.fromPeerId = fromPeerId;
        obj.mInvoker = invoker;
        obj.RpcType = apiType;
        Type type = Global.TypeManager.GetMessageType(msg.ProtocolId);
        obj.mMsgObj = MessagePackSerializer.Deserialize(type, obj.msg.Payload);
        return obj;
    }

    public T ToMessage<T>()
    {
        return (T)this.mMsgObj;
    }

    public void Call()
    {
        if(Global.IsServer)
        {
            if (RpcType == Api.ServerApi)
                this.mInvoker.CallLocalMethod(this.msg.ProtocolId, this.mMsgObj);
            else if (RpcType == Api.ServerOnly)
                this.mInvoker.CallLocalMethod(this.msg.ProtocolId, this.mMsgObj);  
        }
        else
        {
            if (RpcType == Api.ClientApi)
                this.mInvoker.CallLocalMethod(this.msg.ProtocolId, this.mMsgObj);
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


