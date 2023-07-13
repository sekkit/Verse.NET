using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using DataModel.Shared.Message;
using DataModel.Shared.Model;
using MemoryPack; 

namespace Module.Shared
{
    public partial class Entity : ILifecycle, IDisposable
    {    
        public Entity()
        {
        }

        private ConcurrentDictionary<ProtoCode, Delegate> _rpcMethods { get; set; } = new();
        
        public string Uid { get; private set; }

        public User User;
        
        public void SetUid(string uid)
        {
            Uid = uid;
            
            Start();
        }
        
        private ConcurrentDictionary<Type, EntityModule> _modules { get; set; } = new();

        private IChannel _channel { get; set; }

        public void AddModule<T>() where T: EntityModule
        {
            if (!_modules.ContainsKey(typeof(T)))
            {
                var inst = Activator.CreateInstance<T>();
                _modules.TryAdd(typeof(T), inst);
                inst.Attach(this);
                
                var methods = inst.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<ServerApiAttribute>();
                    if (attr == null) continue;
                    Log.Info($"{method.Name}");
                    var methodParams = method.GetParameters();
                    if (methodParams.Length == 1)
                    {
                        Type returnType = method.ReturnType; 
                        Type inputParam = methodParams[0].ParameterType;

                        Type genericFuncType = typeof(Func<,>).MakeGenericType(inputParam, returnType); 
                        var del = Delegate.CreateDelegate(genericFuncType, inst, method);
                        _rpcMethods.TryAdd(attr.Code, del);
                    }
                    else
                    {
                        throw new Exception($"illegal rpc method {method.Name}");
                    }
                }
            }
        }

        public void Attach(IChannel ch)
        {
            _channel = ch; 
        } 

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

        public ConcurrentDictionary<ProtoCode, Delegate> GetRpcMethods() => _rpcMethods;

        public IChannel GetChannel() => _channel;
    }
}