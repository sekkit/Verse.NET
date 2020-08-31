using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.UModule
{
    public class MatchModule : ActorModule<Avatar>
    {
        public MatchModule(Avatar self):base(self) { }
        
        [ServerApi]
        public void BeginTest()
        {

        }

        [ServerApi]
        public void PlayerReady()
        {

        }

        [ServerApi]
        public void PlayerNotReady()
        {

        }

        public override void onClientDisable()
        {

        }

        public override void onClientEnable()
        {

        }

        public override void onDestory()
        {

        }

        public override void onLoad()
        {

        }

        public override void onRestore()
        {

        }

        public override void onUpdate()
        {

        }
    }
}
