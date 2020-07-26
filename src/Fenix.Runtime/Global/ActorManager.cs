using Fenix.Common.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Fenix
{
    public class ActorManager
    {
        public static ActorManager Instance = new ActorManager();

        //客户端与服务端，其实就是Avatar之间的单点通信
        //如果是客户端，则取不到名字和类型信息
        //所以需要特殊处理
        //客户端连接服务器之前，需要知道入口的连接在哪
        //客户端连接上服务器后，会创建ClientAvatar，然后ClientAvatar会得到ServerAvatar信息，以及向ServerAvatar注册
        //所以客户端是单点连接，不需要额外的信息

        public ActorRef GetActorRefByName(Type refType, string toActorName, Actor fromActor, Host fromHost)
        { 
            uint toActorId = Global.IdManager.GetActorId(toActorName);

            //if (toActorId == 0)
            //    return null;

            //即使找不到目标Actor，仍然创建
            // 
#if CLIENT
            if (toActorId == 0)
            {
                toActorId = Basic.GenID32FromName(toActorName);
            }
#endif
            var rt = Global.TypeManager.GetRefType(refType);
            bool isClient = rt == RefType.CLIENT;

            var toHostId = Global.IdManager.GetHostIdByActorId(toActorId, isClient);  
            return ActorRef.Create(toHostId, toActorId, refType, fromActor, fromHost, isClient);
        }

        public ActorRef GetActorRefByName(Type refType, string toHostName, string toActorName, Actor fromActor, Host fromHost)
        {
            uint toActorId = Global.IdManager.GetActorId(toActorName);

            //if (toActorId == 0)
            //    return null;

            //即使找不到目标Actor，仍然创建
            // 
#if CLIENT
            if (toActorId == 0) 
                toActorId = Basic.GenID32FromName(toActorName);
#endif
            var rt = Global.TypeManager.GetRefType(refType);
            bool isClient = rt == RefType.CLIENT;
            var toHostId = Global.IdManager.GetHostIdByActorId(toActorId, isClient);

#if CLIENT
            if (toHostId == 0)
                toHostId = Basic.GenID32FromName(toHostName);
#endif

            return ActorRef.Create(toHostId, toActorId, refType, fromActor, fromHost, isClient);
        }

        public ActorRef GetActorRefByAddress(Type refType, IPEndPoint toPeerEP, string toHostName, string toActorName, Actor fromActor, Host fromHost)
        {
            uint toActorId = Global.IdManager.GetActorId(toActorName);
            //if (toActorId == 0)
            //    return null;

            //即使找不到目标Actor，仍然创建
            // 
#if CLIENT
            if (toActorId == 0 && toActorName != "")
                toActorId = Basic.GenID32FromName(toActorName);
#endif

            var rt = Global.TypeManager.GetRefType(refType);
            bool isClient = rt == RefType.CLIENT;

            var toHostId = Global.IdManager.GetHostIdByActorId(toActorId, isClient);
            if (toHostId == 0)
                toHostId = Basic.GenID32FromName(toHostName); 

            return ActorRef.Create(toHostId, toActorId, refType, fromActor, fromHost, isClient, toPeerEP);
        }

        /*远程创建actor*/
        //public void CreateActor()
        //{
        //    
        //}
    }
}
