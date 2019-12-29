using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace PoeSharp.Filetypes
{
    public static class StreamExtensions
    {

        public static void CopyTo(this Stream source, Stream destination, long offset,
            long length, int bufferSize = 80 * 1024)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            source.Position = offset;
            int read;
            var bytes = length;
            do
            {
                buffer = buffer.Slice(0, (int)Math.Min(bytes, bufferSize));
                read = source.Read(buffer);
                destination.Write(buffer.Slice(0, read));
                bytes -= read;
            } while (read > 0 && bytes > 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadInt32(this Stream stream)
        {
            Span<byte> s = stackalloc byte[4];
            stream.Read(s);
            fixed (byte* b = &s[0])
            {
                return *(int*)b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint ReadUInt32(this Stream stream)
        {
            Span<byte> s = stackalloc byte[4];
            stream.Read(s);
            fixed (byte* b = &s[0])
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
            Span<byte> s = stackalloc byte[8];
            stream.Read(s);
            fixed (byte* b = &s[0])
            {
                return *(long*)b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ushort ReadUInt16(this Stream stream)
        {
            Span<byte> s = stackalloc byte[2];
            stream.Read(s);
            fixed (byte* b = &s[0])
            {
                return *(ushort*)b;
            }
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
