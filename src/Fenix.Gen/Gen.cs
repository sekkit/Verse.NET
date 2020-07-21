using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Fenix.Common.Utils;

namespace Fenix
{
    public class Gen
    { 
        public static void Process(Assembly asm, string output)
        {  
			foreach(Type type in asm.GetTypes())
			{
				if (type.IsAbstract) 
					continue;

				if(Basic.IsHeritedType(type, "Actor"))
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
