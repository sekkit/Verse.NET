using CSRedis;
using Fenix.Common;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Fenix
{
    public class Global
    {
        //public static Global Instance = new Global();

        public static IdManager IdManager => IdManager.Instance;

        public static TypeManager TypeManager => TypeManager.Instance;

        public static ActorRef GetActor<T>() where T: Actor
        {
            var actorName = nameof(T);

        }

        public static ActorRef GetActor(string actorName)
        {

        }
    }
}
