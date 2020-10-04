using System;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace TestingStuff
{
    internal static class Program
    {

        private static void Main(string[] args) =>
            BenchmarkRunner.Run<Bench>();
    }

    [MemoryDiagnoser]
    public class Bench
    {

        [Benchmark]
        public int GetSizeDict() =>
            SizeTable[_tc];

        [GlobalSetup]
        public void Setup() => _tc = Type.GetTypeCode(typeof(decimal));

        private TypeCode _tc;

        [Benchmark]
        public int TypeCodeTest() => _tc switch
        {
            TypeCode.Boolean => 1,
            TypeCode.SByte => 1,
            TypeCode.Byte => 1,
            TypeCode.Int16 => 2,
            TypeCode.UInt16 => 2,
            TypeCode.Int32 => 4,
            TypeCode.UInt32 => 4,
            TypeCode.Int64 => 8,
            TypeCode.UInt64 => 8,
            TypeCode.Single => 4,
            TypeCode.Double => 8,
            TypeCode.Decimal => 16,
            _ => 0,
        };

        [Benchmark]
        [Arguments(typeof(decimal))]
        public int GetSize(Type type)
        {
            if (type == typeof(bool))
                return 1;
            if (type == typeof(byte))
                return 1;
            if (type == typeof(sbyte))
                return 1;
            if (type == typeof(short))
                return 2;
            if (type == typeof(ushort))
                return 2;
            if (type == typeof(int))
                return 4;
            if (type == typeof(uint))
                return 4;
            if (type == typeof(long))
                return 8;
            if (type == typeof(ulong))
                return 8;
            if (type == typeof(float))
                return 4;
            if (type == typeof(double))
                return 8;
            if (type == typeof(decimal))
                return 16;

            return 0;
        }

        private static readonly Dictionary<TypeCode, int> SizeTable =
            new Dictionary<TypeCode, int>
            {
            { TypeCode.Boolean , 1},
            { TypeCode.SByte , 1},
            { TypeCode.Byte , 1},
            { TypeCode.Int16 , 2},
            { TypeCode.UInt16 , 2},
            { TypeCode.Int32 , 4},
            { TypeCode.UInt32 , 4},
            { TypeCode.Int64 , 8},
            { TypeCode.UInt64 , 8},
            { TypeCode.Single , 4},
            { TypeCode.Double , 8},
            { TypeCode.Decimal , 16},
            };
    }
}
