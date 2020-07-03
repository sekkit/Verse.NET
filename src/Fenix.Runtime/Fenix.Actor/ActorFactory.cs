using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class ActorFactory
    {
        public static Actor CreateActor<T>(ActorConfig actorConfig) where T: ActorLogic, new()
        {
            var actor = Actor.Create<T>();
            return actor;
        }
    }
}
