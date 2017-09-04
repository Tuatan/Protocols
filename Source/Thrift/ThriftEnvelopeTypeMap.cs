using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Text;
using ThriftSharp;
using System.Reflection;
using System.Linq;

namespace Tx.Protocol.Thrift
{
    public sealed class ThriftEnvelopeTypeMap : EnvelopeTypeMap
    {
        public ThriftEnvelopeTypeMap()
            : this(false)
        {
        }

        public ThriftEnvelopeTypeMap(bool handleTransportObject)
            : base(handleTransportObject)
        {
        }

        protected override IReadOnlyDictionary<string, Func<byte[], object>> BuildDeserializers(Type outputType)
        {
            // TODO: Replace with dynamic compilation
            var method = typeof(MemoryBuffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(i => i.Name == nameof(MemoryBuffer.Deserialize))
                .MakeGenericMethod(outputType);

            Func<byte[], object> deserializer = (byte[] e) => method.Invoke(null, new object[]{ new ArraySegment<byte>(e) });

            var deserializerMap = new Dictionary<string, Func<byte[], object>>(StringComparer.Ordinal)
            {
                { Protocol.Thrift, deserializer },
            };

            return deserializerMap;
        }
    }
}
