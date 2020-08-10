using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Fenix.Common.Utils
{ 
    public static class RpcUtil
    {
        //Optimization one
        //cache IMessage instance

        public static MessagePackSerializerOptions lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

        public delegate IMessage MsgDeserialize(byte[] data);

#if ENABLE_IL2CPP
        public volatile static ConcurrentDictionary<Type, MsgDeserialize> _msgCache = new ConcurrentDictionary<Type, MsgDeserialize>();
#endif

        public static byte[] Serialize(IMessage msg)
        {
            return msg.Pack();
        }

        public static T Deserialize<T>(byte[] bytes)
        { 
            //Log.Info("D1", typeof(T).Name, MessagePackSerializer.ConvertToJson(bytes));
            return MessagePackSerializer.Deserialize<T>(bytes, RpcUtil.lz4Options);
        }

        public static IMessage Deserialize(Type type, byte[] bytes)
        {
#if ENABLE_IL2CPP
            if(_msgCache.ContainsKey(type))
            {
                return _msgCache[type](bytes);
            }
            else
            {
                var d = (MsgDeserialize)Delegate.CreateDelegate(typeof(MsgDeserialize), type.GetMethod("Deserialize"));
                _msgCache[type] = d;
                return d(bytes);
            }
#else
            //Log.Info("D2", type.Name, MessagePackSerializer.ConvertToJson(bytes));
            //return (IMessage)type.GetMethod("Deserialize").Invoke(null, new object[] { bytes});
            return (IMessage)MessagePackSerializer.Deserialize(type, bytes, RpcUtil.lz4Options);
#endif
        }

        public static Type GetBaseType(Type type)
        {
            if (type.Name == "Object")
                return type;

            if (type.BaseType == null)
                return type;

            if (type.BaseType.Name == "Object")
                return type;

            return GetBaseType(type.BaseType);
        }

        public static bool IsHeritedType(Type type, string baseTypeName)
        {
            if (type.Name == baseTypeName)
            {
                return true;
            }

            if (type.Name == "Object" || type.BaseType == null || type.BaseType.Name == "Object")
            {
                return false;
            }

            return IsHeritedType(type.BaseType, baseTypeName);
        }
    }
}