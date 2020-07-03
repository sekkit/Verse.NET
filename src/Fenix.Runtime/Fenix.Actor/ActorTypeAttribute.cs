using Fenix.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class ActorTypeAttribute: Attribute
    {
        public string ActorTypeName { get; set; }

        public ActorTypeAttribute(Type type)
        {
            this.ActorTypeName = type.GetType().Name;
            Global.Global.Instance.RegisterActorType(type); 
        }
    }
}
