using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class ActorManager
    {
        public static ActorManager Instance = new ActorManager();

        //protected ConcurrentDictionary<string, ActorRef> actorDic = new ConcurrentDictionary<string, ActorRef>();
        //protected ConcurrentDictionary<string, ActorRef> hostDic = new ConcurrentDictionary<string, ActorRef>();
        
        //

        public ActorRef GetActorRefByName(string name, Actor fromActor)
        {
            //if(actorDic.ContainsKey(name))
            //{
            //    return actorDic[name];
            //}

            uint toActorId = Global.IdManager.GetActorId(name);
            if (toActorId == 0)
                return null;
            //uint hostId = Global.IdManager.GetHostIdByActorId(actorId); 
            //var toActor = Host.Instance.GetActor(toActorId); 
            //actorDic[name] = ActorRef.Create(toActorId, fromActor);
            //return actorDic[name];
            return ActorRef.Create(toActorId, fromActor);
        }

        /*远程创建actor*/
        //public void CreateActor()
        //{
        //    
        //}
    }
}
