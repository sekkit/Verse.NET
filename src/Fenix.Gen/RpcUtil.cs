
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Utils
{
    public class RpcUtil
    {
        public static byte[] Serialize(object msg)
        {
            return null;
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            return default(T);
        }

        public static object Deserialize(Type type, byte[] bytes)
        {
            return null;
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

        public static bool IsHeritedType(TypeDefinition type, string baseTypeName)
        {
            if (type.Namespace.StartsWith("UnityEngine"))
                return false;
            if (type.Namespace.StartsWith("UnityEditor"))
                return false;
            if (type.Name == baseTypeName)
            {
                return true;
            }

            if (type.Name == "Object" || type.BaseType == null || type.BaseType.Name == "Object" || 
                type.BaseType.Namespace.StartsWith("UnityEngine") ||
                type.BaseType.Namespace.StartsWith("UnityEditor"))
            {
                return false;
            }
            try
            {
                return IsHeritedType(type.BaseType, baseTypeName);
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static bool IsHeritedType(TypeReference type, string baseTypeName)
        {
            if (type == null)
                return false;
            if (type.Namespace.StartsWith("UnityEngine"))
                return false;
            if (type.Namespace.StartsWith("UnityEditor"))
                return false;
            if (type.Name == baseTypeName)
            {
                return true;
            }

            //if (type.Name == "Object" || type. == null || type.BaseType.Name == "Object" ||
            //    type.BaseType.Namespace.StartsWith("UnityEngine") ||
            //    type.BaseType.Namespace.StartsWith("UnityEditor"))
            //{
            //    return false;
            //}
            try
            {
                return IsHeritedType(type.Resolve().BaseType, baseTypeName);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    } 
}