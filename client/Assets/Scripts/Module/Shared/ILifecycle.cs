namespace Module.Shared
{
    public interface ILifecycle
    { 
        void Start();

        void Update();
        
        void LateUpdate();

        void FrameFinishedUpdate();

        void Destroy();
    }
}