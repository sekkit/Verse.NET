using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Fenix
{
    public static class GenUtil
    {
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
                return true; 

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
            catch (Exception ex)
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
                return true; 

            try
            {
                return IsHeritedType(type.Resolve().BaseType, baseTypeName);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static dynamic GetAttribute<T>(Type type, bool noInherit = false) where T : Attribute
        {
            var attrs = type.GetCustomAttributes(!noInherit);
            return attrs.Where(m => (m.GetType().Name == typeof(T).Name)).FirstOrDefault();
        }

        public static List<dynamic> GetAttributes<T>(Type type, bool noInherit = false) where T : Attribute
        {
            var attrs = type.GetCustomAttributes(!noInherit);
            return attrs.Where(m => (m.GetType().Name == typeof(T).Name)).ToList();
        }

        public static dynamic GetAttribute<T>(MethodInfo methodInfo, bool noInherit = false) where T : Attribute
        {
            var attrs = methodInfo.GetCustomAttributes(!noInherit);
            return attrs.Where(m => (m.GetType().Name == typeof(T).Name)).FirstOrDefault();
        }

        public static dynamic GetAttribute<T>(TypeDefinition type, bool noInherit = false) where T : Attribute
        {
            var attrs = type.CustomAttributes;
            var attr = attrs.Where(m => (m.AttributeType.Name == typeof(T).Name)).FirstOrDefault();
            if (attr == null && !noInherit)
            {
                if (type.BaseType != null)
                {
                    var bt = type.BaseType.Resolve();
                    return GetAttribute<T>(bt, noInherit);
                }
            }
            return attr;
        }

        public static List<CustomAttribute> GetAttributes<T>(TypeDefinition type, bool noInherit = false) where T : Attribute
        {
            var attrs = type.CustomAttributes;
            var attrList = attrs.Where(m => (m.AttributeType.Name == typeof(T).Name)).ToList();
            if (attrList.Count == 0 && !noInherit)
            {
                if (type.BaseType != null)
                {
                    var bt = type.BaseType.Resolve();
                    return GetAttributes<T>(bt, noInherit);
                }
            }
            return attrList;
        }

        public static dynamic GetAttribute<T>(MethodDefinition methodInfo, bool noInherit = false) where T : Attribute
        {
            var attrs = methodInfo.CustomAttributes;
            var attr = attrs.Where(m => (m.AttributeType.Name == typeof(T).Name)).FirstOrDefault();
            //if (attr == null && !noInherit)
            //{
            //    var bm = methodInfo.GetBaseMethod();
            //    if (bm != null)
            //        return GenUtil.GetAttribute<T>(bm, noInherit);
            //}
            return attr;
        }

        public static string[] SplitCamelCase(string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }

        public static string NameToApi(string name)
        {
            var parts = SplitCamelCase(name);
            for (int i = 0; i < parts.Length; ++i)
                parts[i] = parts[i].ToLower();
            return string.Join("_", parts);
        }

        public static string NameToProtoCode(string prefix, string ns, string entityName, string apiName)
        {
            var parts = SplitCamelCase(apiName);
            for (int i = 0; i < parts.Length; ++i)
                parts[i] = parts[i].ToUpper();
            return string.Format("{0}__{1}__{2}__{3}", prefix == null ? "" : prefix, ns == null ? "" : ns.Replace(".", "").ToUpper(), entityName.ToUpper(), string.Join("_", parts)).ToUpper();
        }

        public static string NameToProtoCode(string prefix, string apiName)
        {
            var parts = SplitCamelCase(apiName);
            for (int i = 0; i < parts.Length; ++i)
                parts[i] = parts[i].ToUpper();
            return prefix == null ? string.Format("{0}", string.Join("_", parts)) : string.Format("{0}{1}", prefix, apiName).ToUpper();
        }
    }
}
