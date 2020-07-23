
using System;
using System.Net;
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

#if !CLIENT
        public static DbManager DbManager => DbManager.Instance;
#endif
          
        public static ActorRef GetActorRef(Type refType, string actorName, Actor fromActor, Host fromHost)
        {
            return Global.ActorManager.GetActorRefByName(refType, actorName, fromActor, fromHost);
        }

        public static ActorRef GetActorRefByAddr(Type refType, IPEndPoint toPeerEP, string toHostName, string toActorName, Actor fromActor, Host fromHost)
        {
            return Global.ActorManager.GetActorRefByAddress(refType, toPeerEP, toHostName, toActorName, fromActor, fromHost);
        } 

        public static void Init(Assembly[] asmList)
        {
            Global.TypeManager.ScanAssemblies(asmList);
        }
    }
}
