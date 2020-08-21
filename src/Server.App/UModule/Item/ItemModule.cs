using System;
using Fenix;
using Fenix.Common.Attributes;

namespace Server.UModule
{
    public class ItemModule : ActorModule<Avatar>
    {
        public ItemModule(Avatar self): base(self) { }


        [ServerApi]
        public void TestItemApi(Action callback)
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
