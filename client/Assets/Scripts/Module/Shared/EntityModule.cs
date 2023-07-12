namespace Module.Shared
{
    public abstract class EntityModule : ILifecycle
    {
        private bool _started = false; 
        
        protected Entity self { get; set; }

        public void Attach(Entity parent) => self = parent;
        
        public abstract void Start();
        public abstract void Update();
        public abstract void LateUpdate();
        public abstract void FrameFinishedUpdate(); 
        public abstract void Destroy();
    }
}