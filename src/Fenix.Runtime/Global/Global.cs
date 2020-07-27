
using Fenix.Config;
using System;
using System.Net;
using System.Reflection; 

namespace Fenix
{
    public static partial class Global
    {
        public static Host Host;

        public static RuntimeConfig Config = RuntimeConfig.MakeDefaultConfig();

#if !CLIENT
        public static DbManager DbManager = new DbManager();
#endif

        public static IdManagerSimple IdManager = new IdManagerSimple();

        public static TypeManager TypeManager = new TypeManager();

        public static ActorManager ActorManager = new ActorManager();

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
