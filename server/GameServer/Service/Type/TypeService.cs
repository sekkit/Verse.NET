using System;
using System.Collections;
using System.Reflection; 
using Module.Shared;

namespace Service.Message;

public class TypeService : Singleton<TypeService>, ILifecycle
{
    protected DoubleMap<ProtoCode, Type> _reqMap = new();
    protected DoubleMap<ProtoCode, Type> _rspMap = new();

    public Type GetReqType(ProtoCode code)
    {
        return _reqMap.GetValueByKey(code);
    }
    
    public Type GetRspType(ProtoCode code)
    {
        return _rspMap.GetValueByKey(code);
    }
    
    public void Start()
    {
        var types = Assembly.GetAssembly(GetType()).GetTypes();
        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ProtocolAttribute>();
            if (attr != null)
            {
                if (type.Name.EndsWith("Req"))
                {
                    _reqMap.Add(attr.Code, type);
                }
                else if(type.Name.EndsWith("Rsp"))
                {
                    _rspMap.Add(attr.Code, type);
                }
            }
        }
    }

    public void Update()
    {
        
    }

    public void LateUpdate()
    {
        
    }

    public void FrameFinishedUpdate()
    {
        
    }

    public void Destroy()
    {
        
    }
}