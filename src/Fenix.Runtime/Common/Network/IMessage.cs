using Fenix.Common.Utils;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Fenix.Common.Rpc
{
    public static class Copier<T>
    {
        private static readonly Action<T, T> _copier;

        static Copier()
        {
            var x = Expression.Parameter(typeof(T), "x");
            var y = Expression.Parameter(typeof(T), "y");
            var expressions = new List<Expression>();
            foreach (var property in typeof(T).GetProperties())
            {
                var attrs = property.GetCustomAttributes(typeof(KeyAttribute), true);
                if (attrs.Length == 0)
                    continue;
                if (property.CanWrite)
                {
                    var xProp = Expression.Property(x, property);
                    var yProp = Expression.Property(y, property);
                    expressions.Add(Expression.Assign(yProp, xProp));
                }
            }

            foreach (var field in typeof(T).GetFields())
            {
                var attrs = field.GetCustomAttributes(typeof(KeyAttribute), true);
                if (attrs.Length == 0)
                    continue;

                var xProp = Expression.Field(x, field);
                var yProp = Expression.Field(y, field);
                expressions.Add(Expression.Assign(yProp, xProp));
            }

            var block = Expression.Block(expressions);
            var lambda = Expression.Lambda<Action<T, T>>(block, x, y);
            _copier = lambda.Compile();
        }

        public static void CopyTo(T from, T to)
        {
            _copier(from, to);
        }
    }

    [MessagePackObject]
    public class IMessage
    {
        public IMessage()
        {
        }

        public virtual byte[] Pack()
        {
            return MessagePackSerializer.Serialize(this);//, Utils.RpcUtil.lz4Options);
        }

        public virtual void UnPack(byte[] data)
        {
            var obj = Deserialize(data);

            Copier<IMessage>.CopyTo(obj, this);
        }

        public virtual string ToJson()
        { 
           return MessagePackSerializer.ConvertToJson(Pack());
        }

        public virtual void FromJson(string json)
        {
            var data = MessagePackSerializer.ConvertFromJson(json);
            this.UnPack(data);
        }

        public static IMessage Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<IMessage>(data);
        }

        public static IMessage DeserializeJson(string json)
        {
            byte[] bytes = MessagePackSerializer.ConvertFromJson(json);
            return Deserialize(bytes);
        }

        public virtual bool HasCallback()
        {
            return false;
        }

        public virtual object GetCallbackMsg()
        {
            return null;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    [MessagePackObject]
    public class IMessageWithCallback : IMessage
    {
        [IgnoreMember]
        public object _callback;

        public override bool HasCallback()
        {
            return true;
        }

        public override object GetCallbackMsg()
        {
            return _callback;
        }

        public override byte[] Pack()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}
