
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;

using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Fenix
{
    public class Gen2
    {
        public static void AutogenActor(AssemblyDefinition ad, bool isServer, string sharedCPath, string sharedSPath, string clientPath, string serverPath)
        {
            List<TypeDefinition> actorTypes = new List<TypeDefinition>(); 
            foreach(var m in ad.Modules)
            {
                foreach (var type in m.Types)
                {
                    if (type.IsAbstract)
                        continue; 

                    if (!GenUtil.IsHeritedType(type, "Actor"))
                        continue;

                    var attr = GenUtil.GetAttribute<ActorTypeAttribute>(type); 
                    if (attr == null)
                        continue;

                    var attr2 = GenUtil.GetAttribute<NoCodeGenAttribute>(type, true);
                    if (attr2 != null)
                        continue;

                    var at = (int)attr.ConstructorArguments[0].Value;
                    if (isServer && at != (int)AType.SERVER)
                        continue;
                    if (!isServer && at != (int)AType.CLIENT)
                        continue;

                    actorTypes.Add(type);
                }
            }

            GenProtoCode(actorTypes, sharedCPath, sharedSPath, clientPath, serverPath);

            foreach (var type in actorTypes)
                GenFromActorType(type, sharedCPath, sharedSPath, clientPath, serverPath);
        }

        public static void AutogenHost(AssemblyDefinition ad, string sharedPath, string clientPath, string serverPath)
        {
            List<Type> actorTypes = new List<Type>();
            foreach(var m in ad.Modules)
                foreach(var type in m.Types)
                {
                    if (type.IsAbstract)
                        continue;

                    if (type.FullName == "Fenix.Host")
                    {
                        GenFromActorType(type, sharedPath, sharedPath, clientPath, serverPath);
                    }
                } 
        }  
        static string ParseGenericDecl(TypeReference genericType, bool isDynamic=false)
        {
            string decl = "global::" + genericType.FullName.Split('`')[0] + "<";

            foreach (var argType in ((GenericInstanceType)genericType).GenericArguments)
            {
                if (!argType.IsGenericParameter)
                {
                    if (isDynamic)
                    {
                        decl += "dynamic, ";
                    }
                    else
                    {
                        if (argType.Namespace.StartsWith("Server"))
                            decl += "global::" + argType.FullName + ", ";
                        else
                            decl += "global::" + argType.FullName + ", ";
                    }
                }
                else
                    decl += ParseGenericDecl(argType, isDynamic);
            }

            if (decl.EndsWith(", "))
                decl = decl.Substring(0, decl.Length - ", ".Length);
            decl += ">";
            return decl;
        }

        static string ParseDynamicArgsDecl(ParameterDefinition[] paramInfos, bool ignoreCallback)
        {
            List<string> paramList = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.Name.StartsWith("__"))
                    continue;
                if (ignoreCallback)
                    if (p.Name == "callback")
                        continue;
                if (p.HasDefault)
                    paramList.Add(string.Format("{0} {1}", ParseTypeName(p.ParameterType, isDynamic: true), p.Name));
                //paramList.Add(string.Format("{0} {1}={2}", ParseTypeName(p.ParameterType.Resolve()), p.Name, p.));
                else
                    paramList.Add(string.Format("{0} {1}", ParseTypeName(p.ParameterType, isDynamic: true), p.Name));
            }

            var result = string.Join(", ", paramList);
            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            return result;
        }

        static string ParseArgsDecl(ParameterDefinition[] paramInfos, bool ignoreCallback)
        {
            List<string> paramList = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.Name.StartsWith("__"))
                    continue;
                if (ignoreCallback)
                    if (p.Name == "callback")
                        continue;
                if (p.HasDefault)
                    paramList.Add(string.Format("{0} {1}", ParseTypeName(p.ParameterType), p.Name));
                    //paramList.Add(string.Format("{0} {1}={2}", ParseTypeName(p.ParameterType.Resolve()), p.Name, p.));
                else
                    paramList.Add(string.Format("{0} {1}", ParseTypeName(p.ParameterType), p.Name));
            }

            var result = string.Join(", ", paramList);
            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            return result;
        }

        static string ParseArgsType(ParameterDefinition[] paramInfos, bool ignoreCallback)
        {
            List<string> paramList = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.Name.StartsWith("__"))
                    continue;
                if (ignoreCallback)
                    if (p.Name == "callback")
                        continue;
                paramList.Add(string.Format("{0}", ParseTypeName(p.ParameterType)));
            }

            var result = string.Join(", ", paramList);
            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            return result;
        }

        static string ParseTypeName(TypeReference type, bool isDynamic=false)
        {
            if (isDynamic)
                return "dynamic";

            string decl = string.Empty;
            if (type.ContainsGenericParameter)
                decl = ParseGenericDecl(type, isDynamic);
            else
            {
                if (isDynamic)
                    decl = "dynamic";
                else
                {
                    if (type.Namespace.StartsWith("Server"))
                        decl = "global::" + type.FullName;
                    else
                        decl = "global::" + type.FullName;
                }
            }
            decl = decl.Replace("`1<", "<");
            decl = decl.Replace("`2<", "<");
            decl = decl.Replace("`3<", "<");
            decl = decl.Replace("`4<", "<");
            decl = decl.Replace("`5<", "<");
            decl = decl.Replace("`6<", "<");
            decl = decl.Replace("`7<", "<");
            decl = decl.Replace("`8<", "<");
            decl = decl.Replace("`9<", "<");
            decl = decl.Replace("`10<", "<");
            return decl;
        } 

        static string ParseArgs(ParameterDefinition[] paramInfos, string tarInstanceName = "", string exclude = "")
        {
            List<string> paramList = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.Name == exclude)
                    continue;
                if (p.Name.StartsWith("__"))
                    continue;
                if (p.HasDefault)
                    paramList.Add(string.Format("{0}{1}", tarInstanceName, p.Name));
                else
                    paramList.Add(string.Format("{0}{1}", tarInstanceName, p.Name));
            }

            var result = string.Join(", ", paramList);
            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            return result;
        }

        static string ParseArgsMsgAssign(ParameterDefinition[] paramInfos, string prefix, string tarInstanceName = "")
        {
            List<string> paramList = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.Name == "callback")
                    continue;
                if (p.Name.StartsWith("__"))
                    continue;
                if (p.HasDefault)
                    paramList.Add(string.Format("{0}{1}{2}={3}", prefix, tarInstanceName, p.Name, p.Name));
                else
                    paramList.Add(string.Format("{0}{1}{2}={3}", prefix, tarInstanceName, p.Name, p.Name));
            }

            return string.Join(",\n", paramList);
        }

        static string ParseArgsMsgAssign(TypeDefinition[] types, string[] names, string prefix, string tarInstanceName = "")
        {
            List<string> lines = new List<string>();
            for (var i = 0; i < types.Length; ++i)
            {
                var t = types[i];
                int position = i;
                string pType = ParseTypeName(t);
                var attr = GenUtil.GetAttribute<RpcArgAttribute>(t);
                string pName;
                if (attr != null)
                    pName = (string)attr.ConstructorArguments[0].Value;
                else
                    pName = names.Length > position ? names[position] : "arg" + position.ToString();
                lines.Add(string.Format("{0}{1}{2}={3};", prefix, tarInstanceName, pName, pName));
            }

            return string.Join("\n", lines);
        }

        static string ParseMessageFields(ParameterDefinition[] paramInfos, string prefix)
        {
            List<string> lines = new List<string>();
            foreach (var p in paramInfos)
            {
                if (p.Name == "callback")
                    continue;
                if (p.Name.StartsWith("__"))
                    continue;
                int position = p.Index;
                string pType = ParseTypeName(p.ParameterType);
                string pName = p.Name;
                var attr2 = p.ParameterType.Resolve().CustomAttributes.Where(m=>m.AttributeType.Resolve().FullName == typeof(DefaultValueAttribute).FullName).FirstOrDefault() as CustomAttribute;
                if (attr2 != null)
                {
                    var value = (CustomAttributeArgument)attr2.ConstructorArguments[0].Value;
                    bool isEnum = value.Type.Resolve().BaseType.FullName == "System.Enum";
                    if (isEnum)
                    {
                        var inst = value.Type.Resolve();
                        var enumType = value.Type.FullName + "." + inst.Fields.Where(m => m.Constant?.ToString() == value.Value.ToString()).Select(m => m.Name).FirstOrDefault();
                        //var v = isEnum ? ("(global::" + value.Type.FullName + ")(" + value.Value.ToString() + ")") : value.Value.ToString();
                        lines.Add($"{prefix}[Key({position})]\n{prefix}public {pType} {pName} {{ get; set; }} = {enumType};\n");
                    }
                    else
                    {
                        lines.Add($"{prefix}[Key({position})]\n{prefix}public {pType} {pName} {{ get; set; }} = {value.Value.ToString()};\n");
                    }
                }

                else
                    lines.Add($"{prefix}[Key({position})]\n{prefix}public {pType} {pName} {{ get; set; }}\n");
            }

            return string.Join("\n", lines);
        }

        static string ParseMessageFields(TypeDefinition[] types, string[] names, string prefix)
        {
            List<string> lines = new List<string>();
            for (var i = 0; i < types.Length; ++i)
            {
                var t = types[i];
                int position = i;
                string pType = ParseTypeName(t);
                var attr = GenUtil.GetAttribute<RpcArgAttribute>(t);
                string pName;
                string ns = t.Namespace;
                if (attr != null)
                    pName = (string)attr.ConstructorArguments[0].Value;
                else
                    pName = names.Length > position ? names[position] : "arg" + position.ToString();

                var attr2 = t.CustomAttributes.Where(m => m.AttributeType.FullName == typeof(DefaultValueAttribute).FullName).FirstOrDefault();
                if (attr2 != null)
                {
                    var value = (CustomAttributeArgument)attr2.ConstructorArguments[0].Value;
                    bool isEnum = value.Type.Resolve().BaseType.FullName == "System.Enum";
                    if (isEnum)
                    {
                        var inst = value.Type.Resolve();
                        var enumType = value.Type.FullName + "." + inst.Fields.Where(m => m.Constant?.ToString() == value.Value.ToString()).Select(m => m.Name).FirstOrDefault();  
                        //var v = isEnum ? ("(global::" + value.Type.FullName + ")(" + value.Value.ToString() + ")") : value.Value.ToString();
                        lines.Add($"{prefix}[Key({position})]\n{prefix}public {pType} {pName} {{ get; set; }} = {enumType};\n");
                    }
                    else
                    {
                        lines.Add($"{prefix}[Key({position})]\n{prefix}public {pType} {pName} {{ get; set; }} = {value.Value.ToString()};\n");
                    }
                    //lines.Add($"{prefix}[Key({position})]\n{prefix}[DefaultValue({v})]\n{prefix}public {pType} {pName} {{ get; set; }} = {v};\n");
                }
                else
                    lines.Add($"{prefix}[Key({position})]\n{prefix}public {pType} {pName} {{ get; set; }}\n");
            }

            return string.Join("\n", lines);
        }

        static string GenCallbackMsgDecl(ParameterDefinition[] paramInfos, string[] names, string prefix)
        {
            foreach (var p in paramInfos)
            {
                if (p.Name != "callback")
                    continue;
                if (p.Name.StartsWith("__"))
                    continue;
                var t = (GenericInstanceType)p.ParameterType; 
                 
                string code = ParseMessageFields(t.GenericArguments.Select(m=>m.Resolve()).ToArray(), names, prefix + "    ");
                var builder = new StringBuilder()
                    .AppendLine($"        [MessagePackObject]")
                    .AppendLine($"        public class Callback : IMessage")
                    .AppendLine($"        {{")
                    .AppendLine($"{code}")
                    .AppendLine($"            public override byte[] Pack()")
                    .AppendLine($"            {{")
                    .AppendLine($"                return MessagePackSerializer.Serialize<Callback>(this);")
                    .AppendLine($"            }}\n")
                    .AppendLine($"            public new static Callback Deserialize(byte[] data)")
                    .AppendLine($"            {{")
                    .AppendLine($"                return MessagePackSerializer.Deserialize<Callback>(data);")
                    .AppendLine($"            }}\n")
                    .AppendLine($"            public override void UnPack(byte[] data)")
                    .AppendLine($"            {{")
                    .AppendLine($"                var obj = Deserialize(data);")
                    .AppendLine($"                Copier<Callback>.CopyTo(obj, this);")
                    .AppendLine($"            }}")
                    .AppendLine($"        }}");
                return builder.ToString();
            }

            return "";
        }

        static string[] GetCallbackArgs(MethodDefinition method)
        {
            var attr = GenUtil.GetAttribute<CallbackArgsAttribute>(method);
            if (attr == null)
                return new string[] { };
            return attr.Names;
        }

        static void GenProtoCode(List<TypeDefinition> types, string sharedCPath, string sharedSPath, string clientPath, string serverPath)
        {
            foreach (var type in types)
            {
                if (GenUtil.GetAttribute<ActorTypeAttribute>(type) == null)
                    continue; 

                bool isHost = type.Name == "Host";
                var codes = new SortedDictionary<string, uint>();

                foreach (var kv in ParseProtoCode(type, null))
                {
                    codes[kv.Key] = kv.Value;
                }

                ////////////
                // gen sub actormodule
                ////////////

                var rmAttrs = GenUtil.GetAttributes<RequireModuleAttribute>(type);
                foreach(var rmAttr in rmAttrs)
                {  
                    var t = (TypeDefinition)rmAttr.ConstructorArguments[0].Value;

                    //检查是不是有一样的api
                    //有的话，报错

                    //生成module的api
                    foreach (var kv in ParseProtoCode(t, type))
                    {
                        codes[kv.Key] = kv.Value;
                    }
                }

                bool isServer = (int)GenUtil.GetAttribute<ActorTypeAttribute>(type).ConstructorArguments[0].Value == (int)AType.SERVER;

                foreach (var sharedPath in new List<string>() { sharedCPath, sharedSPath })
                {
                    var pPath = Path.Combine(sharedPath, "Protocol");
                    //if (Directory.Exists(pPath))
                    //    Directory.Delete(pPath, false);
                    if (!Directory.Exists(pPath))
                        Directory.CreateDirectory(pPath);

                    //生成客户端msg时，判断一下actor类型的accesstype，如果是serveronly的就不写客户端msg
                    if (!isHost && sharedPath == sharedCPath)
                    {
                        var al = GenUtil.GetAttribute<AccessLevelAttribute>(type);
                        if (al == null)
                            continue;
                        if ((int)al.ConstructorArguments[0].Value == (int)ALevel.SERVER)
                            continue;
                    }

                    using (var sw = new StreamWriter(Path.Combine(sharedPath, 
                            "Protocol",
                            string.Format("ProtocolCode.{0}.{1}.{2}.cs", 
                            type.Namespace,
                            type.Name, 
                            isServer ? "s" : "c")), 
                        false, 
                        Encoding.UTF8))
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
        }

        static SortedDictionary<string, uint> ParseProtoCode(TypeDefinition type, TypeDefinition parentType)
        {
            var codes = new SortedDictionary<string, uint>();

            bool isHost = parentType == null ? type.Name == "Host" : parentType.Name == "Host";
            string tPrefix = parentType==null?null: string.Format("__{0}__{1}__M", parentType.Namespace.Replace(".", ""), parentType.Name);
            var methods = type.Methods;//.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < methods.Count(); ++i)
            {
                MethodDefinition method = methods[i];
                var attr = GenUtil.GetAttribute<ServerApiAttribute>(method);
                if (attr != null)
                {
                    string proto_code =  (isHost ? GenUtil.NameToProtoCode(tPrefix, method.Name) : GenUtil.NameToProtoCode(tPrefix, type.Namespace, type.Name, method.Name)) + "_REQ";
                    uint code = Basic.GenID32FromName(proto_code);
                    codes[proto_code.ToUpper()] = code;
                }
            }

            for (int i = 0; i < methods.Count(); ++i)
            {
                MethodDefinition method = methods[i];
                var attr = GenUtil.GetAttribute<ServerOnlyAttribute>(method);
                if (attr != null)
                {
                    string proto_code = (isHost ? GenUtil.NameToProtoCode(tPrefix, method.Name) : GenUtil.NameToProtoCode(tPrefix, type.Namespace, type.Name, method.Name)) + "_REQ";
                    uint code = Basic.GenID32FromName(proto_code);
                    codes[proto_code.ToUpper()] = code;
                }
            }

            for (int i = 0; i < methods.Count(); ++i)
            {
                MethodDefinition method = methods[i];
                var attr = GenUtil.GetAttribute<ClientApiAttribute>(method);
                if (attr != null)
                {
                    string proto_code = (isHost ? GenUtil.NameToProtoCode(tPrefix, method.Name) : GenUtil.NameToProtoCode(tPrefix, type.Namespace, type.Name, method.Name)) + "_NTF";
                    uint code = Basic.GenID32FromName(proto_code);
                    codes[proto_code.ToUpper()] = code;
                }
            }

            return codes;
        }

    
        static string GenCbArgs(TypeDefinition[] types, string[] names, string instanceName)
        {
            string args = "";

            for (var i = 0; i < types.Length; ++i)
            {
                var t = types[i];
                int position = i;
                string pType = ParseTypeName(t);
                var attr = GenUtil.GetAttribute<RpcArgAttribute>(t);
                string pName;
                if (attr != null)
                    pName = (string)attr.ConstructorArguments[0].Value;
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

        static string GetMessageName(string prefix, string ns, string entityName, string typeName)
        {
            return string.Format("{0}__{1}__{2}__{3}", (prefix == null ? "" : prefix), ns == null ? "" : ns.Replace(".", ""), entityName, typeName);
        }

        static Dictionary<string, SortedDictionary<string, string>> ParseActorType(TypeDefinition type, TypeDefinition parentType, string sharedCPath, string sharedSPath, string clientPath, string serverPath)
        {
            var rpcDefineDic = new SortedDictionary<string, string>();
            var rpcTypeDic = new SortedDictionary<string, string>();
            var apiDefineDic = new SortedDictionary<string, string>();
            var apiNativeDefineDic = new SortedDictionary<string, string>();

            bool isHost = parentType == null ? type.Name == "Host" : parentType.Name == "Host";
            bool isModule = parentType != null;
            string parentTypePrefix = parentType != null ? string.Format("__{0}__{1}__M", parentType.Namespace.Replace(".", ""), parentType.Name) : null;

            var methods = type.Methods;//.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < methods.Count(); ++i)
            {
                MethodDefinition method = methods[i];

                //Console.WriteLine(method.Name);

                var attr = GenUtil.GetAttribute<ServerApiAttribute>(method);
                Api api = Api.NoneApi;
                if (attr != null)
                    api = Api.ServerApi;
                else
                {
                    attr = GenUtil.GetAttribute<ServerOnlyAttribute>(method);
                    if (attr != null)
                        api = Api.ServerOnly;
                    else
                    {
                        attr = GenUtil.GetAttribute<ClientApiAttribute>(method);
                        if (attr != null)
                            api = Api.ClientApi;
                    }
                }

                if (api == Api.ClientApi)
                {
                    if (GenUtil.IsHeritedType(type, "Service"))
                    {
                        Log.Info(string.Format("client_api not allowed in Service {0}", type.Name));
                        continue;
                    }
                }

                if (api != Api.NoneApi)
                {
                    var methodParameterList = method.Parameters.ToList();

                    if (isHost)
                    {
                        var newList = new List<ParameterDefinition>();
                        for (var ii = 0; ii < methodParameterList.Count(); ++ii)
                        {
                            if (ii == methodParameterList.Count() - 1)
                                continue;
                            newList.Add(methodParameterList[ii]);
                        }
                        methodParameterList = newList;
                    }

                    //现在生成message
                    string message_type = GetMessageName(parentTypePrefix, type.Namespace, type.Name, method.Name) + GetApiMessagePostfix(api);

                    if (isHost)
                        message_type = method.Name + GetApiMessagePostfix(api);

                    string message_fields = ParseMessageFields(methodParameterList.ToArray(), "        ");

                    string callback_define = GenCallbackMsgDecl(methodParameterList.ToArray(),
                        GetCallbackArgs(method),
                        "        ");

                    bool hasCallback = methodParameterList.Any(m => m.Name == "callback");

                    string itype = "IMessage";
                    if (hasCallback)
                        itype = "IMessageWithCallback";

                    string proto_code = (isHost ? GenUtil.NameToProtoCode(parentTypePrefix, method.Name) : GenUtil.NameToProtoCode(parentTypePrefix, type.Namespace, type.Name, method.Name)) + "_" + GetApiMessagePostfix(api).ToUpper();
                    uint code = Basic.GenID32FromName(proto_code);

                    string msg_ns = isHost ? "Fenix.Common.Message" : "Shared.Message";

                    string pc_cls = isHost ? "OpCode" : "ProtocolCode";

                    var msgBuilder = new StringBuilder()
                        .AppendLine($"//AUTOGEN, do not modify it!\n")
                        .AppendLine($"using Fenix.Common.Utils;")
                        .AppendLine($"using Fenix.Common;")
                        .AppendLine($"using Fenix.Common.Attributes;")
                        .AppendLine($"using Fenix.Common.Rpc;")
                        .AppendLine($"using MessagePack; ")
                        .AppendLine($"using System.ComponentModel;");

                    if (!isHost)
                    {
                        msgBuilder.AppendLine($"using Shared;")
                        .AppendLine($"using Shared.Protocol;")
                        .AppendLine($"using Shared.DataModel;");
                    }

                    //if(!isHost && isServer)
                    //{
                    //    msgBuilder.AppendLine($"using Server.DataModel;");
                    //}

                    msgBuilder.AppendLine($"using System; ")
                        .AppendLine($"")
                        .AppendLine($"namespace {msg_ns}")
                        .AppendLine($"{{")
                        .AppendLine($"    [MessageType({pc_cls}.{proto_code})]")
                        .AppendLine($"    [MessagePackObject]")
                        .AppendLine($"    public class {message_type} : {itype}")
                        .AppendLine($"    {{")
                        .AppendLine($"{message_fields}");

                    if (hasCallback)
                    {
                        msgBuilder.AppendLine($"        [Key({methodParameterList.Count() - 1})]")
                            .AppendLine(@"
        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 
").AppendLine($"{callback_define}");
                    }
                    msgBuilder.AppendLine($"        public override byte[] Pack()")
                    .AppendLine($"        {{")
                    .AppendLine($"            return MessagePackSerializer.Serialize<{message_type}>(this);")
                    .AppendLine($"        }}\n");
                    msgBuilder.AppendLine($"        public new static {message_type} Deserialize(byte[] data)")
                    .AppendLine($"        {{")
                    .AppendLine($"            return MessagePackSerializer.Deserialize<{message_type}>(data);")
                    .AppendLine($"        }}\n");
                    msgBuilder.AppendLine($"        public override void UnPack(byte[] data)")
                    .AppendLine($"        {{")
                    .AppendLine($"            var obj = Deserialize(data);")
                    .AppendLine($"            Copier<{message_type}>.CopyTo(obj, this);")
                    .AppendLine($"        }}");
                    msgBuilder.AppendLine($"    }}")
                        .AppendLine($"}}");

                    var msgCode = msgBuilder.ToString();
                    foreach (var sharedPath in new List<string>() { sharedCPath, sharedSPath })
                    {
                        var pPath = Path.Combine(sharedPath, "Message");
                        //if(Directory.Exists(pPath))
                        //    Directory.Delete(pPath, false);
                        if (!Directory.Exists(pPath))
                            Directory.CreateDirectory(pPath);

                        //生成客户端msg时，判断一下actor类型的accesstype，如果是serveronly的就不写客户端msg
                        if (!isHost && sharedPath == sharedCPath)
                        {
                            var al = GenUtil.GetAttribute<AccessLevelAttribute>(type);
                            if (al == null)
                            {
                                if (isModule)
                                {
                                    al = GenUtil.GetAttribute<AccessLevelAttribute>(parentType);
                                    if (al == null) continue;
                                }
                                else
                                    continue;
                            }

                            if ((int)al.ConstructorArguments[0].Value == (int)ALevel.SERVER)
                                continue;
                        }

                        if (isHost)
                        {
                            using (var sw = new StreamWriter(Path.Combine(pPath, message_type + ".cs"), false, Encoding.UTF8))
                                sw.WriteLine(msgCode.Replace("\r", ""));
                        }
                        else
                        {
                            using (var sw = new StreamWriter(Path.Combine(pPath, message_type + ".cs"), false, Encoding.UTF8))
                                sw.WriteLine(msgCode.Replace("\r", ""));
                        }
                    }


                    #region GenActorRef
                    //现在生成actor_ref定义 
                    var rpc_name = "rpc_" + GenUtil.NameToApi(method.Name);
                    if (api == Api.ClientApi)
                        rpc_name = "client_" + GenUtil.NameToApi(method.Name);
                    else if (api == Api.ServerOnly)
                        rpc_name = "rpc_" + GenUtil.NameToApi(method.Name);

                    if (isHost)
                        rpc_name = method.Name;

                    string args_decl = ParseArgsDecl(methodParameterList.ToArray(), ignoreCallback: false);
                    string dynamic_args_decl = ParseDynamicArgsDecl(methodParameterList.ToArray(), ignoreCallback: false);
                    string args_type = ParseArgsType(methodParameterList.ToArray(), ignoreCallback: false);
                    //string args_decl_no_cb = ParseArgsDecl(methodParameterList, ignoreCallback: false);

                    string typename = type.Name;
                    string args = ParseArgs(methodParameterList.ToArray());

                    string method_name = method.Name;
                    //string msg_type = method.Name + GetApiMessagePostfix(api);
                    string msg_assign = ParseArgsMsgAssign(methodParameterList.ToArray(), "                    ");

                    /******************************************************************************
                     * Gen Synchronous Version of ActorRef API
                     ******************************************************************************/
                    StringBuilder builder;

                    if (hasCallback)
                    {
                        var cbType = methodParameterList.Where(m => m.Name == "callback").First().ParameterType;
                        var inst = (GenericInstanceType)cbType;
                        string cb_args = GenCbArgs(inst.GenericArguments.Select(m => m.Resolve()).ToArray(), GetCallbackArgs(method), "cbMsg.");

                        builder = new StringBuilder()
                        .AppendLine($"#if FENIX_CODEGEN && !RUNTIME")
                        .AppendLine($"        public void {rpc_name}({dynamic_args_decl})")
                        .AppendLine($"#else")
                        .AppendLine($"        public void {rpc_name}({args_decl})")
                        .AppendLine($"#endif")
                        .AppendLine($"        {{")
                        .AppendLine($"#if !FENIX_CODEGEN")
                        .AppendLine($"            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);")
                        .AppendLine($"            if (this.FromHostId == toHostId)")
                        .AppendLine($"            {{")
                        .AppendLine($"                var protoCode = {pc_cls}.{proto_code};")
                        .AppendLine($"                if (protoCode < OpCode.CALL_ACTOR_METHOD)")
                        .AppendLine($"                {{")
                        .AppendLine($"                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);")
                        .AppendLine($"                    var context = new RpcContext(null, peer);");
                        if (args.Trim().Length == 0)
                            builder.AppendLine($"                    Global.Host.CallMethodWithParams(protoCode, new object[] {{ context }});");
                        else
                            builder.AppendLine($"                    Global.Host.CallMethodWithParams(protoCode, new object[] {{ {args}, context }});");
                        builder.AppendLine($"                }}")
                        .AppendLine($"                else")
                        .AppendLine($"                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] {{ {args} }});")
                        //.AppendLine($"                Global.Host.GetActor(this.toActorId).CallLocalMethod({pc_cls}.{proto_code}, new object[] {{ {args} }});")
                        //.AppendLine($"                (({typename})Global.Host.GetActor(this.toActorId)).{method_name}({args});")
                        .AppendLine($"                return;")
                        .AppendLine($"            }}")
                        .AppendLine($"            Task.Run(() => {{")
                        .AppendLine($"                var msg = new {message_type}()")
                        .AppendLine($"                {{")
                        .AppendLine($"{msg_assign}")
                        .AppendLine($"                }};")
                        .AppendLine($"                var cb = new Action<byte[]>((cbData) => {{")
                        .AppendLine($"                    var cbMsg = cbData==null?new {message_type}.Callback():global::Fenix.Common.Utils.RpcUtil.Deserialize<{message_type}.Callback>(cbData);")
                        .AppendLine($"                    callback?.Invoke({cb_args});")
                        .AppendLine($"                }});")
                        .AppendLine($"                this.CallRemoteMethod({pc_cls}.{proto_code}, msg, cb);")
                        .AppendLine($"            }});")
                        .AppendLine($"#endif")
                        .AppendLine($"        }}");
                    }
                    else
                    {
                        builder = new StringBuilder()
                        .AppendLine($"#if FENIX_CODEGEN && !RUNTIME")
                        .AppendLine($"        public void {rpc_name}({dynamic_args_decl})")
                        .AppendLine($"#else")
                        .AppendLine($"        public void {rpc_name}({args_decl})")
                        .AppendLine($"#endif")
                        .AppendLine($"        {{")
                        .AppendLine($"#if !FENIX_CODEGEN")
                        .AppendLine($"           var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);")
                        .AppendLine($"           if (this.FromHostId == toHostId)")
                        .AppendLine($"           {{")
                        .AppendLine($"                var protoCode = {pc_cls}.{proto_code};")
                        .AppendLine($"                if (protoCode < OpCode.CALL_ACTOR_METHOD)")
                        .AppendLine($"                {{")
                        .AppendLine($"                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);")
                        .AppendLine($"                    var context = new RpcContext(null, peer);");
                        if (args.Trim().Length == 0)
                            builder.AppendLine($"                    Global.Host.CallMethodWithParams(protoCode, new object[] {{ context }});");
                        else
                            builder.AppendLine($"                    Global.Host.CallMethodWithParams(protoCode, new object[] {{ {args}, context }});");
                        builder.AppendLine($"                }}")
                        .AppendLine($"                else")
                        .AppendLine($"                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] {{ {args} }}); ")
                        //.AppendLine($"                Global.Host.GetActor(this.toActorId).CallLocalMethod({pc_cls}.{proto_code}, new object[] {{ {args} }});")
                        //.AppendLine($"                (({typename})Global.Host.GetActor(this.toActorId)).{method_name}({args});")
                        .AppendLine($"               return;")
                        .AppendLine($"           }}")
                        .AppendLine($"           Task.Run(() => {{")
                        .AppendLine($"               var msg = new {message_type}()")
                        .AppendLine($"               {{")
                        .AppendLine($"{msg_assign}")
                        .AppendLine($"               }};")
                        .AppendLine($"               this.CallRemoteMethod({pc_cls}.{proto_code}, msg, null);")
                        .AppendLine($"            }});")
                        .AppendLine($"#endif")
                        .AppendLine($"        }}");
                    }

                    var rpcSyncDefineCode = builder.ToString();

                    /******************************************************************************
                     * Gen Asynchronous Version of ActorRef API
                     ******************************************************************************/
                    if (hasCallback)
                    {
                        var cbType = methodParameterList.Where(m => m.Name == "callback").First().ParameterType;
                        var inst = (GenericInstanceType)cbType;

                        string cb_args = GenCbArgs(inst.GenericArguments.Select(m => m.Resolve()).ToArray(), GetCallbackArgs(method), "cbMsg.");
                        string cb_pure_args = GenCbArgs(inst.GenericArguments.Select(m => m.Resolve()).ToArray(), GetCallbackArgs(method), "");
                        string cb_types = ParseTypeName(cbType);
                        string api_cb_assign = ParseArgsMsgAssign(inst.GenericArguments.Select(m => m.Resolve()).ToArray(),
                                                        GetCallbackArgs(method),
                                                        "                     ",
                                                        "cbMsg.");
                        var async_args = args.Replace("callback", "_cb");
                        var async_msg_assign = ParseArgsMsgAssign(methodParameterList.ToArray(), "                         ");
                        builder = new StringBuilder();
                        if (isHost)
                        {
                            builder.AppendLine($"#if FENIX_CODEGEN && !RUNTIME");
                            builder.AppendLine($"        public async Task<dynamic> {rpc_name}Async({dynamic_args_decl}=null)");
                            builder.AppendLine($"#else");
                            builder.AppendLine($"        public async Task<{message_type}.Callback> {rpc_name}Async({args_decl}=null)");
                            builder.AppendLine($"#endif");
                        }
                        else
                        {
                            builder.AppendLine($"#if FENIX_CODEGEN && !RUNTIME");
                            builder.AppendLine($"        public async Task<dynamic> {rpc_name}_async({dynamic_args_decl}=null)");
                            builder.AppendLine($"#else");
                            builder.AppendLine($"        public async Task<{message_type}.Callback> {rpc_name}_async({args_decl}=null)");
                            builder.AppendLine($"#endif");
                        }
                        builder.AppendLine($"        {{")
                        .AppendLine($"#if FENIX_CODEGEN")
                        .AppendLine($"#if !RUNTIME")
                        .AppendLine($"            var t = new TaskCompletionSource<dynamic>();")
                        .AppendLine($"#else") 
                        .AppendLine($"            var t = new TaskCompletionSource<{message_type}.Callback>();")
                        .AppendLine($"#endif")
                        .AppendLine($"#else")
                        .AppendLine($"            var t = new TaskCompletionSource<{message_type}.Callback>();")

                        .AppendLine($"            var toHostId = Global.IdManager.GetHostIdByActorId(this.toActorId, this.isClient);")
                        .AppendLine($"            if (this.FromHostId == toHostId)")
                        .AppendLine($"            {{")
                        .AppendLine($"                {cb_types} _cb = ({cb_pure_args}) =>")
                        .AppendLine($"                {{")
                        .AppendLine($"                     var cbMsg = new {message_type}.Callback();")
                        .AppendLine($"{api_cb_assign}")
                        .AppendLine($"                     callback?.Invoke({cb_args});")
                        .AppendLine($"                     t.TrySetResult(cbMsg);")
                        .AppendLine($"                }}; ")
                        .AppendLine($"                var protoCode = {pc_cls}.{proto_code};")
                        .AppendLine($"                if (protoCode < OpCode.CALL_ACTOR_METHOD)")
                        .AppendLine($"                {{")
                        .AppendLine($"                    var peer = Global.NetManager.GetRemotePeerById(this.FromHostId, this.NetType);")
                        .AppendLine($"                    var context = new RpcContext(null, peer);");
                        if (async_args.Trim().Length == 0)
                            builder.AppendLine($"                    Global.Host.CallMethodWithParams(protoCode, new object[] {{ context }});");
                        else
                            builder.AppendLine($"                    Global.Host.CallMethodWithParams(protoCode, new object[] {{ {async_args}, context }});");
                        builder.AppendLine($"                }}")
                        .AppendLine($"                else")
                        .AppendLine($"                    Global.Host.GetActor(this.toActorId).CallMethodWithParams(protoCode, new object[] {{ {async_args} }});")
                        .AppendLine($"            }}")
                        .AppendLine($"            else")
                        .AppendLine($"            {{")
                        .AppendLine($"                Action<{message_type}.Callback> _cb = (cbMsg) =>")
                        .AppendLine($"                {{")
                        .AppendLine($"                    callback?.Invoke({cb_args});")
                        .AppendLine($"                    t.TrySetResult(cbMsg);")
                        .AppendLine($"                }};")
                        .AppendLine($"                await Task.Run(() => {{")
                        .AppendLine($"                    var msg = new {message_type}()")
                        .AppendLine($"                    {{")
                        .AppendLine($"{async_msg_assign}")
                        .AppendLine($"                    }};")
                        .AppendLine($"                    var cb = new Action<byte[]>((cbData) => {{")
                        .AppendLine($"                        var cbMsg = cbData==null ? new {message_type}.Callback() : global::Fenix.Common.Utils.RpcUtil.Deserialize<{message_type}.Callback>(cbData);")
                        .AppendLine($"                        _cb?.Invoke(cbMsg);")
                        .AppendLine($"                    }});")
                        .AppendLine($"                    this.CallRemoteMethod({pc_cls}.{proto_code}, msg, cb);")
                        .AppendLine($"                 }});")
                        .AppendLine($"             }}")
                        .AppendLine($"#endif")
                        .AppendLine($"             return await t.Task;")
                        .AppendLine($"        }}");
                    }
                    else
                    {
                        builder = new StringBuilder();
                    }

                    var rpcAsyncDefineCode = builder.ToString();

                    string api_rpc_args = ParseArgs(methodParameterList.ToArray(), "_msg.", "callback");
                    string api_type = "ServerApi";
                    string ns = type.Namespace == null ? "" : type.Namespace;
                    string api_name = "SERVER_API" + (parentTypePrefix == null ? "__" : parentTypePrefix + "__") + ns.Replace(".", "") + "__" + type.Name + "__" + GenUtil.NameToApi(method.Name);
                    if (api == Api.ClientApi)
                    {
                        api_name = "CLIENT_API" + (parentTypePrefix == null ? "__" : parentTypePrefix + "__") + ns.Replace(".", "") + "__" + type.Name + "__" + GenUtil.NameToApi(method.Name);
                        api_type = "ClientApi";
                    }
                    else if (api == Api.ServerOnly)
                    {
                        api_name = "SERVER_ONLY" + (parentTypePrefix == null ? "__" : parentTypePrefix + "__") + ns.Replace(".", "") + "__" + type.Name + "__" + GenUtil.NameToApi(method.Name);
                        api_type = "ServerOnly";
                    }

                    rpcDefineDic[rpc_name] = rpcAsyncDefineCode + "\n" + rpcSyncDefineCode;
                    rpcTypeDic[rpc_name] = api_type;
                    #endregion

                    bool hasEvent = api_type == "ClientApi" && method.Name.ToLower().StartsWith("on") && method.Name.Length > 2 && method.Name[3] >= 'A';

                    builder = new StringBuilder()
                        .AppendLine($"        [RpcMethod({pc_cls}.{proto_code}, Api.{api_type})]");
                        //.AppendLine($"        [EditorBrowsable(EditorBrowsableState.Never)]");
                    if (isHost)
                    {
                        if (hasCallback)
                            builder.AppendLine($"        public void {api_name}(IMessage msg, Action<IMessage> cb, RpcContext context)");
                        else
                            builder.AppendLine($"        public void {api_name}(IMessage msg, RpcContext context)");
                    }
                    else
                    {
                        if (hasCallback)
                            builder.AppendLine($"        public void {api_name}(IMessage msg, Action<IMessage> cb)");
                        else
                            builder.AppendLine($"        public void {api_name}(IMessage msg)");
                    }


                    //builder.AppendLine($"        {{");
                        //.AppendLine($"            var _msg = ({message_type})msg;");

                    var fileterArgs = api_rpc_args != "" ? api_rpc_args + "," : "";

                    string selfName = isModule ? string.Format("this.GetModule<{0}>()", type.Name) : "this";

                    if (hasCallback)
                    {
                        var cbType2 = methodParameterList.Where(m => m.Name == "callback").First().ParameterType;
                        var inst2 = (GenericInstanceType)cbType2;
                        string api_cb_args = GenCbArgs(inst2.GenericArguments.Select(m => m.Resolve()).ToArray(), GetCallbackArgs(method), "");
                        string cb_types = ParseTypeName(cbType2);
                        string dynamic_cb_args = cb_types;/// String.Concat(Enumerable.Repeat("dynamic, ", inst2.GenericArguments.Count()));
                        if (dynamic_cb_args.EndsWith(", "))
                            dynamic_cb_args = dynamic_cb_args.Substring(0, dynamic_cb_args.Length - ", ".Length);
                        string api_cb_assign = ParseArgsMsgAssign(inst2.GenericArguments.Select(m => m.Resolve()).ToArray(),
                                                        GetCallbackArgs(method),
                                                        "                ",
                                                        "cbMsg.");

                        builder.AppendLine($"        {{");
                        builder.AppendLine("#if ENABLE_IL2CPP || !DEBUG || RUNTIME");
                        {
                            builder.AppendLine($"            var _msg = ({message_type})msg;"); 
                            builder.AppendLine($"            {selfName}.{method.Name}({fileterArgs} ({api_cb_args}) =>")
                            .AppendLine($"            {{")
                            .AppendLine($"                var cbMsg = new {message_type}.Callback();")
                            .AppendLine($"{api_cb_assign}")
                            .AppendLine($"                cb.Invoke(cbMsg);");

                            if (isHost)
                                builder.AppendLine($"            }}, context);");
                            else
                                builder.AppendLine($"            }});");

                            if (hasEvent)
                            {
                                builder.AppendLine($"            {GenUtil.NameToApi(method.Name)}?.Invoke({fileterArgs} ({api_cb_args}) =>")
                                .AppendLine($"            {{")
                                .AppendLine($"                dynamic cbMsg = new {message_type}.Callback();")
                                .AppendLine($"{api_cb_assign}")
                                .AppendLine($"                cb.Invoke(cbMsg);")
                                .AppendLine($"            }});");
                            }
                        }
                        builder.AppendLine("#else\n");
                        {
                            builder.AppendLine($"            dynamic _msg = msg;");

                            builder.AppendLine($"            {selfName.Replace("this", "self")}.{method.Name}({fileterArgs} ({dynamic_cb_args})(({api_cb_args}) =>")
                                .AppendLine($"            {{")
                                .AppendLine($"                var cbMsg = new {message_type}.Callback();")
                                .AppendLine($"{api_cb_assign}")
                                .AppendLine($"                cb.Invoke(cbMsg);");

                            if (isHost)
                                builder.AppendLine($"            }}), context);");
                            else
                                builder.AppendLine($"            }}));");

                            if (hasEvent)
                            {
                                builder.AppendLine($"            {GenUtil.NameToApi(method.Name)}?.Invoke({fileterArgs} ({dynamic_cb_args})(({api_cb_args}) =>")
                                .AppendLine($"            {{")
                                .AppendLine($"                var cbMsg = new {message_type}.Callback();")
                                .AppendLine($"{api_cb_assign}")
                                .AppendLine($"                cb.Invoke(cbMsg);")
                                .AppendLine($"            }}));");
                            }
                        }
                        builder.AppendLine("#endif\n");
                    }
                    else
                    {
                        builder.AppendLine($"        {{");
                        builder.AppendLine($"#if ENABLE_IL2CPP || !DEBUG || RUNTIME");

                        builder.AppendLine($"            var _msg = ({message_type})msg;");
                        if (isHost)
                            builder.AppendLine($"            {selfName}.{method.Name}({fileterArgs} context);");
                        else
                            builder.AppendLine($"            {selfName}.{method.Name}({api_rpc_args});");

                        if (hasEvent)
                            builder.AppendLine($"            {GenUtil.NameToApi(method.Name)}?.Invoke({api_rpc_args});");

                        builder.AppendLine($"#else");

                        builder.AppendLine($"            dynamic _msg = msg;");
                        if (isHost)
                            builder.AppendLine($"            {selfName.Replace("this", "self")}.{method.Name}({fileterArgs} context);");
                        else
                            builder.AppendLine($"            {selfName.Replace("this", "self")}.{method.Name}({api_rpc_args});");

                        if (hasEvent)
                            builder.AppendLine($"            {GenUtil.NameToApi(method.Name)}?.Invoke({api_rpc_args});");

                        builder.AppendLine($"#endif");
                    }

                    builder.AppendLine($"        }}");

                    var apiDefineCode = builder.ToString().Replace("Action<>", "Action"); ;
                    apiDefineDic[api_name] = apiDefineCode;

                    api_type = "ServerApi";
                    api_name = "SERVER_API_NATIVE" + (parentTypePrefix == null ? "__" : parentTypePrefix + "__") + ns.Replace(".", "") + "__" + type.Name + "__" + GenUtil.NameToApi(method.Name);
                    if (api == Api.ClientApi)
                    {
                        api_name = "CLIENT_API_NATIVE" + (parentTypePrefix == null ? "__" : parentTypePrefix + "__") + ns.Replace(".", "") + "__" + type.Name + "__" + GenUtil.NameToApi(method.Name);
                        api_type = "ClientApi";
                    }
                    else if (api == Api.ServerOnly)
                    {
                        api_name = "SERVER_ONLY_NATIVE" + (parentTypePrefix == null ? "__" : parentTypePrefix + "__") + ns.Replace(".", "") + "__" + type.Name + "__" + GenUtil.NameToApi(method.Name);
                        api_type = "ServerOnly";
                    }

                    builder = new StringBuilder();

                    if (hasEvent)
                    {
                        //if (hasCallback)
                        //{
                        //    var cbType = methodParameterList.Where(m => m.Name == "callback").First().ParameterType; 
                        //    string cb_types = ParseTypeName(cbType);
                        //    builder.AppendLine($"        public event {args_decl} {GenUtil.NameToApi(method.Name)};");
                        //}
                        //else
                        //{
                        if (args_type != "")
                            builder.AppendLine($"        public event Action<{args_type}> {GenUtil.NameToApi(method.Name)};");
                        else
                            builder.AppendLine($"        public event Action {GenUtil.NameToApi(method.Name)};");
                        //}
                    }

                    builder.AppendLine($"        [RpcMethod({pc_cls}.{proto_code}, Api.{api_type})]");
                        //.AppendLine($"        [EditorBrowsable(EditorBrowsableState.Never)]");

                    if (isHost)
                    {
                        if (hasCallback)
                            builder.AppendLine($"        public void {api_name}({args_decl}, RpcContext context)");
                        else
                            builder.AppendLine($"        public void {api_name}({args_decl}, RpcContext context)");
                    }
                    else
                    {
                        if (hasCallback)
                            builder.AppendLine($"        public void {api_name}({args_decl})");
                        else
                            builder.AppendLine($"        public void {api_name}({args_decl})");
                    }

                    if (hasCallback)
                    {
                        //var cbType2 = methodParameterList.Where(m => m.Name == "callback").First().ParameterType;
                        //string api_cb_args = GenCbArgs(cbType2.GetGenericArguments(), GetCallbackArgs(method), "");

                        builder.AppendLine($"        {{");

                        builder.AppendLine("#if ENABLE_IL2CPP || !DEBUG || RUNTIME");
                        {
                            if (isHost)
                                builder.AppendLine($"            {selfName}.{method.Name}({args}, context);");
                            else
                                builder.AppendLine($"            {selfName}.{method.Name}({args});");

                            if (hasEvent)
                                builder.AppendLine($"            {GenUtil.NameToApi(method.Name)}?.Invoke({args});");
                        }
                        builder.AppendLine("#else");
                        {
                            if (isHost)
                                builder.AppendLine($"            {selfName.Replace("this", "self")}.{method.Name}({args}, context);");
                            else
                                builder.AppendLine($"            {selfName.Replace("this", "self")}.{method.Name}({args});");

                            if (hasEvent)
                                builder.AppendLine($"            {GenUtil.NameToApi(method.Name)}?.Invoke({args});");
                        }
                        builder.AppendLine("#endif");
                    }
                    else
                    { 
                        builder.AppendLine($"        {{");
                        builder.AppendLine("#if ENABLE_IL2CPP || !DEBUG || RUNTIME");
                        {
                            if (isHost)
                                builder.AppendLine($"            {selfName}.{method.Name}({args}, context);");
                            else
                                builder.AppendLine($"            {selfName}.{method.Name}({args});");

                            if (hasEvent)
                                builder.AppendLine($"            {GenUtil.NameToApi(method.Name)}?.Invoke({args});");
                        }
                        builder.AppendLine("#else");
                        {
                            if (isHost)
                                builder.AppendLine($"            {selfName.Replace("this", "self")}.{method.Name}({args}, context);");
                            else
                                builder.AppendLine($"            {selfName.Replace("this", "self")}.{method.Name}({args});");

                            if (hasEvent)
                                builder.AppendLine($"            {GenUtil.NameToApi(method.Name)}?.Invoke({args});");
                        }
                        builder.AppendLine("#endif");
                    }

                    builder.AppendLine($"        }}");

                    var apiNativeDefineCode = builder.ToString().Replace("Action<>", "Action"); ;
                    apiNativeDefineDic[api_name] = apiNativeDefineCode;
                }
            }

            return new Dictionary<string, SortedDictionary<string, string>>()
            {
                { "rpcDefineDic", rpcDefineDic},
                { "rpcTypeDic", rpcTypeDic},
                { "apiDefineDic", apiDefineDic},
                { "apiNativeDefineDic", apiNativeDefineDic}
            };
        }

        static void GenFromActorType(TypeDefinition type, string sharedCPath, string sharedSPath, string clientPath, string serverPath)
        {
            var rpcDefineDic = new SortedDictionary<string, string>();
            var rpcTypeDic = new SortedDictionary<string, string>();
            var apiDefineDic = new SortedDictionary<string, string>();
            var apiNativeDefineDic = new SortedDictionary<string, string>();

            if (GenUtil.GetAttribute<ActorTypeAttribute>(type) == null && type.Name != "Host")
                return;

            Log.Info("Gen", type.FullName);

            bool isServer = type.Name == "Host" ? true : ((int)GenUtil.GetAttribute<ActorTypeAttribute>(type).ConstructorArguments[0].Value == (int)AType.SERVER);
            
            bool isHost = type.Name == "Host";

            var r = ParseActorType(type, null, sharedCPath, sharedSPath, clientPath, serverPath);

            foreach (var kv in r["rpcDefineDic"]) rpcDefineDic[kv.Key] = kv.Value;
            foreach (var kv in r["rpcTypeDic"]) rpcTypeDic[kv.Key] = kv.Value;
            foreach (var kv in r["apiDefineDic"]) apiDefineDic[kv.Key] = kv.Value;
            foreach (var kv in r["apiNativeDefineDic"]) apiNativeDefineDic[kv.Key] = kv.Value;

            ////////////
            // gen sub actormodule
            ////////////

            var rmAttrs = GenUtil.GetAttributes<RequireModuleAttribute>(type);
            foreach (var rmAttr in rmAttrs)
            {
                var t = (TypeDefinition)rmAttr.ConstructorArguments[0].Value;

                //检查是不是有一样的api
                //有的话，报错 
                //生成module的api
                var r2 = ParseActorType(t, type, sharedCPath, sharedSPath, clientPath, serverPath);
                foreach (var kv in r2["rpcDefineDic"]) rpcDefineDic[kv.Key] = kv.Value;
                foreach (var kv in r2["rpcTypeDic"]) rpcTypeDic[kv.Key] = kv.Value;
                foreach (var kv in r2["apiDefineDic"]) apiDefineDic[kv.Key] = kv.Value;
                foreach (var kv in r2["apiNativeDefineDic"]) apiNativeDefineDic[kv.Key] = kv.Value;
            }

            string refCode = string.Join("\n", rpcDefineDic.Values);

            string clientRefCode = string.Join("\n", rpcDefineDic.Where(m => rpcTypeDic[m.Key].StartsWith("ServerApi")).Select(m => m.Value).ToList());
            string serverRefCode = string.Join("\n", rpcDefineDic.Values);

            string tname = type.Name;
            string ns = type.Namespace;

            var alAttr = GenUtil.GetAttribute<AccessLevelAttribute>(type);
            if (alAttr == null && !isHost)
            {
                Log.Info(string.Format("ERROR: {0} has no AccessLevel", type.Name));
                return;
            }

            string root_ns = isHost ? "Fenix" : (isServer ? "Server" : "Client");

            string refTypeName = tname == "Avatar" ? type.FullName : tname;

            var refBuilder = new StringBuilder()
                .AppendLine(@"
//AUTOGEN, do not modify it!

using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Utils;
using Fenix.Common.Message;

");
            if (!isHost)
            {
                refBuilder.AppendLine(@"using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;
                    ");
            }

            //            if (isServer && !isHost)
            //            {
            //                refBuilder.AppendLine(@"using Server.DataModel;
            //");
            //            }

            refBuilder.AppendLine(@"//using MessagePack;
using System;
using System.Threading.Tasks;
")
            .AppendLine($"namespace {root_ns}")
            .AppendLine(@"{
");
            if (!isHost)
            {
                refBuilder.AppendLine($"    [RefType(\"{refTypeName}\")]")
                    .AppendLine($"    public partial class {tname}Ref : ActorRef");
            }
            else
            {
                refBuilder.AppendLine($"    public partial class ActorRef");
            }

            string isClientStr = isServer ? "false" : "true";

            if (!isHost)
            {
                refBuilder.AppendLine($"    {{")
                    //.AppendLine($"        public {tname}Ref() {{}}")
                    .AppendLine($"        public new bool isClient => {isClientStr};")
                    .AppendLine($"{refCode}    }}")
                    .AppendLine($"}}");
                var result = refBuilder.ToString();
                foreach (var sharedPath in new List<string>() { sharedCPath, sharedSPath })
                {
                    var pPath = Path.Combine(sharedPath, "ActorRef", root_ns); 
                    if (!Directory.Exists(pPath))
                        Directory.CreateDirectory(pPath);

                    //生成客户端msg时，判断一下actor类型的accesstype，如果是serveronly的就不写客户端msg
                    if (!isHost && sharedPath == sharedCPath)
                    {
                        var al = GenUtil.GetAttribute<AccessLevelAttribute>(type);
                        if (al == null)
                            continue; 
                        if ((int)al.ConstructorArguments[0].Value == (int)ALevel.SERVER)
                            continue;
                    }

                    using (var sw = new StreamWriter(Path.Combine(pPath, type.Name + "Ref.cs"), false, Encoding.UTF8))
                    {
                        sw.WriteLine(result);
                    }
                }
            }
            else
            {
                var preludePart = refBuilder.ToString();
                var clientCode = "#if CLIENT\n" + preludePart + string.Format("    {{\n{0}    }}\n}}",
                    /*string.Format("        public new bool isClient => {0};", !isServer),*/ clientRefCode) + "\n#endif\n";
                var serverCode = "#if !CLIENT\n" + preludePart + string.Format("    {{\n{0}    }}\n}}",
                    /*string.Format("        public new bool isClient => {0};", !isServer),*/ serverRefCode) + "\n#endif\n";

                if (!Directory.Exists(Path.Combine(sharedCPath, "../Actor")))
                    Directory.CreateDirectory(Path.Combine(sharedCPath, "../Actor"));

                using (var sw = new StreamWriter(Path.Combine(sharedCPath, "../Actor", "ActorRef.Client.cs"), false, Encoding.UTF8)) 
                    sw.WriteLine(clientCode); 

                using (var sw = new StreamWriter(Path.Combine(sharedCPath, "../Actor", "ActorRef.Server.cs"), false, Encoding.UTF8)) 
                    sw.WriteLine(serverCode); 
            }

            string internalClientApiCode = string.Join("\n", apiDefineDic.Where(m => m.Key.StartsWith("CLIENT_API")).Select(m => m.Value).ToList());
            string internalClientNativeApiCode = string.Join("\n", apiNativeDefineDic.Where(m => m.Key.StartsWith("CLIENT_API")).Select(m => m.Value).ToList());

            string internalServerApiCode = string.Join("\n", apiDefineDic.Where(m => !m.Key.StartsWith("CLIENT_API")).Select(m => m.Value).ToList());
            string internalServerNativeApiCode = string.Join("\n", apiNativeDefineDic.Where(m => !m.Key.StartsWith("CLIENT_API")).Select(m => m.Value).ToList());

            var apiBuilder = new StringBuilder()
                .AppendLine(@"
//AUTOGEN, do not modify it!

using Fenix.Common;
using Fenix.Common.Attributes;
using Fenix.Common.Message;
using Fenix.Common.Rpc;
using Fenix.Common.Utils;");
            if (!isHost)
            {
                apiBuilder.AppendLine(@"
using Shared;
using Shared.DataModel;
using Shared.Protocol; 
using Shared.Message;");
            }
            if (isServer && !isHost)
            {
                apiBuilder.AppendLine(@"using Server.DataModel;
");
            }
            apiBuilder.AppendLine(@"
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

");

            apiBuilder.AppendLine($"namespace {ns}")
            .AppendLine($"{{")
            .AppendLine($"    public partial class {tname}")
            .AppendLine($"    {{")
            .AppendLine($"#if !FENIX_CODEGEN")
            .AppendLine(@"#if CLIENT")
            .AppendLine($"{internalClientApiCode}")
            .AppendLine($"{internalClientNativeApiCode}")
            .AppendLine(@"#endif")
            .AppendLine(@"#if !CLIENT")
            .AppendLine($"{internalServerApiCode}")
            .AppendLine($"{internalServerNativeApiCode}")
            .AppendLine($"#endif")
            .AppendLine($"#endif")
            .AppendLine($"    }}")
            .AppendLine($"}}");
            var apiResultCode = apiBuilder.ToString();
            if (isHost)
            {
                var stubPath = Path.Combine(sharedCPath, "Stub");
                //if(Directory.Exists(stubPath))
                //    Directory.Delete(stubPath, true);
                if (!Directory.Exists(stubPath))
                    Directory.CreateDirectory(stubPath);
                using (var sw = new StreamWriter(Path.Combine(stubPath, type.Name + ".Stub.cs"), false, Encoding.UTF8))
                {
                    sw.WriteLine(apiResultCode);
                }
            }
            else
            {
                string output = isServer ? serverPath : clientPath;

                var stubPath = Path.Combine(output, "Gen", "Stub");
                //if (Directory.Exists(stubPath))
                //    Directory.Delete(stubPath, true);

                if (!Directory.Exists(stubPath))
                    Directory.CreateDirectory(stubPath);

                using (var sw = new StreamWriter(Path.Combine(stubPath, type.Name + ".Stub.cs"), false, Encoding.UTF8))
                {
                    sw.WriteLine(apiResultCode);
                }
            }
        }
    }
}