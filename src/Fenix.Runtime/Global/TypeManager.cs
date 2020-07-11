using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class TypeManager
    {

        public static TypeManager Instance = new TypeManager();

        protected ConcurrentDictionary<string, Type> mTypeDic = new ConcurrentDictionary<string, Type>();

        public void Register(string typeName, Type type)
        {
            mTypeDic[typeName] = type;
        }

        public Type Get(string typeName)
        {
            return mTypeDic[typeName];
        }
    }
}
