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
        public uint actorId;

        public static ActorRef Create(uint actorId)
        {
            var obj = new ActorRef();
            obj.actorId = actorId;

            return obj;
        } 
    }
}