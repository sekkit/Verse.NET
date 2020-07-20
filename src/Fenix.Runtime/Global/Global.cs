 
using System.Reflection; 

namespace Fenix
{
    public partial class Global
    {
        public static bool IsServer = true;

        public static bool NativeSerializationMethod = false;

        public static IdManager IdManager => IdManager.Instance;

        public static TypeManager TypeManager => TypeManager.Instance;

        public static ActorManager ActorManager => ActorManager.Instance;
          
        public static ActorRef GetActorRef(string actorName)
        {
            return Global.ActorManager.GetActorRefByName(actorName, Container.Instance);
        }

        public static T Get<T>(string name, Container fromContainer) where T: ActorRef
        {
            var actorRef = ActorManager.Instance.GetActorRefByName(name, fromContainer);

            return (T)actorRef;
        }

        public static ActorRef Get(string name, Container fromContainer)
        {
            var actorRef = ActorManager.Instance.GetActorRefByName(name, fromContainer);
            return actorRef;
        }

        public static void Init(Assembly asm)
        {
            Global.TypeManager.ScanAssemblies(new Assembly[] { asm });
        }
    }
}
