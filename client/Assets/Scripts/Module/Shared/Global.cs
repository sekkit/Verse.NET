using System;
using System.Collections.Generic;

namespace Module.Shared
{

    public static class Global
    {
        private static readonly Dictionary<Type, ISingleton> _singletonTypes = new();

        private static readonly Stack<ISingleton> _singletons = new();

        public static T AddSingleton<T>() where T : Singleton<T>, new()
        {
            T singleton = new T();
            AddSingleton(singleton);
            return singleton;
        }

        public static void AddSingleton(ISingleton singleton)
        {
            Type singletonType = singleton.GetType();
            if (_singletonTypes.ContainsKey(singletonType))
            {
                throw new Exception($"already exist singleton: {singletonType.Name}");
            }

            _singletonTypes.Add(singletonType, singleton);
            _singletons.Push(singleton);

            singleton.Register();
        }

        public static void Close()
        {
            while (_singletons.Count > 0)
            {
                ISingleton iSingleton = _singletons.Pop();
                iSingleton.Destroy();
            }

            _singletonTypes.Clear();
        }

        public static void Start()
        {
            foreach (var singleton in _singletonTypes.Values)
            {
                if (singleton is ILifecycle svc)
                {
                    svc.Start();
                }
            }
        }
        
        public static void Update()
        {
            foreach (var singleton in _singletonTypes.Values)
            {
                if (singleton is ILifecycle svc)
                {
                    svc.Update();
                }
            }
        }
        
        public static void LateUpdate()
        {
            foreach (var singleton in _singletonTypes.Values)
            {
                if (singleton is ILifecycle svc)
                {
                    svc.LateUpdate();
                }
            }
        }
        
        public static void FrameFinishedUpdate()
        {
            foreach (var singleton in _singletonTypes.Values)
            {
                if (singleton is ILifecycle svc)
                {
                    svc.FrameFinishedUpdate();
                }
            }
        }
    }
}