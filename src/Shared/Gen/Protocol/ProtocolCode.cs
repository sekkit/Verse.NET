
//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
namespace Shared
{
    public partial class ProtocolCode
    {
        public void Validate()
        {
            Dictionary<uint, string> dic = new Dictionary<uint, string>();
            foreach(var f in this.GetType().GetFields())
            {
                uint key = (uint)f.GetValue(this);
                if(dic.ContainsKey(key))
                {
                    Console.WriteLine(string.Format("Duplicated protocol {0} and {1}", dic[key], f.Name));
                }
                dic[key] = f.Name;
                Console.WriteLine(string.Format("    ProtocolCode({0} = {1})", f.Name, f.GetValue(this)));
            }

            Console.WriteLine(dic.Count() == this.GetType().GetFields().Count()?"Validation Passed": "Duplicated Protocol");
        }
    }
}

