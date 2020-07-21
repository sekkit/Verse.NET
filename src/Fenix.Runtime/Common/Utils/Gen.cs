using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils; 

namespace Fenix
{
    public class Gen
    { 
        public static void Autogen(Assembly asm, string output)
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
              
            GenProtoCode(actorTypes, output); 
             
            foreach(var type in actorTypes)
                GenFromActorType(type, output);
             
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

        static void GenProtoCode(List<Type> types, string output)
        {
            var codes = new SortedDictionary<string, uint>();
            foreach (var type in types)
            {
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
            }

            using (var sw = new StreamWriter(Path.Combine(output, "Protocol", "ProtocolCode.cs"), false, Encoding.UTF8))
            {
                string lines = @"
using System;
using System.Collections.Generic;
using System.Text; 
namespace Shared
{
    public class ProtocolCode
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

        static void GenFromActorType(Type type, string output)
        { 
            var rpcDefineDic = new SortedDictionary<string, string>();
            var apiDefineDic = new SortedDictionary<string, string>();

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo method = methods[i];
                var attrs = method.GetCustomAttributes(typeof(ServerApiAttribute));
                if (attrs.Count() > 0)
                {
                    uint code = Basic.GenID32FromName(method.Name);
                     
                    //现在生成message
                    string message_type = method.Name + "Req";

                    string message_fields = ParseMessageFields(method.GetParameters(), "        ");

                    string callback_define = GenCallbackMsgDecl(method.GetParameters(), 
                        (attrs.First() as ServerApiAttribute).CallbackArgs, 
                        "        ");

                    string itype = "IMessage";
                    if (callback_define != "")
                        itype = "IMessageWithCallback"; 

                    string proto_code = NameToProtoCode(method.Name) + "_REQ"; 

                    var msgBuilder = new StringBuilder()
                        .AppendLine($"//AUTOGEN, do not modify it!")
                        .AppendLine($"using Fenix.Common.Attributes;")
                        .AppendLine($"using Fenix.Common.Rpc;")
                        .AppendLine($"using MessagePack; ")
                        .AppendLine($"using Shared.Protocol;")
                        .AppendLine($"using System; ")
                        .AppendLine($"")
                        .AppendLine($"namespace Shared.Protocol.Message")
                        .AppendLine($"{{")
                        .AppendLine($"    [MessageType(ProtocolCode.{proto_code})]")
                        .AppendLine($"    [MessagePackObject]")
                        .AppendLine($"    public class {message_type} : {itype}")
                        .AppendLine($"    {{")
                        .AppendLine($"{message_fields}");
                    if(callback_define != "")
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

                    using (var sw = new StreamWriter(Path.Combine(output, "Message", message_type + ".cs"), false, Encoding.UTF8))
                    {
                        sw.WriteLine(msgCode.Replace("\r", ""));
                    }

                    //现在生成actor_ref定义

                    var rpc_name = "rpc_" + NameToApi(method.Name);
                     
                    string args_decl = ParseArgsDecl(method.GetParameters());

                    string typename = type.Name;
                    string args = ParseArgs(method.GetParameters());

                    string method_name = method.Name;
                    string msg_type = method.Name + "Req";
                    string msg_assign = ParseArgsMsgAssign(method.GetParameters(), "                ");
                     
                    StringBuilder builder;

                    if(callback_define != "")
                    {
                        var cbType = method.GetParameters().Where(m => m.Name == "callback").First().ParameterType;
                        string cb_args = GenCbArgs(cbType.GetGenericArguments(), (attrs.First() as ServerApiAttribute).CallbackArgs, "cbMsg.");

                        builder = new StringBuilder()
                        .AppendLine($"public void {rpc_name}({args_decl})")
                        .AppendLine($"        {{")
                        .AppendLine($"            var toContainerId = Global.IdManager.GetContainerIdByActorId(this.toActorId);")
                        .AppendLine($"            if (this.fromActor.ContainerId == toContainerId)")
                        .AppendLine($"            {{")
                        .AppendLine($"                (({typename})Container.Instance.GetActor(this.toActorId)).{method_name}({args});")
                        .AppendLine($"                return;")
                        .AppendLine($"            }}")
                        .AppendLine($"            var msg = new {msg_type}()")
                        .AppendLine($"            {{")
                        .AppendLine($"{msg_assign}")
                        .AppendLine($"            }};")
                        .AppendLine($"            var cb = new Action<byte[]>((cbData) => {{")
                        .AppendLine($"                var cbMsg = RpcUtil.Deserialize<{msg_type}.Callback>(cbData);")
                        .AppendLine($"                callback?.Invoke({cb_args});")
                        .AppendLine($"            }});")
                        .AppendLine($"            this.CallRemoteMethod(ProtocolCode.{proto_code}, msg, cb);")
                        .AppendLine($"        }}");
                    }
                    else
                    {
                        builder = new StringBuilder()
                        .AppendLine($"public void {rpc_name}({args_decl})")
                        .AppendLine($"        {{")
                        .AppendLine($"           var toContainerId = Global.IdManager.GetContainerIdByActorId(this.toActorId);")
                        .AppendLine($"           if (this.fromActor.ContainerId == toContainerId)")
                        .AppendLine($"           {{")
                        .AppendLine($"               (({typename})Container.Instance.GetActor(this.toActorId)).{method_name}({args});")
                        .AppendLine($"               return;")
                        .AppendLine($"           }}")
                        .AppendLine($"           var msg = new {msg_type}()")
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
                    builder = new StringBuilder()
                        .AppendLine($"    [RpcMethod(ProtocolCode.{proto_code})]")
                        .AppendLine($"    [EditorBrowsable(EditorBrowsableState.Never)]")
                        .AppendLine($"    public void {api_name}(IMessage msg, Action<object> cb)")
                        .AppendLine($"    {{")
                        .AppendLine($"        var _msg = ({msg_type})msg;");
                    
                    if (callback_define != "")
                    {
                        var cbType2 = method.GetParameters().Where(m => m.Name == "callback").First().ParameterType;
                        string api_cb_args = GenCbArgs(cbType2.GetGenericArguments(), (attrs.First() as ServerApiAttribute).CallbackArgs, "");
                        string api_cb_assign = ParseArgsMsgAssign(cbType2.GetGenericArguments(),
                                                        (attrs.First() as ServerApiAttribute).CallbackArgs,
                                                        "            ",
                                                        "cbMsg.");
                        builder.AppendLine($"        this.{method.Name}({api_rpc_args}, ({api_cb_args}) =>")
                        .AppendLine($"        {{")
                        .AppendLine($"            var cbMsg = new {msg_type}.Callback();")
                        .AppendLine($"{api_cb_assign}")
                        .AppendLine($"            cb.Invoke(cbMsg);")
                        .AppendLine($"        }});");
                    }

                    builder.AppendLine($"    }}");

                    var apiDefineCode = builder.ToString();
                    apiDefineDic[api_name] = apiDefineCode;
                } 

                attrs = method.GetCustomAttributes(typeof(ServerOnlyAttribute));
                if (attrs.Count() > 0)
                {
                    uint code = Basic.GenID32FromName(method.Name);
                     
                }

                attrs = method.GetCustomAttributes(typeof(ClientApiAttribute));
                if (attrs.Count() > 0)
                {
                    uint code = Basic.GenID32FromName(method.Name); 
                }
            }

            string refCode = string.Join("\n", rpcDefineDic.Values);
            string tname = type.Name;
            string ns = type.Namespace;
            var refBuilder = new StringBuilder()
                .AppendLine(@"
using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Shared.Protocol;
").AppendLine($"using {ns};")
.AppendLine(@"using MessagePack;
using Shared.Protocol.Message;
using System;

namespace Shared
{
")
.AppendLine($"    [RefType(typeof({tname}))]")
.AppendLine($"    public class {tname}Ref : ActorRef")
.AppendLine($"    {{")
.AppendLine($"        { refCode}    }}") 
.AppendLine($"}}");
            var result = refBuilder.ToString();

            using (var sw = new StreamWriter(Path.Combine(output, "ActorRef", type.Name + "Ref.cs"), false, Encoding.UTF8))
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
using Shared.Protocol;
using Shared.Protocol.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

")
                .AppendLine($"namespace {ns}")
.AppendLine("{")
.AppendLine($"    public partial class {tname}")
.AppendLine($"    {{")
.AppendLine($"{internalApiCode}")
.AppendLine($"    }}")
.AppendLine($"}}");
            var apiResultCode = apiBuilder.ToString();
            using (var sw = new StreamWriter(Path.Combine(output, "Stub", type.Name + ".Stub.cs"), false, Encoding.UTF8))
            {
                sw.WriteLine(apiResultCode);
            }
        }
    }
}
