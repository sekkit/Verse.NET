using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
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

        public void CallActorMethod(uint protocolCode, object msg, bool hasCallback)
        {
            fromContainer.Rpc(protocolCode, this.actorId, msg, hasCallback);
        }
    }
}