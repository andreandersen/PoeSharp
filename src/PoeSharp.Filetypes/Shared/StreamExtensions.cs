using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
        private static Span<byte> ReadBytes(this Stream stream, int length)
        {
            var span = new Span<byte>(new byte[length]);
            var read = stream.Read(span);
            return span.Slice(0, read);
        }

        private static async Task<Memory<byte>> ReadBytesAsync(this Stream stream, int length)
        {
            var mem = new byte[length].AsMemory();
            var read = await stream.ReadAsync(mem);
            return mem.Slice(0, read);
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

        private static unsafe Memory<T> ConvertMany<T>(this Memory<byte> mem)
        {
            var size = Unsafe.SizeOf<T>();
            var buffer = mem.Span;
            fixed (void* ptr = &buffer[0])
            {
                return new Span<T>(ptr, buffer.Length / size).ToArray();
            }
        }

        public static async Task<Memory<T>> ReadAsync<T>(this Stream stream, int elements)
        {
            var size = Unsafe.SizeOf<T>();
            var mem = await stream.ReadBytesAsync(size * elements);
            return mem.ConvertMany<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(this Stream stream)
        {
            var size = Unsafe.SizeOf<T>();
            var buffer = stream.ReadBytes(size);
            return buffer.To<T>();
        }

        public static async Task<T> ReadAsync<T>(this Stream stream)
        {
            var size = Unsafe.SizeOf<T>();
            var buffer = await stream.ReadBytesAsync(size);
            return buffer.Span.To<T>();
        }

        public static ReadOnlySpan<char> ReadUtf8String(this Stream stream, int length)
        {
            Span<byte> span = stackalloc byte[length];
            stream.Read(span);
            return System.Text.Encoding.UTF8.GetString(span);
        }

        public static Stream ValidateSourceStream(this Stream source)
        {
            _ = source ?? throw new ArgumentNullException(
                nameof(source), "Stream is null");

            return !source.CanRead
                ? throw new ArgumentException(
                    "Stream cannot be read", nameof(source))
                : source;
        }

        public static Stream ValidateDestinationStream(this Stream destination)
        {
            _ = destination ?? throw new ArgumentNullException(
                nameof(destination), "Stream is null");

            return !destination.CanWrite ? 
                throw new ArgumentException(
                    "Stream cannot be written to", nameof(destination)) 
                : destination;
        }

    }
}
