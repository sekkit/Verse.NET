
using Fenix.Common.Utils;
using Fenix.Config; 
#if !CLIENT
using Server.Config;
#endif
using System;
using System.Net;
using System.Reflection; 

namespace Fenix
{
    public static partial class Global
    {
        public static Host Host;
        
        static RuntimeConfig _cfg { get; set; }
#if !CLIENT
        public static RuntimeConfig Config => _cfg==null?_cfg = RuntimeConfig.MakeDefaultServerConfig(): _cfg;
#else
        public static RuntimeConfig Config => _cfg == null ? _cfg = RuntimeConfig.MakeDefaultClientConfig() : _cfg;
#endif

#if !CLIENT
        public static DbManager DbManager = new DbManager();
#endif

        public static IdManager IdManager = new IdManager();

        public static TypeManager TypeManager = new TypeManager();

        public static NetManager NetManager = new NetManager();

        public static ActorRef GetActorRef(Type refType, string actorName, Actor fromActor, Host fromHost)
        {
            return GetActorRefByName(refType, actorName, fromActor, fromHost);
        }

        public static ActorRef GetActorRefByAddr(Type refType, IPEndPoint toPeerEP, string toHostName, string toActorName, Actor fromActor, Host fromHost)
        {
            return GetActorRefByAddress(refType, toPeerEP, toHostName, toActorName, fromActor, fromHost);
        }

        //客户端与服务端，其实就是Avatar之间的单点通信
        //如果是客户端，则取不到名字和类型信息
        //所以需要特殊处理
        //客户端连接服务器之前，需要知道入口的连接在哪
        //客户端连接上服务器后，会创建ClientAvatar，然后ClientAvatar会得到ServerAvatar信息，以及向ServerAvatar注册
        //所以客户端是单点连接，不需要额外的信息

        public static ActorRef GetActorRefByName(Type refType, string toActorName, Actor fromActor, Host fromHost)
        {
            ulong toActorId = Global.IdManager.GetActorId(toActorName);

            //if (toActorId == 0)
            //    return null;

            //即使找不到目标Actor，仍然创建
#if CLIENT
            if (toActorId == 0)
            {
                toActorId = Basic.GenID64FromName(toActorName);
            }
#endif
            var rt = Global.TypeManager.GetRefType(refType);
            bool isClient = rt == RefType.CLIENT;

            var toHostId = Global.IdManager.GetHostIdByActorId(toActorId, isClient);
            return ActorRef.Create(toHostId, toActorId, refType, fromActor, fromHost, isClient);
        }

        public static ActorRef GetActorRefByName(Type refType, string toHostName, string toActorName, Actor fromActor, Host fromHost)
        {
            ulong toActorId = Global.IdManager.GetActorId(toActorName);

            //if (toActorId == 0)
            //    return null;

            //即使找不到目标Actor，仍然创建
            // 
#if CLIENT
            if (toActorId == 0)
                toActorId = Basic.GenID64FromName(toActorName);
#endif
            var rt = Global.TypeManager.GetRefType(refType);
            bool isClient = rt == RefType.CLIENT;
            var toHostId = Global.IdManager.GetHostIdByActorId(toActorId, isClient);

#if CLIENT
            if (toHostId == 0)
                toHostId = Basic.GenID64FromName(toHostName);
#endif

            return ActorRef.Create(toHostId, toActorId, refType, fromActor, fromHost, isClient);
        }

        public static ActorRef GetActorRefByAddress(Type refType, IPEndPoint toPeerEP, string toHostName, string toActorName, Actor fromActor, Host fromHost)
        {
            ulong toActorId = Global.IdManager.GetActorId(toActorName);
            //if (toActorId == 0)
            //    return null;

            //即使找不到目标Actor，仍然创建
            // 
#if CLIENT
            if (toActorId == 0 && toActorName != "")
                toActorId = Basic.GenID64FromName(toActorName);
#endif

            var rt = Global.TypeManager.GetRefType(refType);
            bool isClient = rt == RefType.CLIENT;

            var toHostId = Global.IdManager.GetHostIdByActorId(toActorId, isClient);
            if (toHostId == 0)
                toHostId = Basic.GenID64FromName(toHostName);

            return ActorRef.Create(toHostId, toActorId, refType, fromActor, fromHost, isClient, toPeerEP);
        }

        public static void Init(RuntimeConfig cfg, Assembly[] asmList)
        {
            RpcUtil.Init(); 

#if !CLIENT
            CacheConfig.Init();
#endif
            _cfg = cfg;

            Global.TypeManager.ScanAssemblies(asmList);
            Global.TypeManager.ScanAssemblies(new Assembly[] { typeof(Global).Assembly });
        }

        public static void DeInit()
        {
#if !CLIENT
            Global.DbManager.Destroy();
#endif
        }
    }
}
