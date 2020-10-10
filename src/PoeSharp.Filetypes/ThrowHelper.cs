using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace PoeSharp.Filetypes
{
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
    }

}