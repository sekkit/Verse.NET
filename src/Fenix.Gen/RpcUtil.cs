
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
    } 
}