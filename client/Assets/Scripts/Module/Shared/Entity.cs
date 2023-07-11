using System;
using System.Collections;
using System.Collections.Concurrent;
using DataModel.Shared.Message;
using MemoryPack; 

namespace Module.Shared
{
    public partial class Entity : ILifecycle, IDisposable
    {
        public string Uid { get; private set; }
        
        public void SetUid(string uid)
        {
            Uid = uid;
            
            this.Start();
        }
        
        private ConcurrentDictionary<Type, EntityModule> _modules { get; set; } = new();

        private IChannel _channel { get; set; }

        public void AddModule<T>() where T: EntityModule
        {
            var inst = Activator.CreateInstance<T>();
            if (!_modules.ContainsKey(typeof(T)))
            {
                _modules.TryAdd(typeof(T), inst);
                inst.Attach(this);
            }
        }

        public void Attach(IChannel ch) => _channel = ch;

        public bool IsAttached => _channel != null;

        public bool IsEnabled => IsAttached && Uid != null;

        public T Get<T>() where T : EntityModule
        {
            if (!_modules.ContainsKey(typeof(T)))
            {
                throw new Exception(string.Format("Module {0} not found", typeof(T)));
            }

            _modules.TryGetValue(typeof(T), out var m);
            return m as T;
        }

        public void Start()
        { 
            foreach (var module in _modules.Values)
            {
                module.Start();
            }
        }

        public void Update()
        {
            foreach (var module in _modules.Values)
            {
                module.Update();
            }
        }

        public void LateUpdate()
        {
            foreach (var module in _modules.Values)
            {
                module.LateUpdate();
            }
        }

        public void FrameFinishedUpdate()
        {
            foreach (var module in _modules.Values)
            {
                module.FrameFinishedUpdate();
            }
        }

        public void Destroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            _channel = null;
            
            foreach(var m in _modules.Values)
            {
                m.Destroy();
            }
        }
    }
}