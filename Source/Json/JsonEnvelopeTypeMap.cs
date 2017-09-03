using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Text;
using Newtonsoft.Json;

namespace Tx.Protocol.Json
{
    public sealed class JsonEnvelopeTypeMap : EnvelopeTypeMap
    {
        private readonly JsonSerializer _jsonSerializer;

        public JsonEnvelopeTypeMap()
            : this(false, JsonSerializer.CreateDefault())
        {
        }

        public JsonEnvelopeTypeMap(bool handleTransportObject, JsonSerializer serializer)
            : base(handleTransportObject)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            _jsonSerializer = serializer;
        }

        protected override IReadOnlyDictionary<string, Func<byte[], object>> BuildDeserializers(Type outputType)
        {
            object JsonDeserializer(byte[] e) => DeserializeJson(e, outputType);

            var deserializerMap = new Dictionary<string, Func<byte[], object>>(StringComparer.Ordinal)
            {
                { Protocol.Json, JsonDeserializer },
            };

            return deserializerMap;
        }

        private object DeserializeJson(byte[] data, Type outputType)
        {
            var json = Encoding.UTF8.GetString(data);

            using (var jsonTextReader = new JsonTextReader(new StringReader(json)))
            {
                return _jsonSerializer.Deserialize(jsonTextReader, outputType);
            }
        }
    }
}
