using System;
using System.Reactive;
using Newtonsoft.Json;

namespace Tx.Protocol.Json
{
    public class JsonEnvelope : IEnvelope
    {
        public long OccurrenceFileTime { get; set; }
        public long ReceivedFileTime { get; set; }
        public string Protocol { get; set; }
        public string Source { get; set; }
        public string TypeId { get; set; }
        public byte[] Payload { get; set; }

        [JsonIgnore]
        public DateTimeOffset OccurrenceTime => DateTimeOffset.FromFileTime(OccurrenceFileTime);

        [JsonIgnore]
        public DateTimeOffset ReceivedTime => DateTimeOffset.FromFileTime(ReceivedFileTime);

        [JsonIgnore]
        public object PayloadInstance { get; set; }
    }
}
