using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class ActorLogic
    {
        protected Actor actor { get; set; }

        public void setActor(Actor _actor)
        {
            this.actor = _actor;
        }

        public virtual void onActive() { }

        public virtual void onDeActive() { }

        public virtual void onDestroy() { }
    }
}
