using System;
using System.Text;
using Newtonsoft.Json;
using Tx.Core;

namespace Tx.Protocol.Json
{
    public class JsonTransformBuilder : ITransformBuilder<byte[]>
    {
        public Func<TIn, byte[]> Build<TIn>()
        {
            return Serialize<TIn>;
        }

        private static byte[] Serialize<TIn>(TIn value)
        {
            var json = JsonConvert.SerializeObject(value);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}
