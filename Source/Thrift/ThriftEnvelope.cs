using System;
using System.Reactive;
using ThriftSharp;

namespace Tx.Protocol.Thrift
{
    [ThriftStruct("Envelope")]
    public class ThriftEnvelope : IEnvelope
    {
        [ThriftField(1, true, "OccurrenceFileTime")]
        public long OccurrenceFileTime { get; set; }
        [ThriftField(2, true, "ReceivedFileTime")]
        public long ReceivedFileTime { get; set; }
        [ThriftField(3, true, "Protocol")]
        public string Protocol { get; set; }
        [ThriftField(4, false, "Source")]
        public string Source { get; set; }
        [ThriftField(5, true, "TypeId")]
        public string TypeId { get; set; }
        [ThriftField(6, true, "Data")]
        public sbyte[] Data { get; set; }

        public byte[] Payload => (byte[])(Array)Data;

        public DateTimeOffset OccurrenceTime => DateTimeOffset.FromFileTime(OccurrenceFileTime);
        public DateTimeOffset ReceivedTime => DateTimeOffset.FromFileTime(ReceivedFileTime);
        public object PayloadInstance => null;
    }
}
