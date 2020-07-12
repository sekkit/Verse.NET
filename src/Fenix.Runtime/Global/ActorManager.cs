using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class ActorManager
    {
        public static ActorManager Instance = new ActorManager();

        protected ConcurrentDictionary<string, ActorRef> actorDic = new ConcurrentDictionary<string, ActorRef>();
        protected ConcurrentDictionary<string, ActorRef> ContainerDic = new ConcurrentDictionary<string, ActorRef>();

        public ActorRef GetActorRefByName(string name)
        {
            if(!actorDic.ContainsKey(name))
            {
                return actorDic[name];
            }

            uint actorId = Global.IdManager.GetActorId(name);
            uint containerId = Global.IdManager.GetContainerIdByActorId(actorId);

            actorDic[name] = ActorRef.Create(actorId);
            return actorDic[name];
        }
    }
}
