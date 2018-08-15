using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace PoeSharp.Util
{
    public static class StreamExtensions
    {
        private static readonly byte[] BInt32 = new byte[4];
        private static readonly byte[] BInt64 = new byte[8];
        private static readonly byte[] BShort = new byte[2];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this Stream source, Stream destination, long offset,
            long length, int bufferSize = 80 * 1024)
        {
            var buffer = new byte[bufferSize];
            source.Position = offset;
            int read;
            var bytes = length;
            do
            {
                read = source.Read(buffer, 0, (int)Math.Min(bytes, bufferSize));
                destination.Write(buffer, 0, read);
                bytes -= read;
            } while (read > 0 && bytes > 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadInt32(this Stream stream)
        {
            stream.Read(BInt32, 0, 4);
            fixed (byte* b = &BInt32[0])
            {
                return *(int*)b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint ReadUInt32(this Stream stream)
        {
            stream.Read(BInt32, 0, 4);
            fixed (byte* b = &BInt32[0])
            {
                return *(uint*)b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadBytes(this Stream stream, int length)
        {
            var sharedBuffer = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                stream.Read(sharedBuffer, 0, length);
                return sharedBuffer;
            }
            finally { ArrayPool<byte>.Shared.Return(sharedBuffer); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long ReadInt64(this Stream stream)
        {
            stream.Read(BInt64, 0, 8);
            fixed (byte* b = &BInt64[0])
            {
                return *(long*)b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ushort ReadUInt16(this Stream stream)
        {
            stream.Read(BShort, 0, 2);
            fixed (byte* b = &BShort[0])
            {
                return *(ushort*)b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Read(this Stream stream, Span<byte> buffer)
        {
            var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                var numRead = stream.Read(sharedBuffer, 0, buffer.Length);
                if ((uint)numRead > buffer.Length)
                {
                    throw new IOException("Read too much");
                }
                new Span<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);
                return numRead;
            }
            finally { ArrayPool<byte>.Shared.Return(sharedBuffer); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Span<T> Read<T>(this Stream stream, int elements)
        {
            var size = Unsafe.SizeOf<T>();
            var buffer = stream.ReadBytes(elements * size);
            fixed (void* ptr = &buffer[0])
            {
                return new Span<T>(ptr, buffer.Length / size);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(this Stream stream)
        {
            var size = Unsafe.SizeOf<T>();
            var buffer = stream.ReadBytes(size);
            return buffer.To<T>();
        }
    }
}
