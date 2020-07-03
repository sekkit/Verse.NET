using CSRedis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;



namespace Fenix.Global
{
    public class IdManager
    {
        public static IdManager Instance = new IdManager();

        protected ConcurrentDictionary<uint, string> ContainerAddrMap = new ConcurrentDictionary<uint, string>();

        protected ConcurrentDictionary<uint, uint> ActorContainerMap = new ConcurrentDictionary<uint, uint>();

        protected IdManager()
        {
             
        }

        ~IdManager()
        {
             
        }

        public void RegisterContainer(uint containerId, string address)
        {
            ContainerAddrMap[containerId] = address; 
            Global.Instance.RegisterContainer(containerId, address);
        }

        public void RegisterActor(uint actorId, uint containerId)
        {
            ActorContainerMap[actorId] = containerId;
            Global.Instance.RegisterActor(actorId, containerId);
        }
    }
}
