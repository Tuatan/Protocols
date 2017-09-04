using System;
using System.Linq;
using System.Reflection;
using ThriftSharp;

namespace Tx.Protocol.Thrift
{
    internal static class TypeExtensions
    {
        public static bool IsThriftStruct(this Type type)
        {
            return type.GetTypeInfo().GetCustomAttributes()
                .Any(a => a is ThriftStructAttribute);
        }
    }
}
