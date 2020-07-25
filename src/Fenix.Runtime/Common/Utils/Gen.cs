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
using DotNetty.Common.Utilities;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils; 

namespace Fenix
{
    public class Gen
    {
        public static void AutogenActor(Assembly asm, bool isServer, string sharedPath, string clientPath, string serverPath)
        {
            List<Type> actorTypes = new List<Type>();
            foreach (Type type in asm.GetTypes())
            {
                if (type.IsAbstract)
                    continue;
 
                if (!RpcUtil.IsHeritedType(type, "Actor"))
                    continue;

                var attr = GetAttribute<ActorTypeAttribute>(type); 
                if (attr == null)
                    continue;

                var at = (int)attr.AType;
                if (isServer && at != (int)AType.SERVER)
                    continue;
                if (!isServer && at != (int)AType.CLIENT)
                    continue;

                actorTypes.Add(type);
            }

            GenProtoCode(actorTypes, sharedPath, clientPath, serverPath);

            foreach (var type in actorTypes)
                GenFromActorType(type, sharedPath, clientPath, serverPath);
        }

        public static void AutogenHost(Assembly asm, string sharedPath, string clientPath, string serverPath)
        {
            List<Type> actorTypes = new List<Type>();
            foreach (Type type in asm.GetTypes())
            {
                if (type.IsAbstract)
                    continue;

                if (type.FullName == "Fenix.Host")
                {
                    GenFromActorType(type, sharedPath, clientPath, serverPath); 
                }
            } 
        }

        static dynamic GetAttribute<T>(Type type) where T: Attribute
        {
            var attrs = type.GetCustomAttributes(true);
            return attrs.Where(m => (m.GetType().Name == typeof(T).Name)).FirstOrDefault();
        }

        static dynamic GetAttribute<T>(MethodInfo methodInfo) where T : Attribute
        {
            var attrs = methodInfo.GetCustomAttributes(true);
            return attrs.Where(m => (m.GetType().Name == typeof(T).Name)).FirstOrDefault();
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
                if (p.Name.StartsWith("__"))
                    continue;
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
                if (p.Name.StartsWith("__"))
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
                if (p.Name.StartsWith("__"))
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
                var attr = GetAttribute<RpcArgAttribute>(t);
                string pName;
                if (attr != null) 
                     pName = attr.Name;
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
                if (p.Name.StartsWith("__"))
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
                var attr = GetAttribute<RpcArgAttribute>(t);  
                string pName;
                string ns = t.Namespace;
                if (attr != null)
                    pName = attr.Name;
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
                if (p.Name.StartsWith("__"))
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
            var attr = GetAttribute<CallbackArgsAttribute>(method); 
            if (attr == null)
                return new string[] { };
            return attr.Names;
        }

