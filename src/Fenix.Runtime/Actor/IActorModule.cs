using Fenix; 
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public abstract class ActorModule<T> : IActor where T: Actor
    {
        protected T self;
        
        public ActorModule(T self)
        {
            this.self = self;
        }
    }

    public abstract class IActor
    {
        public abstract void onLoad();

        public abstract void onClientEnable();

        public abstract void onClientDisable();

        public abstract void onRestore();

        public abstract void onUpdate();

        public abstract void onDestory();
    }
}
