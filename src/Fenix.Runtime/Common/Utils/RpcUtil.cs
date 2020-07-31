using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Utils
{ 
    public class RpcUtil
    {
        public static byte[] Serialize(IMessage msg)
        {
            return msg.Pack(); 
        }

        public static T Deserialize<T>(byte[] bytes)
        { 
            Log.Info(MessagePackSerializer.ConvertToJson(bytes));
            return MessagePackSerializer.Deserialize<T>(bytes);
        }

        public static object Deserialize(Type type, byte[] bytes)
        {
            Log.Info(MessagePackSerializer.ConvertToJson(bytes));
            return MessagePackSerializer.Deserialize(type, bytes);
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