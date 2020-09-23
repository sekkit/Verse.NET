using Fenix.Common;
using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Fenix
{
    public partial class Host
    {
        public T CreateActorLocally<T>(string name) where T: Actor, new()
        {
            return (T)CreateActorLocally(typeof(T), name);
        }

        //protected T CreateActor<T>(string name) where T : Actor
        //{
        //    return (T)CreateActor(typeof(T), name);
        //}

        public Actor CreateActorLocally(Type type, string name)
        {
            if (name == "" || name == null)
            {
                Log.Error("actor_name_invalid", type.Name, name);
                return null;
            }

            var actorId = Global.IdManager.GetActorId(name);
            if (this.actorDic.TryGetValue(actorId, out var a))
                return a;

            var newActor = Actor.Create(type, name);
            Log.Info(string.Format("CreateActor:success {0} {1}", name, newActor.Id));
            this.ActivateActor(newActor);
            return newActor;
        }

        public Actor CreateActorLocally(string typename, string name)
        {
            if (name == "" || name == null)
            {
                Log.Error("actor_name_invalid", typename, name);
                return null;
            }
            var type = Global.TypeManager.Get(typename);
            return CreateActorLocally(type, name);
        }

        protected Actor CreateActorLocally(string typename, byte[] data)
        {
            var type = Global.TypeManager.Get(typename);
            var a = (Actor)RpcUtil.Deserialize(type, data);
            this.ActivateActor(a);
            return a;
        }

        protected Actor ActivateActor(Actor actor)
        {
            this.RegisterGlobalManager(actor);
            actor.OnLoad();
            actorDic[actor.Id] = actor;
            return actor;
        }

        public Actor GetActor(ulong actorId)
        {
            if (this.actorDic.TryGetValue(actorId, out var a))
                return a;
            return null;
        }

        public T GetService<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), name, null, Global.Host);
        }

        public T GetAvatar<T>(string uid) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), uid, null, Global.Host);
        }
        public T GetActorRef<T>(string name) where T : ActorRef
        {
            return (T)Global.GetActorRef(typeof(T), name, null, Global.Host);
        }

#if FENIX_CODEGEN
        public dynamic GetService<T>() where T : ActorRef
        {
            var refTypeName = typeof(T).Name;
            string name = refTypeName.Substring(0, refTypeName.Length - 3);
            return (T)Global.GetActorRef(typeof(T), name, null, Global.Host);
        }
#else
        public T GetService<T>() where T : ActorRef
        {
            var refTypeName = typeof(T).Name;
            string name = refTypeName.Substring(0, refTypeName.Length - 3);
            return (T)Global.GetActorRef(typeof(T), name, null, Global.Host);
        }
#endif
        //public T GetService<T>(string hostName, string ip, int port) where T : ActorRef
        //{
        //    var refTypeName = typeof(T).Name;
        //    string name = refTypeName.Substring(0, refTypeName.Length - 3);
        //    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
        //    return (T)Global.GetActorRefByAddr(typeof(T), ep, hostName, name,  null, Global.Host);
        //}

        public ActorRef GetHost(string hostName, string ip, int port)
        {
            string _ip = ip;
            if (_ip == "0.0.0.0" || _ip == "127.0.0.1")
                _ip = Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(_ip), port);
            return Global.GetActorRefByAddr(typeof(ActorRef), ep, hostName, "", null, Global.Host);
        }

        public ActorRef GetHost(string hostName, string addr)
        {
            string _ip = Basic.ToIP(addr);
            int _port = Basic.ToPort(addr);
            if (_ip == "0.0.0.0" || _ip == "127.0.0.1")
                _ip = Basic.GetLocalIPv4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(_ip), _port);
            return Global.GetActorRefByAddr(typeof(ActorRef), ep, hostName, "", null, Global.Host);
        }

        public ActorRef GetHost(ulong hostId)
        {
            var addr = Global.IdManager.GetHostAddr(hostId);
            if (addr == null || addr == "")
                return null;
            var hostName = Global.IdManager.GetHostName(hostId);
            var ip = addr.Split(':')[0];
            var port = int.Parse(addr.Split(':')[1]);
            return GetHost(hostName, ip, port);
        }  
    }
}
