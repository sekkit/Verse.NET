// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;

namespace MessagePack
{
    /// <summary>
    /// public utilities and extension methods for various external types.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// A value indicating whether we're running on mono.
        /// </summary>
#if UNITY_2018_3_OR_NEWER
        public const bool IsMono = true; // hard code since we haven't tested whether mono is detected on all unity platforms.
#else
        public static readonly bool IsMono = Type.GetType("Mono.Runtime") is Type;
#endif

        public delegate void GetWriterBytesAction<TArg>(ref MessagePackWriter writer, TArg argument);

        public static byte[] GetWriterBytes<TArg>(TArg arg, GetWriterBytesAction<TArg> action)
        {
            using (var sequenceRental = SequencePool.Shared.Rent())
            {
                var writer = new MessagePackWriter(sequenceRental.Value);
                action(ref writer, arg);
                writer.Flush();
                return sequenceRental.Value.AsReadOnlySequence.ToArray();
            }
        }

        public static Memory<T> GetMemoryCheckResult<T>(this IBufferWriter<T> bufferWriter, int size = 0)
        {
            var memory = bufferWriter.GetMemory(size);
            if (memory.IsEmpty)
            {
                throw new InvalidOperationException("The underlying IBufferWriter<byte>.GetMemory(int) method returned an empty memory block, which is not allowed. This is a bug in " + bufferWriter.GetType().FullName);
            }

            return memory;
        }
    }
}
