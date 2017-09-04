using System;
using Tx.Core;
using ThriftSharp;

namespace Tx.Protocol.Thrift
{
    public class ThriftTransformBuilder : ITransformBuilder<byte[]>, ITransformBuilder<ArraySegment<byte>>
    {
        Func<TIn, byte[]> ITransformBuilder<byte[]>.Build<TIn>()
        {
            if (!typeof(TIn).IsThriftStruct())
            {
                return null;
            }

            return new ThriftWriter<TIn>().Transform;
        }

        Func<TIn, ArraySegment<byte>> ITransformBuilder<ArraySegment<byte>>.Build<TIn>()
        {
            if (!typeof(TIn).IsThriftStruct())
            {
                return null;
            }

            return new ThriftWriter<TIn>().TransformToArraySegment;
        }

        internal sealed class ThriftWriter<T>
        {
            public ThriftWriter()
            {
                var type = typeof(T);

                if (!type.IsThriftStruct())
                {
                    throw new NotSupportedException();
                }
            }

            public ArraySegment<byte> TransformToArraySegment(T value)
            {
                var payload = ThriftSerializer.Serialize(value);

                return new ArraySegment<byte>(payload);
            }

            public byte[] Transform(T value)
            {
                var data = TransformToArraySegment(value);

                var payload = data.ToByteArray();

                return payload;
            }
        }
    }
}
