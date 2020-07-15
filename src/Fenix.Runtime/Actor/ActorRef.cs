using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    //Proxy for Actor
    //public class ActorRef<T>
    //{
        
    //}

    public partial class ActorRef
    {
        public Container fromContainer;

        public uint actorId;

        public static ActorRef Create(uint actorId, Container fromContainer)
        {
            var obj = new ActorRef();
            obj.actorId = actorId;

            obj.fromContainer = fromContainer;

            return obj;
        } 

        public void CallActorMethod(uint protocolCode, object msg)
        {
            var toContainerId = Global.IdManager.GetContainerIdByActorId(this.actorId);

            //NetManager.Instance.SendTo(this.actorId, );

            var NetPeer = NetManager.Instance.GetPeerById(toContainerId);


            
        }
    }
}