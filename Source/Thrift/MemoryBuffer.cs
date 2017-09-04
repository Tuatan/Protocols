using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ThriftSharp.Protocols;
using ThriftSharp.Transport;

namespace Tx.Protocol.Thrift
{
    public sealed class MemoryBuffer : IThriftTransport
    {
        internal static MethodInfo WriteMethod;
        internal static MethodInfo ReadMethod;

        private MemoryStream _memory;

        static MemoryBuffer()
        {
            var assembly = typeof(ThriftBinaryProtocol).GetTypeInfo().Assembly;

            var thriftStructReaderType = assembly
                .GetTypes()
                .FirstOrDefault(i => !i.GetTypeInfo().IsPublic && i.Name == "ThriftStructReader");

            var thriftStructWriterType = assembly
                .GetTypes()
                .FirstOrDefault(i => !i.GetTypeInfo().IsPublic && i.Name == "ThriftStructWriter");

            WriteMethod = thriftStructWriterType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(i => i.Name == "Write");

            ReadMethod = thriftStructReaderType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(i => i.Name == "Read");
        }

        public static ArraySegment<byte> Serialize<T>(T obj)
        {
            var buffer = new MemoryBuffer { _memory = new MemoryStream() };

            WriteMethod
                .MakeGenericMethod(typeof(T))
                .Invoke(null, new object[] { obj, new ThriftBinaryProtocol(buffer) });

            //ThriftStructWriter.Write(obj, new ThriftBinaryProtocol(buffer));
#if NET451
            return new ArraySegment<byte>(buffer._memory.GetBuffer());
#else
            buffer._memory.TryGetBuffer(out ArraySegment<byte> _buffer);

            return _buffer;
#endif
        }

        public static T Deserialize<T>(ArraySegment<byte> bytes)
        {
            var buffer = new MemoryBuffer { _memory = new MemoryStream(bytes.Array, bytes.Offset, bytes.Count) };

            return (T)ReadMethod
                .MakeGenericMethod(typeof(T))
                .Invoke(null, new object[] { new ThriftBinaryProtocol(buffer) });
        }

        public void ReadBytes(byte[] output, int offset, int count)
        {
            _memory.Read(output, offset, count);
        }

        public void WriteBytes(byte[] bytes, int offset, int count)
        {
            _memory.Write(bytes, offset, count);
        }

        public Task FlushAndReadAsync()
        {
            throw new Exception("Don't use this.");
        }

        public void Dispose()
        {
            _memory.Dispose();
        }
    }
}
