using Fenix;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.App
{
    [Serializable]
    public class B
    {

    }

    [Serializable]
    //[MessagePack.MessagePackObject]
    public class A
    {
        //[Key(0)]
        //[DataMember]
        //public object a;

        //[Key(1)]
        [DataMember]
        public Dictionary<string, object> hello = new Dictionary<string, object>();

        public A()
        {
            hello[typeof(A).Name] = new B();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var act = new A();
            MemoryStream s = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, act);
            s.Close();

            var bytes = s.ToArray(); 

            ////var bytes = MessagePack.MessagePackSerializer.Serialize(act);
            //Console.WriteLine(DotNetty.Common.Utilities.StringUtil.ToHexString(bytes)); 
            ////act.a = new object();
            //bytes = MessagePack.MessagePackSerializer.Serialize(act);
            //Console.WriteLine(DotNetty.Common.Utilities.StringUtil.ToHexString(bytes));

            //var act2 = new A();
            //var bytes2 = MessagePack.MessagePackSerializer.Serialize(act2);
            //Console.WriteLine(DotNetty.Common.Utilities.StringUtil.ToHexString(bytes2));

            //var act3 =  MessagePack.MessagePackSerializer.Deserialize<A>(bytes);
            var act3 = (A)b.Deserialize(new MemoryStream(bytes));
            Console.WriteLine(act3.hello);


            //Bootstrap.Start();
        }
    }
}
