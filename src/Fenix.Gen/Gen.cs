using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Fenix
{
    public class Gen
    {
		static Type GetBaseType(Type type)
        {
			if (type.Name == "Object")
				return type;

			if (type.BaseType == null)
				return type;

			if (type.BaseType.Name == "Object")
				return type;

			return GetBaseType(type.BaseType);
        }

		static bool IsHeritedType(Type type, string baseTypeName)
        {
			if(type.Name == baseTypeName)
            {
				return true;
            }

			if (type.Name == "Object" || type.BaseType == null || type.BaseType.Name == "Object")
            {
				return false;
            }

			return IsHeritedType(type.BaseType, baseTypeName);
		}

        public static void Process(Assembly asm, string output)
        {  
			foreach(Type type in asm.GetTypes())
			{
				if (type.IsAbstract) 
					continue;

				if(IsHeritedType(type, "Actor"))
					Console.WriteLine(type.Name);

				//object[] objects = type.GetCustomAttributes(typeof(), true);
				//if (objects.Length == 0)
				//{
				//	continue;
				//}
				//
				//foreach (BaseAttribute baseAttribute in objects)
				//{
				//	this.types.Add(baseAttribute.AttributeType, type);
				//}
			} 
		}
    }
}
