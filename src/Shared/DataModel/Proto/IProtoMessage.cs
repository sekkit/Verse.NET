/*
 * Wrapper for Protobuf Message
 */
/*
using Fenix.Common.Rpc;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using Google.ProtocolBuffers;
using Fenix.Common; 

namespace Shared.DataModel
{
    [MessagePackObject]
    public class IProtoMessage<T> : Fenix.Common.Rpc.IMessage where T : Google.ProtocolBuffers.IMessage
    {
        [Key(0)]
        public byte[] ProtoBytes
        {
            get
            {
                if (Proto == null)
                {
                    var t = (T)Activator.CreateInstance(typeof(T));
                    _proto = (T)parseFromFunc(new byte[] { });
                }

                return Proto.ToByteArray();
            }
            set
            {
                var t = (T)Activator.CreateInstance(typeof(T));
                _proto = (T)parseFromFunc(value);
            }
        }

        private delegate T ParseFromDelegate(byte[] data);

        [IgnoreMember]
        ParseFromDelegate parseFromFunc;

        public IProtoMessage()
        {
            Init();
        }

        [IgnoreMember]
        protected T _proto;

        [IgnoreMember]
        public T Proto
        {
            get => GetProto();
            set => _proto = value;
        }

        public T GetProto()
        {
            if (_proto != null)
                return _proto;

            //if (_proto == null)
            //{
            _proto = (T)Activator.CreateInstance(typeof(T));
            return _proto;
            //}

            //var t = (T)Activator.CreateInstance(typeof(T));
            //_proto = (T)parseFromFunc(ProtoBytes);
            //return _proto;
        }

        private void Init()
        {
            var type = typeof(T);
            var method = type.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
            if (method == null)
            {
                Log.Error("proto_type_error " + type.Name);
                throw new Exception("invalid_proto_type");
            }
            parseFromFunc = (ParseFromDelegate)Delegate.CreateDelegate(typeof(ParseFromDelegate), method);
        }

        public override byte[] Pack()
        {
            return ProtoBytes;
            //ProtoBytes = this.Proto.ToByteArray();
            //return ProtoBytes;
        }

        public new static IProtoMessage<T> Deserialize(byte[] data)
        {
            var obj = new IProtoMessage<T>();
            obj.ProtoBytes = data;
            return obj;
            //return MessagePackSerializer.Deserialize<IProtoMessage<T>>(data, Fenix.Common.Utils.RpcUtil.lz4Options);
        }

        public override string ToString()
        {
            return ToJson();
        }

        public override string ToJson()
        {
            return this.Proto.ToJson();
        }
    }
}
*/