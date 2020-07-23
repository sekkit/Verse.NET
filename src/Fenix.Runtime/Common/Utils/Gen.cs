using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using DotNetty.Codecs;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils; 

namespace Fenix
{
    public class Gen
    { 
        public static void Autogen(Assembly asm, string sharedPath, string clientPath, string serverPath)
        {
            List<Type> actorTypes = new List<Type>();
            foreach (Type type in asm.GetTypes())
            {
                if (type.IsAbstract)
                    continue;

                if (!RpcUtil.IsHeritedType(type, "Actor"))
                    continue; 

                actorTypes.Add(type);
            }
              
            GenProtoCode(actorTypes, sharedPath, clientPath, serverPath); 
             
            foreach(var type in actorTypes)
                GenFromActorType(type, sharedPath, clientPath, serverPath);
        }  

        static string[] SplitCamelCase(string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }

        static string NameToApi(string name)
        {
            var parts = SplitCamelCase(name);
            for (int i=0;i< parts.Length;++i)
                parts[i] = parts[i].ToLower();
            return string.Join("_", parts); 
        }

        static string NameToProtoCode(string name)
        {
            var parts = SplitCamelCase(name);
            for (int i = 0; i < parts.Length; ++i)
                parts[i] = parts[i].ToUpper();
            return string.Join("_", parts);
        }

        static string ParseGenericDecl(Type genericType)
        {
            string decl = genericType.Name.Split('`')[0] + "<";

            foreach (var argType in genericType.GetGenericArguments())
            {
                if (!argType.IsGenericType)
                    decl += argType.Name + ", ";
                else
                    decl += ParseGenericDecl(argType);
            }

            if (decl.EndsWith(", "))
                decl = decl.Substring(0, decl.Length - ", ".Length);
            decl += ">";
            return decl;
        }

        static string ParseArgsDecl(ParameterInfo[] paramInfos)
        {
            List<string> paramList = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.HasDefaultValue)
                    paramList.Add(string.Format("{0} {1}={2}", ParseTypeName(p.ParameterType), p.Name, p.DefaultValue));
                else 
                    paramList.Add(string.Format("{0} {1}", ParseTypeName(p.ParameterType), p.Name));  
            }

