using System;
using System.Reflection;
using Module.Shared;

namespace Module.Shared
{
    public class ProtocolProvider : Singleton<ProtocolProvider>, ILifecycle
    {
        protected DoubleMap<ProtoCode, Type> _reqMap = new();
        protected DoubleMap<ProtoCode, Type> _rspMap = new();
        protected DoubleMap<ProtoCode, Type> _ntfMap = new();
        
        public Type GetReqType(ProtoCode code) => _reqMap.ContainsKey(code)?_reqMap.GetValueByKey(code):null; 

        public Type GetRspType(ProtoCode code) => _rspMap.ContainsKey(code)?_rspMap.GetValueByKey(code):null;

        public Type GetNtfType(ProtoCode code) => _ntfMap.ContainsKey(code)?_ntfMap.GetValueByKey(code):null;
        
        public ProtoCode GetCodeByReqType(Type type) => _reqMap.GetKeyByValue(type);

        public ProtoCode GetCodeByRspType(Type type) => _rspMap.GetKeyByValue(type);

        public ProtoCode GetCodeByNtfType(Type type) => _ntfMap.GetKeyByValue(type);
        
        public ProtoCode GetCodeByType(Type type) => type.GetCustomAttribute<ProtocolAttribute>().Code;

        public void Start()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    var attr = type.GetCustomAttribute<ProtocolAttribute>();
                    if (attr != null)
                    {
                        if (type.Name.EndsWith("Req"))
                        {
                            _reqMap.Add(attr.Code, type);
                        }
                        else if (type.Name.EndsWith("Rsp"))
                        {
                            _rspMap.Add(attr.Code, type);
                        }
                        else if (type.Name.EndsWith("Ntf"))
                        {
                            _ntfMap.Add(attr.Code, type);
                        }
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
}