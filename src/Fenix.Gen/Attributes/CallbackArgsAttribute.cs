using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix.Common.Attributes
{
    public class CallbackArgsAttribute : Attribute
    {
        public string[] Names;

        public CallbackArgsAttribute(params string[] args)
        {
            this.Names = args;
        }
    }
}
