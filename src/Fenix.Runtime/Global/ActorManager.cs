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
        protected ConcurrentDictionary<string, ActorRef> containerDic = new ConcurrentDictionary<string, ActorRef>();
 
        public ActorRef GetActorRefByName(string name, Container fromContainer)
        {
            if(actorDic.ContainsKey(name))
            {
                return actorDic[name];
            }

            uint actorId = Global.IdManager.GetActorId(name);
            //uint containerId = Global.IdManager.GetContainerIdByActorId(actorId);

            var fromActor = fromContainer.GetActor(actorId); 
            actorDic[name] = ActorRef.Create(actorId, fromActor);
            return actorDic[name];
        }

        /*远程创建actor*/
        public void CreateActor()
        {

        }
    }
}
