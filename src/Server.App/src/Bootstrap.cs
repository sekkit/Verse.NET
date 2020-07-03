using Fenix;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.App
{
    public class Bootstrap
    {
        public static void Start()
        {
            var c = Container.Create("0.0.0.0", 7777);
            ContainerHelper.Run(c);
        }
    }
}