        static void GenProtoCode(List<Type> types, string sharedPath, string clientPath, string serverPath)
        {
            
            foreach (var type in types)
            {
                if (GetAttribute<ActorTypeAttribute>(type) == null)
                    continue;
                var codes = new SortedDictionary<string, uint>();
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly); 
                for (int i = 0; i < methods.Length; ++i)
                {
                    MethodInfo method = methods[i];
                    var attr = GetAttribute<ServerApiAttribute>(method); 
                    if (attr != null)
                    {
                        uint code = Basic.GenID32FromName(method.Name);
                        string proto_code = NameToProtoCode(method.Name) + "_REQ";
                        codes[proto_code] = code;
                    }
                }

                for (int i = 0; i < methods.Length; ++i)
                {
                    MethodInfo method = methods[i];
                    var attr = GetAttribute<ServerOnlyAttribute>(method); 
                    if (attr != null)
                    {
                        uint code = Basic.GenID32FromName(method.Name);
                        string proto_code = NameToProtoCode(method.Name) + "_REQ";
                        codes[proto_code] = code;
                    }
                }

                for (int i = 0; i < methods.Length; ++i)
                {
                    MethodInfo method = methods[i];
                    var attr = GetAttribute<ClientApiAttribute>(method); 
                    if (attr != null)
                    {
                        uint code = Basic.GenID32FromName(method.Name);
                        string proto_code = NameToProtoCode(method.Name) + "_NTF";
                        codes[proto_code] = code;
                    }
                }

                bool isServer = (int)GetAttribute<ActorTypeAttribute>(type).AType == (int)AType.SERVER; 

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
                var attr = GetAttribute<RpcArgAttribute>(t); 
                string pName;
                if (attr != null)
                    pName = attr.Name;
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

            if (GetAttribute<ActorTypeAttribute>(type) == null && type.Name != "Host")
                return;

            bool isServer = type.Name == "Host" ? true : ((int)GetAttribute<ActorTypeAttribute>(type).AType == (int)AType.SERVER);

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo method = methods[i];
                var attr = GetAttribute<ServerApiAttribute>(method);  
                Api api = Api.NoneApi;
                if (attr != null)
                    api = Api.ServerApi;
                else
                {
                    attr = GetAttribute<ServerOnlyAttribute>(method);  
                    if (attr != null)
                        api = Api.ServerOnly;
                    else
                    {
                        attr = GetAttribute<ClientApiAttribute>(method);  
                        if (attr != null)
                            api = Api.ClientApi;
                    }
                } 

                if(api == Api.ClientApi)
                {
                    if(RpcUtil.IsHeritedType(type, "Service"))
                    {
                        Log.Info(string.Format("client_api not allowed in Service {0}", type.Name));
                        continue;
                    }
                }  

                if (api != Api.NoneApi)
                {
                    var methodParameterList = method.GetParameters();

                    if (type.Name == "Host")
                        methodParameterList = methodParameterList.Slice(0, methodParameterList.Length - 1);

                    uint code = Basic.GenID32FromName(method.Name);

                    //现在生成message
                    string message_type = method.Name + GetApiMessagePostfix(api);

                    string message_fields = ParseMessageFields(methodParameterList, "        ");

                    string callback_define = GenCallbackMsgDecl(methodParameterList,
                        GetCallbackArgs(method),
                        "        ");

                    string itype = "IMessage";
                    if (callback_define != "")
                        itype = "IMessageWithCallback";

                    string proto_code = NameToProtoCode(method.Name) + "_" + GetApiMessagePostfix(api).ToUpper();

                    string msg_ns = type.Name == "Host" ? "Fenix.Common.Message" : "Shared.Message";

                    string pc_cls = type.Name == "Host" ? "OpCode" : "ProtocolCode";

                    var msgBuilder = new StringBuilder()
                        .AppendLine($"//AUTOGEN, do not modify it!\n")
                        .AppendLine($"using Fenix.Common;")
                        .AppendLine($"using Fenix.Common.Attributes;")
                        .AppendLine($"using Fenix.Common.Rpc;")
                        .AppendLine($"using MessagePack; ");
                    if (type.Name != "Host")
                    {
                        msgBuilder.AppendLine($"using Shared;")
                        .AppendLine($"using Shared.Protocol;")
                        .AppendLine($"using Shared.DataModel;");
                    }
                    msgBuilder.AppendLine($"using System; ")
                        .AppendLine($"")
                        .AppendLine($"namespace {msg_ns}")
                        .AppendLine($"{{")
                        .AppendLine($"    [MessageType({pc_cls}.{proto_code})]")
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

                    if (type.Name == "Host")
                        rpc_name = method.Name;

                    string args_decl = ParseArgsDecl(methodParameterList);

                    string typename = type.Name;
                    string args = ParseArgs(methodParameterList);

                    string method_name = method.Name;
                    //string msg_type = method.Name + GetApiMessagePostfix(api);
                    string msg_assign = ParseArgsMsgAssign(methodParameterList, "                ");

                    StringBuilder builder;

                    if (callback_define != "")
                    {
                        var cbType = methodParameterList.Where(m => m.Name == "callback").First().ParameterType;
                        string cb_args = GenCbArgs(cbType.GetGenericArguments(), GetCallbackArgs(method), "cbMsg.");

                        builder = new StringBuilder()
                        .AppendLine($"        public void {rpc_name}({args_decl})")
                        .AppendLine($"        {{")
                        .AppendLine($"            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);//, this.isClient);")
                        .AppendLine($"            if (this.FromHostId == toHostId)")
                        .AppendLine($"            {{")
                        .AppendLine($"                Host.Instance.GetActor(this.toActorId).CallLocalMethod({pc_cls}.{proto_code}, new object[] {{ {args} }});")
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
                        .AppendLine($"            this.CallRemoteMethod({pc_cls}.{proto_code}, msg, cb);")
                        .AppendLine($"        }}");
                    }
                    else
                    {
                        builder = new StringBuilder()
                        .AppendLine($"        public void {rpc_name}({args_decl})")
                        .AppendLine($"        {{")
                        .AppendLine($"           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId);//, this.isClient);")
                        .AppendLine($"           if (this.FromHostId == toHostId)")
                        .AppendLine($"           {{")
                        .AppendLine($"                Host.Instance.GetActor(this.toActorId).CallLocalMethod({pc_cls}.{proto_code}, new object[] {{ {args} }});")
                        //.AppendLine($"                (({typename})Host.Instance.GetActor(this.toActorId)).{method_name}({args});")
                        .AppendLine($"               return;")
                        .AppendLine($"           }}")
                        .AppendLine($"           var msg = new {message_type}()")
                        .AppendLine($"           {{")
                        .AppendLine($"{msg_assign}")
                        .AppendLine($"           }};") 
                        .AppendLine($"           this.CallRemoteMethod({pc_cls}.{proto_code}, msg, null);")
                        .AppendLine($"        }}");
                    } 

                    var rpcDefineCode = builder.ToString();
                    rpcDefineDic[rpc_name] = rpcDefineCode;

                    
                    string api_rpc_args = ParseArgs(methodParameterList, "_msg.", "callback"); 
                    string api_type = "ServerApi";
                    string api_name = "SERVER_API_"+NameToApi(method.Name);
                    if (api == Api.ClientApi)
                    {
                        api_name = "CLIENT_API_" + NameToApi(method.Name);
                        api_type = "ClientApi";
                    }
                    else if (api == Api.ServerOnly)
                    { 
                        api_name = "SERVER_ONLY_" + NameToApi(method.Name);
                        api_type = "ServerOnly";
                    }
                    builder = new StringBuilder()
                        .AppendLine($"        [RpcMethod({pc_cls}.{proto_code}, Api.{api_type})]")
                        .AppendLine($"        [EditorBrowsable(EditorBrowsableState.Never)]");
                    if(type.Name == "Host")
                    {
                        if (callback_define != "")
                            builder.AppendLine($"        public void {api_name}(IMessage msg, Action<object> cb, RpcContext context)");
                        else
                            builder.AppendLine($"        public void {api_name}(IMessage msg, RpcContext context)");
                    }
                    else
                    {
                        if (callback_define != "")
                            builder.AppendLine($"        public void {api_name}(IMessage msg, Action<object> cb)");
                        else
                            builder.AppendLine($"        public void {api_name}(IMessage msg)");
                    }
                    

                    builder.AppendLine($"        {{")
                        .AppendLine($"            var _msg = ({message_type})msg;");
                  

                    if (callback_define != "")
                    {
                        var cbType2 = methodParameterList.Where(m => m.Name == "callback").First().ParameterType;
                        string api_cb_args = GenCbArgs(cbType2.GetGenericArguments(), GetCallbackArgs(method), "");
                        string api_cb_assign = ParseArgsMsgAssign(cbType2.GetGenericArguments(),
                                                        GetCallbackArgs(method),
                                                        "                ",
                                                        "cbMsg.");
                        builder.AppendLine($"            this.{method.Name}({api_rpc_args}, ({api_cb_args}) =>")
                        .AppendLine($"            {{")
                        .AppendLine($"                var cbMsg = new {message_type}.Callback();")
                        .AppendLine($"{api_cb_assign}")
                        .AppendLine($"                cb.Invoke(cbMsg);");
                        if(type.Name == "Host")
                            builder.AppendLine($"            }}, context);");
                        else
                            builder.AppendLine($"            }});");
                    }
                    else
                    {
                        
                        if (type.Name == "Host")
                            builder.AppendLine($"            this.{method.Name}({api_rpc_args}, context);");
                        else
                            builder.AppendLine($"            this.{method.Name}({api_rpc_args});");
                    }

                    builder.AppendLine($"        }}");

                    var apiDefineCode = builder.ToString();
                    apiDefineDic[api_name] = apiDefineCode;
                }
            }

