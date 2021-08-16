namespace PoeSharp.Filetypes
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    [Pure, DebuggerStepThrough]
    public static class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void NotImplemented()
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T NotImplemented<T>()
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void NotSupported()
        {
            throw new NotSupportedException();
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T NotSupported<T>()
        {
            throw new NotSupportedException();
        }

        public static void ArgumentException(string? message = default)
        {
            throw new ArgumentException(message);
        }
    }
}