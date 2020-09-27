using Fenix.Common.Rpc;
using MessagePack;
using MessagePack.Resolvers;
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

        public static void Init()
        {
#if !CLIENT || !ENABLE_IL2CPP
            var option = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray); 
            MessagePackSerializer.DefaultOptions = option;
#else
            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance).WithCompression(MessagePackCompression.Lz4BlockArray); 
            MessagePackSerializer.DefaultOptions = option;
#endif
        }

        public delegate IMessage MsgDeserialize(byte[] data);

        public delegate IMessage MsgJsonDeserialize(string data);

        //#if ENABLE_IL2CPP
        public volatile static ConcurrentDictionary<Type, MsgDeserialize> _msgCache = new ConcurrentDictionary<Type, MsgDeserialize>();

        public volatile static ConcurrentDictionary<Type, MsgJsonDeserialize> _msgJsonCache = new ConcurrentDictionary<Type, MsgJsonDeserialize>();

        //#endif

        public static byte[] Serialize(IMessage msg)
        {
            return msg.Pack();
        }

        public static T Deserialize<T>(byte[] bytes) where T: IMessage
        {
            //Log.Info("D1", typeof(T).Name, MessagePackSerializer.ConvertToJson(bytes));
            //return MessagePackSerializer.Deserialize<T>(bytes);
            return (T)Deserialize(typeof(T), bytes);
        }

        public static IMessage Deserialize(Type type, byte[] bytes)
        {
//#if ENABLE_IL2CPP
            if(_msgCache.ContainsKey(type))
            {
                return _msgCache[type](bytes);
            }
            else
            {
                var d = (MsgDeserialize)Delegate.CreateDelegate(typeof(MsgDeserialize), type.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                _msgCache[type] = d;
                return d(bytes);
            }
//#else
//            //Log.Info("D2", type.Name, MessagePackSerializer.ConvertToJson(bytes));
//            //return (IMessage)type.GetMethod("Deserialize").Invoke(null, new object[] { bytes});
//            return (IMessage)MessagePackSerializer.Deserialize(type, bytes);
//#endif
        }

        public static T DeserializeJson<T>(string json) where T: IMessage
        {
            return (T)DeserializeJson(typeof(T), json);
        }

        public static IMessage DeserializeJson(Type type, string json)
        {
//#if ENABLE_IL2CPP
            if (_msgJsonCache.ContainsKey(type))
            {
                return _msgJsonCache[type](json);
            }
            else
            {
                var d = (MsgJsonDeserialize)Delegate.CreateDelegate(typeof(MsgJsonDeserialize), type.GetMethod("DeserializeJson", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                _msgJsonCache[type] = d;
                return d(json);
            }
//#else
//            var bytes = MessagePackSerializer.ConvertFromJson(json);
//            return Deserialize(type, bytes);
//#endif
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
            try
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
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}