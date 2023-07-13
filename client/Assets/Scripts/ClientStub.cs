

using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using DataModel.Shared.Message;
using DataModel.Shared.Model;
using JetBrains.Annotations;
using MemoryPack;
using Module.Shared;
using Newtonsoft.Json;
using UnityEngine;

public class ClientStub : MonoBehaviour
{
    public static ClientStub Instance;

    public User User;
    
    private ConcurrentDictionary<ProtoCode, Delegate> _ntfMethods { get; set; } = new();
    
    private void Start()
    {
        Instance = this;
        
        DontDestroyOnLoad(this);
        
        var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<ClientApiAttribute>();
            if (attr == null) continue;
            Log.Info($"{method.Name}");
            var methodParams = method.GetParameters();
            if (methodParams.Length == 1)
            {
                Type returnType = method.ReturnType;
                string methodName = method.Name;
                Type inputParam = methodParams[0].ParameterType;

                Type genericFuncType = typeof(Func<,>).MakeGenericType(inputParam, returnType); 
                var del = Delegate.CreateDelegate(genericFuncType, this, method);
                _ntfMethods.TryAdd(attr.Code, del);
            }
            else
            {
                throw new Exception($"illegal ntf method {method.Name}");
            }
        }
    }

    public async void Call(ProtoCode code, Msg param)
    {
        if (_ntfMethods.TryGetValue(code, out var del))
        {
            //var ntfType = ProtocolProvider.Instance.GetNtfType(code); 
            //object param = MemoryPackSerializer.Deserialize(ntfType, data) as Msg;
         
            if (del.Method.ReturnType.IsSubclassOf(typeof(Task)))
            {
                if (del.Method.ReturnType.ContainsGenericParameters)
                {
                    throw new Exception($"ClientApi {del.Method.Name} parameters are invalid");
                }

                Task task = del.DynamicInvoke(param as object) as Task; 
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    // Shared.Log.Error(Environment.StackTrace); 
                }
            }
            else
            {
                del.DynamicInvoke(param as object);
            }
        }
        else
        {
            Log.Error(string.Format("Invalid code {0}", code)); 
        }
    }

    [ClientApi(ProtoCode.ON_TEST)]
    public async Task OnTestNtf(TestNtf ntf)
    {
        Log.Info(ntf.TestMsg);
        //EventBus.Instance.Publish("on_data_changed");
    }

    [ClientApi(ProtoCode.ON_SYNC_FIELD)]
    public async Task OnSyncField(SyncFieldNtf ntf)
    {
        this.User.MergeFrom(ntf.Key, ntf.Value);
    }

    public void SetUser(User initUser)
    {
        this.User = initUser;
        //JsonConvert.DeserializeObject<User>(initUser.Pack(), ref this.User);
    }
} 