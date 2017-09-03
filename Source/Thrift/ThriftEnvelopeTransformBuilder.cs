using System;
using System.Collections.Generic;
using System.Reactive;
using Tx.Core;

namespace Tx.Protocol.Thrift
{
    public class ThriftEnvelopeTransformBuilder : ITransformBuilder<ThriftEnvelope>
    {
        private readonly IReadOnlyDictionary<string, ITransformBuilder<byte[]>> _serializers;

        public ThriftEnvelopeTransformBuilder(IReadOnlyDictionary<string, ITransformBuilder<byte[]>> serializers)
        {
            _serializers = serializers;
        }

        public Func<TIn, ThriftEnvelope> Build<TIn>()
        {
            foreach (var serializer in _serializers)
            {
                var transformer = serializer.Value.Build<TIn>();

                if (transformer != null)
                {
                    return new ThriftEnvelopeTransformer<TIn>(serializer.Key, transformer).Transform;
                }
            }

            return null;
        }

        internal sealed class ThriftEnvelopeTransformer<T>
        {
            private readonly string _manifestId;

            private readonly string _protocol;
            private readonly Func<T, byte[]> _serializer;

            public ThriftEnvelopeTransformer(string protocol, Func<T, byte[]> serializer)
            {
                _protocol = protocol;
                _serializer = serializer;

                var type = typeof(T);

                _manifestId = type.GetTypeIdentifier();
            }

            public ThriftEnvelope Transform(T value)
            {
                var now = DateTime.UtcNow.ToFileTime();

                var payload = _serializer(value);

                var envelope = new ThriftEnvelope
                {
                    OccurrenceFileTime = now,
                    ReceivedFileTime = now,
                    // ReSharper disable once PossibleInvalidCastException
                    Data = (sbyte[])(Array)payload,
                    Protocol = _protocol,
                    TypeId = _manifestId
                };

                return envelope;
            }
        }
    }
}
