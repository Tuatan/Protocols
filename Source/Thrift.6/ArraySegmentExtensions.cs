using System;

namespace Tx.Protocol.Thrift
{
    internal static class ArraySegmentExtensions
    {
        public static byte[] ToByteArray(this ArraySegment<byte> segment)
        {
            if (segment.Offset == 0 && segment.Count == segment.Array.Length)
            {
                return segment.Array;
            }

            var array = new byte[segment.Count - segment.Offset];

            Buffer.BlockCopy(segment.Array, segment.Offset, array, 0, array.Length);

            return array;
        }
    }
}
