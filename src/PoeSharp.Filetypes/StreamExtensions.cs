using System;
using System.IO;
using System.Runtime.CompilerServices;

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace PoeSharp.Filetypes
{
    public static class StreamExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public static SpanOwner<T> Read<T>(
            this Stream stream, int elements)
            where T : unmanaged
        {
            var spanOwner = SpanOwner<T>.Allocate(elements);
            stream.Read(spanOwner.Span.Cast<T, byte>());
            return spanOwner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> ReadUtf8String(this Stream stream, int length)
        {
            Span<byte> span = stackalloc byte[length];
            stream.Read(span);
            return System.Text.Encoding.UTF8.GetString(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream ValidateSourceStream(this Stream source)
        {
            _ = source ?? throw new ArgumentNullException(
                nameof(source), "Stream is null");

            return !source.CanRead
                ? throw new ArgumentException(
                    "Stream cannot be read", nameof(source))
                : source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
