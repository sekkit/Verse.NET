 
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
          
        public static ActorRef GetActorRef(string actorName, Actor fromActor)
        {
            return Global.ActorManager.GetActorRefByName(actorName, fromActor);
        }

        public static T Get<T>(string name, Actor fromActor) where T: ActorRef
        {
            var actorRef = ActorManager.Instance.GetActorRefByName(name, fromActor);

            return (T)actorRef;
        }

        public static ActorRef Get(string name, Actor fromActor)
        {
            var actorRef = ActorManager.Instance.GetActorRefByName(name, fromActor);
            return actorRef;
        }

        public static void Init(Assembly asm)
        {
            Global.TypeManager.ScanAssemblies(new Assembly[] { asm });
        }
    }
}
