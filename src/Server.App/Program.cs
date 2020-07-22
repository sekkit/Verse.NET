
/*
 * (c)2020 Sekkit.com
 * Fenix是一个基于Actor网络模型的分布式游戏服务器
 * server端通信都是走tcp
 * server/client之间可以走tcp/kcp/websockets
 */

using DotNetty.Buffers;
using Fenix;
using Fenix.Config;
using Fenix.Host;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Server.App
{ 
    class Program
    { 
        static void Main(string[] args)
        {
            /*
            using (StreamWriter sw = new StreamWriter("app.json", false, Encoding.UTF8))
            {
                var content = JsonConvert.SerializeObject(conf, Formatting.Indented);
                sw.Write(content);
            }
            */
             
            if (args.Length == 0)
            {
                var cfgList = new List<RuntimeConfig>();

                var obj = new RuntimeConfig();
                obj.ExternalIp = "auto";
                obj.InternalIp = "auto";
                obj.Port = 17777; //auto
                obj.AppName = "Account.App";
                obj.DefaultActorNames = new List<string>()
                {
                    "AccountService"
                };

                cfgList.Add(obj);

                obj = new RuntimeConfig();
                obj.ExternalIp = "auto";
                obj.InternalIp = "auto";
                obj.Port = 17778; //auto
                obj.AppName = "Match.App";
                obj.DefaultActorNames = new List<string>()
                {
                    "MatchService"
                };

                cfgList.Add(obj);

                Environment.SetEnvironmentVariable("AppName", "Account.App");

                Bootstrap.Start(new Assembly[] { typeof(Program).Assembly }, cfgList, isMultiProcess:true); //单进程模式
            }
            else
            { 
                var builder = new ConfigurationBuilder().AddCommandLine(args);
                var cmdLine = builder.Build();
                
                //if(cmdLine["autogen"] != null)
                //{ 
                    
                //}
                //else
                //{
                    //将命令行参数，设置到进程的环境变量
                    Environment.SetEnvironmentVariable("AppName", cmdLine["AppName"]);

                    using (var sr = new StreamReader(cmdLine["Config"]))
                    {
                        var cfgList = JsonConvert.DeserializeObject<List<RuntimeConfig>>(sr.ReadToEnd());
                        Bootstrap.Start(new Assembly[] { typeof(Program).Assembly }, cfgList, isMultiProcess: true); //分布式
                    }
                //} 
            }
        }
    }
}