            return string.Join(", ", paramList);
        }

        static string ParseTypeName(Type type)
        {
            string decl = string.Empty;
            if (type.IsGenericType)
                decl = ParseGenericDecl(type);
            else
                decl = type.Name;
            return decl;
        }

        static string ParseArgs(ParameterInfo[] paramInfos, string tarInstanceName="", string exclude="")
        {
            List<string> paramList = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.Name == exclude)
                    continue;
                if (p.HasDefaultValue)
                    paramList.Add(string.Format("{0}{1}", tarInstanceName, p.Name));
                else 
                    paramList.Add(string.Format("{0}{1}", tarInstanceName, p.Name));   
            }

            return string.Join(", ", paramList);
        }

        static string ParseArgsMsgAssign(ParameterInfo[] paramInfos, string prefix, string tarInstanceName="")
        {
            List<string> paramList = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.Name == "callback")
                    continue; 
                if (p.HasDefaultValue)
                    paramList.Add(string.Format("{0}{1}{2}={3}", prefix, tarInstanceName, p.Name, p.Name));
                else 
                    paramList.Add(string.Format("{0}{1}{2}={3}", prefix, tarInstanceName, p.Name, p.Name));  
            }

            return string.Join(",\n", paramList);
        }

        static string ParseArgsMsgAssign(Type[] types, string[] names, string prefix, string tarInstanceName = "")
        {
            List<string> lines = new List<string>();
            for (var i = 0; i < types.Length; ++i)
            {
                var t = types[i];
                int position = i; 
                string pType = ParseTypeName(t);
                var attr = t.GetCustomAttribute(typeof(RpcArgAttribute));
                string pName;
                if (attr != null) 
                     pName = ((RpcArgAttribute)attr).Name; 
                else
                    pName = names.Length > position ? names[position] : "arg"+position.ToString(); 
                lines.Add(string.Format("{0}{1}{2}={3};", prefix, tarInstanceName, pName, pName));
            }

            return string.Join("\n", lines);
        }

        static string ParseMessageFields(ParameterInfo[] paramInfos, string prefix)
        {
            List<string> lines = new List<string>();
            foreach(var p in paramInfos)
            {
                if (p.Name == "callback")
                    continue;
                int position = p.Position;
                string pType = ParseTypeName(p.ParameterType);
                string pName = p.Name;

                lines.Add($"{prefix}[Key({position})]\n{prefix}public {pType} {pName};\n"); 
            }

            return string.Join("\n", lines);
        }

        static string ParseMessageFields(Type[] types, string[] names, string prefix)
        {
            List<string> lines = new List<string>();
            for (var i=0;i<types.Length;++i)
            {
                var t = types[i];
                int position = i;
                string pType = ParseTypeName(t);
                var attr = t.GetCustomAttributes(typeof(RpcArgAttribute)).FirstOrDefault();
                string pName;
                string ns = t.Namespace;
                if (attr != null)
                    pName = ((RpcArgAttribute)attr).Name;
                else
                    pName = names.Length > position ? names[position] : "arg" + position.ToString();
                lines.Add($"{prefix}[Key({position})]\n{prefix}public {pType} {pName};\n");
            }

            return string.Join("\n", lines);
        }

        static string GenCallbackMsgDecl(ParameterInfo[] paramInfos, string[] names, string prefix)
        {
            foreach (var p in paramInfos)
            {
                if (p.Name != "callback")
                    continue;
                string code = ParseMessageFields(p.ParameterType.GetGenericArguments(), names, prefix + "    ");
                var builder = new StringBuilder()
                    .AppendLine($"        [MessagePackObject]")
                    .AppendLine($"        public class Callback")
                    .AppendLine($"        {{")
                    .AppendLine($"{code}")
                    .AppendLine($"        }}");
                return builder.ToString();
            }
         
            return "";
        }

        static string[] GetCallbackArgs(MethodInfo method)
        {
            var attr = method.GetCustomAttributes(typeof(CallbackArgsAttribute)).FirstOrDefault();
            if (attr == null)
                return new string[] { };
            return (attr as CallbackArgsAttribute).Names;
        }

        static void GenProtoCode(List<Type> types, string sharedPath, string clientPath, string serverPath)
        {
            
            foreach (var type in types)
            {
                if (type.GetCustomAttribute(typeof(ActorTypeAttribute), true) == null)
                    continue;
                var codes = new SortedDictionary<string, uint>();
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly); 
                for (int i = 0; i < methods.Length; ++i)
                {
                    MethodInfo method = methods[i];
                    var attrs = method.GetCustomAttributes(typeof(ServerApiAttribute));
                    if (attrs.Count() > 0)
                    {
                        uint code = Basic.GenID32FromName(method.Name);
                        string proto_code = NameToProtoCode(method.Name) + "_REQ";
                        codes[proto_code] = code;
                    }
                }

                for (int i = 0; i < methods.Length; ++i)
                {
                    MethodInfo method = methods[i];
                    var attrs = method.GetCustomAttributes(typeof(ServerOnlyAttribute));
                    if (attrs.Count() > 0)
                    {
                        uint code = Basic.GenID32FromName(method.Name);
                        string proto_code = NameToProtoCode(method.Name) + "_REQ";
                        codes[proto_code] = code;
                    }
                }

                for (int i = 0; i < methods.Length; ++i)
                {
                    MethodInfo method = methods[i];
                    var attrs = method.GetCustomAttributes(typeof(ClientApiAttribute));
                    if (attrs.Count() > 0)
                    {
                        uint code = Basic.GenID32FromName(method.Name);
                        string proto_code = NameToProtoCode(method.Name) + "_NTF";
                        codes[proto_code] = code;
                    }
                }

                bool isServer = (type.GetCustomAttribute(typeof(ActorTypeAttribute), true) as ActorTypeAttribute).AType == AType.SERVER;

                using (var sw = new StreamWriter(Path.Combine(sharedPath, "Protocol", string.Format("ProtocolCode.{0}.{1}.cs", type.Name, isServer?"s":"c")), false, Encoding.UTF8))
                {
                    string lines = @"
//AUTOGEN, do not modify it!

using System;
using System.Collections.Generic;
using System.Text; 
namespace Shared
{
    public partial class ProtocolCode
    {
";
                    foreach (var kv in codes)
                    {
                        lines += string.Format("        public const uint {0} = {1};\n", kv.Key, kv.Value);
                    }
                    lines += @"    }
}
";

                    sw.WriteLine(lines.Replace("\r", ""));
                }
            }
        }

        static string GenCbArgs(Type[] types, string[] names, string instanceName)
        {
            string args = ""; 
             
            for (var i = 0; i < types.Length; ++i)
            {
                var t = types[i];
                int position = i;
                string pType = ParseTypeName(t);
                var attr = t.GetCustomAttribute(typeof(RpcArgAttribute));
                string pName;
                if (attr != null)
                    pName = ((RpcArgAttribute)attr).Name;
                else
                    pName = names.Length > position ? names[position] : "arg" + position.ToString();

                args += instanceName + pName + ", ";
            }

            if (args.EndsWith(", "))
                args = args.Substring(0, args.Length - ", ".Length);

            return args;
        }

        static string GetApiMessagePostfix(Api api)
        {
            if (api == Api.ClientApi)
                return "Ntf";
            if (api == Api.ServerApi)
                return "Req";
            if (api == Api.ServerOnly)
                return "Req";
            return "";
        }

        static void GenFromActorType(Type type, string sharedPath, string clientPath, string serverPath)
        { 
            var rpcDefineDic = new SortedDictionary<string, string>();
            var apiDefineDic = new SortedDictionary<string, string>();

            if (type.GetCustomAttribute(typeof(ActorTypeAttribute), true) == null)
                return;

            bool isServer = (type.GetCustomAttribute(typeof(ActorTypeAttribute), true) as ActorTypeAttribute).AType == AType.SERVER;
             
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo method = methods[i];
                var attr = method.GetCustomAttributes(typeof(ServerApiAttribute)).FirstOrDefault();
                Api api = Api.NoneApi;
                if (attr != null)
                    api = Api.ServerApi;
                else
                {
                    attr = method.GetCustomAttributes(typeof(ServerOnlyAttribute)).FirstOrDefault();
                    if (attr != null)
                        api = Api.ServerOnly;
                    else
                    {
                        attr = method.GetCustomAttributes(typeof(ClientApiAttribute)).FirstOrDefault();
                        if (attr != null)
                            api = Api.ClientApi;
                    }
                } 

                if(api == Api.ClientApi)
                {
                    if(RpcUtil.IsHeritedType(type, "Service"))
                    {
                        Console.WriteLine("client_api not allowed in Service", type.Name);
                        continue;
                    }
                }

                if (api != Api.NoneApi)
                {
                    uint code = Basic.GenID32FromName(method.Name);

                    //现在生成message
                    string message_type = method.Name + GetApiMessagePostfix(api);

                    string message_fields = ParseMessageFields(method.GetParameters(), "        ");

                    string callback_define = GenCallbackMsgDecl(method.GetParameters(),
                        GetCallbackArgs(method),
                        "        ");

                    string itype = "IMessage";
                    if (callback_define != "")
                        itype = "IMessageWithCallback";

                    string proto_code = NameToProtoCode(method.Name) + "_" + GetApiMessagePostfix(api).ToUpper();

                    var msgBuilder = new StringBuilder()
                        .AppendLine($"//AUTOGEN, do not modify it!\n")
                        .AppendLine($"using Fenix.Common.Attributes;")
                        .AppendLine($"using Fenix.Common.Rpc;")
                        .AppendLine($"using MessagePack; ")
                        .AppendLine($"using Shared;")
                        .AppendLine($"using Shared.Protocol;")
                        .AppendLine($"using System; ")
                        .AppendLine($"using Shared.DataModel;")
                        .AppendLine($"")
                        .AppendLine($"namespace Shared.Message")
                        .AppendLine($"{{")
                        .AppendLine($"    [MessageType(ProtocolCode.{proto_code})]")
                        .AppendLine($"    [MessagePackObject]")
                        .AppendLine($"    public class {message_type} : {itype}")
                        .AppendLine($"    {{")
                        .AppendLine($"{message_fields}");
                    if (callback_define != "")
                    {
                        msgBuilder.AppendLine(@"
        [Key(199)]
        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 
").AppendLine($"{callback_define}");
                    }

                    msgBuilder.AppendLine($"    }}")
                        .AppendLine($"}}");

                    var msgCode = msgBuilder.ToString();

                    using (var sw = new StreamWriter(Path.Combine(sharedPath, "Message", message_type + ".cs"), false, Encoding.UTF8))
                    {
                        sw.WriteLine(msgCode.Replace("\r", ""));
                    }

                    //现在生成actor_ref定义 
                    var rpc_name = "rpc_" + NameToApi(method.Name);
                    if (api == Api.ClientApi)
                        rpc_name = "client_on_" + NameToApi(method.Name);
                    else if(api == Api.ServerOnly)
                        rpc_name = "rpc_" + NameToApi(method.Name);

                    string args_decl = ParseArgsDecl(method.GetParameters());

                    string typename = type.Name;
                    string args = ParseArgs(method.GetParameters());

                    string method_name = method.Name;
                    //string msg_type = method.Name + GetApiMessagePostfix(api);
                    string msg_assign = ParseArgsMsgAssign(method.GetParameters(), "                ");

                    StringBuilder builder;

                    if (callback_define != "")
                    {
                        var cbType = method.GetParameters().Where(m => m.Name == "callback").First().ParameterType;
                        string cb_args = GenCbArgs(cbType.GetGenericArguments(), GetCallbackArgs(method), "cbMsg.");

                        builder = new StringBuilder()
                        .AppendLine($"        public void {rpc_name}({args_decl})")
                        .AppendLine($"        {{")
                        .AppendLine($"            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);")
                        .AppendLine($"            if (this.fromActor.HostId == toHostId)")
                        .AppendLine($"            {{")
                        .AppendLine($"                Host.Instance.GetActor(this.toActorId).CallLocalMethod(ProtocolCode.{proto_code}, new object[] {{ {args} }});")
                        //.AppendLine($"                (({typename})Host.Instance.GetActor(this.toActorId)).{method_name}({args});")
                        .AppendLine($"                return;")
                        .AppendLine($"            }}")
                        .AppendLine($"            var msg = new {message_type}()")
                        .AppendLine($"            {{")
                        .AppendLine($"{msg_assign}")
                        .AppendLine($"            }};")
                        .AppendLine($"            var cb = new Action<byte[]>((cbData) => {{")
                        .AppendLine($"                var cbMsg = RpcUtil.Deserialize<{message_type}.Callback>(cbData);")
                        .AppendLine($"                callback?.Invoke({cb_args});")
                        .AppendLine($"            }});")
                        .AppendLine($"            this.CallRemoteMethod(ProtocolCode.{proto_code}, msg, cb);")
                        .AppendLine($"        }}");
                    }
                    else
                    {
                        builder = new StringBuilder()
                        .AppendLine($"        public void {rpc_name}({args_decl})")
                        .AppendLine($"        {{")
                        .AppendLine($"           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);")
                        .AppendLine($"           if (this.fromActor.HostId == toHostId)")
                        .AppendLine($"           {{")
                        .AppendLine($"                Host.Instance.GetActor(this.toActorId).CallLocalMethod(ProtocolCode.{proto_code}, new object[] {{ {args} }});")
                        //.AppendLine($"                (({typename})Host.Instance.GetActor(this.toActorId)).{method_name}({args});")
                        .AppendLine($"               return;")
                        .AppendLine($"           }}")
                        .AppendLine($"           var msg = new {message_type}()")
                        .AppendLine($"           {{")
                        .AppendLine($"{msg_assign}")
                        .AppendLine($"           }};") 
                        .AppendLine($"           this.CallRemoteMethod(ProtocolCode.{proto_code}, msg, null);")
                        .AppendLine($"        }}");
                    } 

                    var rpcDefineCode = builder.ToString();
                    rpcDefineDic[rpc_name] = rpcDefineCode;

                    
                    string api_rpc_args = ParseArgs(method.GetParameters(), "_msg.", "callback");
                    //string api_assign = ParseArgsMsgAssign(method.GetParameters(), "                ", "cbMsg.");
                    
                    string api_name = "_INTERNAL_SERVER_API_"+NameToApi(method.Name);
                    if (api == Api.ClientApi)
                        api_name = "_INTERNAL_CLIENT_API_" + NameToApi(method.Name);
                    else if (api == Api.ServerOnly)
                        api_name = "_INTERNAL_SERVER_ONLY_" + NameToApi(method.Name);

                    builder = new StringBuilder()
                        .AppendLine($"        [RpcMethod(ProtocolCode.{proto_code})]")
                        .AppendLine($"        [EditorBrowsable(EditorBrowsableState.Never)]")
                        .AppendLine($"        public void {api_name}(IMessage msg, Action<object> cb)")
                        .AppendLine($"        {{")
                        .AppendLine($"            var _msg = ({message_type})msg;");
                    
                    if (callback_define != "")
                    {
                        var cbType2 = method.GetParameters().Where(m => m.Name == "callback").First().ParameterType;
                        string api_cb_args = GenCbArgs(cbType2.GetGenericArguments(), GetCallbackArgs(method), "");
                        string api_cb_assign = ParseArgsMsgAssign(cbType2.GetGenericArguments(),
                                                        GetCallbackArgs(method),
                                                        "                ",
                                                        "cbMsg.");
                        builder.AppendLine($"            this.{method.Name}({api_rpc_args}, ({api_cb_args}) =>")
                        .AppendLine($"            {{")
                        .AppendLine($"                var cbMsg = new {message_type}.Callback();")
                        .AppendLine($"{api_cb_assign}")
                        .AppendLine($"                cb.Invoke(cbMsg);")
                        .AppendLine($"            }});");
                    }

                    builder.AppendLine($"        }}");

                    var apiDefineCode = builder.ToString();
                    apiDefineDic[api_name] = apiDefineCode;
                }
            }

            string refCode = string.Join("\n", rpcDefineDic.Values);
            string tname = type.Name;
            string ns = type.Namespace;

            var alAttr = type.GetCustomAttribute(typeof(AccessLevelAttribute));
            if (alAttr == null)
            {
                Console.WriteLine(string.Format("ERROR: {0} has no AccessLevel", type.Name));
                return;
            }

            var al = (alAttr as AccessLevelAttribute).AccessLevel;
            Console.WriteLine(string.Format("AccessLevel {0} : {1}", type.Name, al));

            string root_ns = isServer ? "Server" : "Client";

            var refBuilder = new StringBuilder()
                .AppendLine(@"
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
")//.AppendLine($"using {ns};")
.AppendLine(@"using MessagePack; 
using System;
")
.AppendLine($"namespace {root_ns}")
.AppendLine(@"{
")
.AppendLine($"    [RefType(\"{tname}\")]")
.AppendLine($"    public partial class {tname}Ref : ActorRef")
.AppendLine($"    {{")
.AppendLine($"{refCode}    }}") 
.AppendLine($"}}");
            var result = refBuilder.ToString();

            

            using (var sw = new StreamWriter(Path.Combine(sharedPath, "ActorRef", root_ns, type.Name + "Ref.cs"), false, Encoding.UTF8))
            { 
                sw.WriteLine(result);
            }

            string internalApiCode = string.Join("\n", apiDefineDic.Values);
            var apiBuilder = new StringBuilder()
                .AppendLine(@"
//AUTOGEN, do not modify it!

using Fenix.Common.Attributes;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;
using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Server.UModule;

")
                .AppendLine($"namespace {ns}")
.AppendLine("{")
.AppendLine($"    public partial class {tname}")
.AppendLine($"    {{")
.AppendLine($"{internalApiCode}    }}")
.AppendLine($"}}");
            var apiResultCode = apiBuilder.ToString();
            string output = isServer ? serverPath : clientPath;
            using (var sw = new StreamWriter(Path.Combine(output, "Stub", type.Name + ".Stub.cs"), false, Encoding.UTF8))
            {
                sw.WriteLine(apiResultCode);
            }
        }
    }
}
