using System;

namespace Fenix
{
    public class Actor
    {
        public uint Id { get; set; }

        public uint ContainerId { get; set; }

        public string UniqueName { get; set; }

        protected ActorLogic Logic { get; set; }

        protected Actor()
        {
            
        }

        ~Actor()
        {

        }

        public static Actor Create(ActorLogic logic)
        {
            return new Actor();
        }

        public static Actor Create<T>() where T: ActorLogic, new()
        {
            return Actor.Create(new T());
        }
        
        public static Actor Create(Type type)
        {
            return Actor.Create((ActorLogic)Activator.CreateInstance(type)); 
        }
    }
}
