using System;
using System.Reflection;
using Module.Shared;

namespace Module.Shared
{
    public class TypeProvider : Singleton<TypeProvider>, ILifecycle
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
        
        public ProtoCode GetCodeByReqType(Type type)
        {
            return _reqMap.GetKeyByValue(type);
        }

        public ProtoCode GetCodeByRspType(Type type)
        {
            return _rspMap.GetKeyByValue(type);
        }
        
        public ProtoCode GetCodeByType(Type type)
        {
            return type.GetCustomAttribute<ProtocolAttribute>().Code;
        }

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