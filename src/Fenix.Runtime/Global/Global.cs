using CSRedis;
using Fenix.Common;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Fenix
{
    public partial class Global
    {
        public static bool IsServer = true;

        public static bool NativeSerializationMethod = false;

        public static IdManager IdManager => IdManager.Instance;

        public static TypeManager TypeManager => TypeManager.Instance;

        public static ActorManager ActorManager => ActorManager.Instance;
          
        public static ActorRef GetActor(string actorName)
        {

            return null;
        }

        public static dynamic Get<T>(string name, Container fromContainer) where T: ActorRef
        {
            var actorRef = ActorManager.Instance.GetActorRefByName(name, fromContainer);

            return actorRef;
        }

        public static dynamic Get(string name, Container fromContainer)
        {
            var actorRef = ActorManager.Instance.GetActorRefByName(name, fromContainer);
            return actorRef;
        }

        public static ActorRef GetServiceByName<T>(string name) where T : Actor
        {
            return null;
        }

        public static ActorRef GetAvatarByUid(string uid)
        {
            return null;
        }
    }
}
