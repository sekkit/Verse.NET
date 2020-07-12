using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class TypeManager
    { 
        protected TypeManager()
        {
            //scan all the assemblies
            this.scanAssemblies();
        }

        public static TypeManager Instance = new TypeManager();

        protected ConcurrentDictionary<string, Type> mTypeDic = new ConcurrentDictionary<string, Type>();

        protected ConcurrentDictionary<uint, Type> mMessageTypeDic = new ConcurrentDictionary<uint, Type>();

        protected ConcurrentDictionary<uint, string> mId2TypenameDic = new ConcurrentDictionary<uint, string>();
         
        public void scanAssemblies()
        {

        }

        public void Register(string typeName, Type type)
        {
            mTypeDic[typeName] = type;
        }

        public void RegisterMessage(uint protocolId, Type type)
        {
            mMessageTypeDic[protocolId] = type;
        }

        public void RegisterActorType(uint actorId, Type type)
        {
            mId2TypenameDic[actorId] = type.Name;
            if(!mTypeDic.ContainsKey(type.Name))
                mTypeDic[type.Name] = type;
        }

        public Type Get(string typeName)
        {
            return mTypeDic[typeName];
        }

        public Type GetMessageType(uint protocolId)
        {
            return mMessageTypeDic[protocolId];
        }
    }
}
