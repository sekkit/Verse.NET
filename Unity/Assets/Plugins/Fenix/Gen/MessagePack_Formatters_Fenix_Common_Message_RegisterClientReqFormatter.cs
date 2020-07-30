// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.Fenix.Common.Message
{
    using System;
    using System.Buffers;
    using MessagePack;

    public sealed class RegisterClientReqFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Fenix.Common.Message.RegisterClientReq>
    {


        public void Serialize(ref MessagePackWriter writer, global::Fenix.Common.Message.RegisterClientReq value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            writer.Write(value.hostId);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.hostName, options);
            formatterResolver.GetFormatterWithVerify<global::Fenix.Common.Message.RegisterClientReq.Callback>().Serialize(ref writer, value.callback, options);
        }

        public global::Fenix.Common.Message.RegisterClientReq Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __hostId__ = default(uint);
            var __hostName__ = default(string);
            var __callback__ = default(global::Fenix.Common.Message.RegisterClientReq.Callback);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __hostId__ = reader.ReadUInt32();
                        break;
                    case 1:
                        __hostName__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __callback__ = formatterResolver.GetFormatterWithVerify<global::Fenix.Common.Message.RegisterClientReq.Callback>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Fenix.Common.Message.RegisterClientReq();
            ____result.hostId = __hostId__;
            ____result.hostName = __hostName__;
            ____result.callback = __callback__;
            reader.Depth--;
            return ____result;
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name