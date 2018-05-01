using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PoeSharp.Util
{
    public static class StreamExtensions
    {
        private static readonly Encoding UnicodeEncoding = Encoding.Unicode;
        private static readonly byte[] BInt32 = new byte[4];
        private static readonly byte[] BInt64 = new byte[8];
        private static readonly byte[] BShort = new byte[2];

        public static void CopyTo(this Stream source, Stream destination, long offset,
            long length, int bufferSize = 80 * 1024)
        {
            var buffer = new byte[bufferSize];
            source.Position = offset;
            int read;
            var bytes = length;
            do
            {
                read = source.Read(buffer, 0, (int)Math.Min(length, bufferSize));
                bytes -= read;
                destination.Write(buffer, 0, read);
            } while (read > 0 && bytes > 0);
        }

        public static unsafe int ReadInt32(this Stream stream)
        {
            stream.Read(BInt32, 0, 4);
            fixed (byte* b = &BInt32[0])
            {
                return *(int*)b;
            }
        }

        public static unsafe uint ReadUInt32(this Stream stream)
        {
            stream.Read(BInt32, 0, 4);
            fixed (byte* b = &BInt32[0])
            {
                return *(uint*)b;
            }
        }

        public static byte[] ReadBytes(this Stream stream, int length)
        {
            var b = new byte[length];
            stream.Read(b, 0, length);
            return b;
        }

        public static unsafe long ReadInt64(this Stream stream)
        {
            stream.Read(BInt64, 0, 8);
            fixed (byte* b = &BInt64[0])
            {
                return *(long*)b;
            }
        }

        public static unsafe ushort ReadUInt16(this Stream stream)
        {
            stream.Read(BShort, 0, 2);
            fixed (byte* b = &BShort[0])
            {
                return *(ushort*)b;
            }
        }

        public static unsafe Span<T> Read<T>(this Stream stream, int elements)
        {
            var size = Unsafe.SizeOf<T>();
            var buffer = stream.ReadBytes(elements * size);
            fixed (void* ptr = &buffer[0])
            {
                return new Span<T>(ptr, buffer.Length / size);
            }
        }

        public static T Read<T>(this Stream stream)
        {
            var size = Unsafe.SizeOf<T>();
            var buffer = stream.ReadBytes(size);
            return buffer.To<T>();
        }

        public static string ReadUnicodeString(this Stream stream, int length)
        {
            var b = new byte[length * 2];
            stream.Read(b, 0, b.Length);
            var str = UnicodeEncoding.GetString(b);
            if (str.EndsWith("\0"))
                str = str.Trim('\0');

            return str;
        }
    }
}
