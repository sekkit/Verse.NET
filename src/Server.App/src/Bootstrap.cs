using Fenix;
using GModule.Match;
using System;
using System.Collections.Generic;
using System.Text;
using UModule;

namespace Server.App
{
    public class Bootstrap
    {
        public static void Start()
        {
            Global.Init(typeof(Bootstrap).Assembly);

            var c = Container.Create(null, 7777);
            c.CreateActor<MatchService>("MatchService");
            c.CreateActor<Avatar>("12345");
            ContainerHelper.Run(c);
        }
    }
}
