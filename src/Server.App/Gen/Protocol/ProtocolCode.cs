
//AUTOGEN, do not modify it!

using Fenix.Common;
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
                    Log.Info(string.Format("Duplicated protocol {0} and {1}", dic[key], f.Name));
                }
                dic[key] = f.Name;
                Log.Info(string.Format("    ProtocolCode({0} = {1})", f.Name, f.GetValue(this)));
            }

            Log.Info(dic.Count() == this.GetType().GetFields().Count()?"Validation Passed": "Duplicated Protocol");
        }
    }
}

