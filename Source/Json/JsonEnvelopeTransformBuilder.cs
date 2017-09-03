using System;
using System.Collections.Generic;
using System.Reactive;
using Tx.Core;

namespace Tx.Protocol.Json
{
    public class JsonEnvelopeTransformBuilder : ITransformBuilder<JsonEnvelope>
    {
        private readonly IReadOnlyDictionary<string, ITransformBuilder<byte[]>> _serializers;

        public JsonEnvelopeTransformBuilder(IReadOnlyDictionary<string, ITransformBuilder<byte[]>> serializers)
        {
            _serializers = serializers;
        }

        public Func<TIn, JsonEnvelope> Build<TIn>()
        {
            foreach (var serializer in _serializers)
            {
                var transformer = serializer.Value.Build<TIn>();

                if (transformer != null)
                {
                    return new TransformBuilder<TIn>(serializer.Key, transformer).Transform;
                }
            }

            return null;
        }

        internal sealed class TransformBuilder<T>
        {
            private readonly string _manifestId;

            private readonly string _protocol;
            private readonly Func<T, byte[]> _serializer;

            public TransformBuilder(string protocol, Func<T, byte[]> serializer)
            {
                _protocol = protocol;
                _serializer = serializer;

                var type = typeof(T);

                _manifestId = type.GetTypeIdentifier();
            }

            public JsonEnvelope Transform(T value)
            {
                var now = DateTime.UtcNow.ToFileTimeUtc();

                var payload = _serializer(value);

                var envelope = new JsonEnvelope
                {
                    OccurrenceFileTime = now,
                    ReceivedFileTime = now,
                    Payload = payload,
                    Protocol = _protocol,
                    TypeId = _manifestId
                };

                return envelope;
            }
        }
    }
}