            string refCode = string.Join("\n", rpcDefineDic.Values);
            string tname = type.Name;
            string ns = type.Namespace;

            var alAttr = GetAttribute<AccessLevelAttribute>(type); 
            if (alAttr == null && type.Name != "Host")
            {
                Log.Info(string.Format("ERROR: {0} has no AccessLevel", type.Name));
                return;
            }

            if (type.Name != "Host")
            {
                var al = alAttr.AccessLevel;
                Log.Info(string.Format("AccessLevel {0} : {1}", type.Name, al));
            }

            string root_ns = type.Name == "Host" ? "Fenix":(isServer ? "Server" : "Client");

            string refTypeName = tname == "Avatar" ? type.FullName : tname;

            var refBuilder = new StringBuilder()
                .AppendLine(@"
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Fenix.Common.Message;

using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
")
.AppendLine(@"using MessagePack; 
using System;
")
.AppendLine($"namespace {root_ns}")
.AppendLine(@"{
");
            if (type.Name != "Host")
            {
                refBuilder.AppendLine($"    [RefType(\"{refTypeName}\")]")
                    .AppendLine($"    public partial class {tname}Ref : ActorRef");
            }
            else
            { 
                refBuilder.AppendLine($"    public partial class ActorRef");
            }
refBuilder.AppendLine($"    {{")
.AppendLine($"{refCode}    }}") 
.AppendLine($"}}");
            var result = refBuilder.ToString();
             
            if(type.Name == "Host")
            {
                using (var sw = new StreamWriter(Path.Combine(sharedPath, "../Actor", "ActorRef.rpc.cs"), false, Encoding.UTF8))
                {
                    sw.WriteLine(result);
                }
            }
            else
            {
                using (var sw = new StreamWriter(Path.Combine(sharedPath, "ActorRef", root_ns, type.Name + "Ref.cs"), false, Encoding.UTF8))
                { 
                    sw.WriteLine(result);
                }
            }
            

            string internalApiCode = string.Join("\n", apiDefineDic.Values);
            var apiBuilder = new StringBuilder()
                .AppendLine(@"
//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Message;
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

")
                .AppendLine($"namespace {ns}")
.AppendLine("{")
.AppendLine($"    public partial class {tname}")
.AppendLine($"    {{")
.AppendLine($"{internalApiCode}    }}")
.AppendLine($"}}");

             
            var apiResultCode = apiBuilder.ToString();
            //if (type.Name != "Host")
            //{
                string output = isServer ? serverPath : clientPath;
                using (var sw = new StreamWriter(Path.Combine(output, "Stub", type.Name + ".Stub.cs"), false, Encoding.UTF8))
                {
                    sw.WriteLine(apiResultCode);
                }
            //}
            //else
            //{ 
            //    using (var sw = new StreamWriter(Path.Combine(serverPath, "Stub", type.Name + ".Stub.cs"), false, Encoding.UTF8))
            //    {
            //        sw.WriteLine(apiResultCode);
            //    }
                 
            //    using (var sw = new StreamWriter(Path.Combine(clientPath, "Stub", type.Name + ".Stub.cs"), false, Encoding.UTF8))
            //    {
            //        sw.WriteLine(apiResultCode);
            //    }
            //}
        }
    }
}
