using System;
using System.Diagnostics;

namespace DatRead
{
    public static class SwExt
    {
        public static TimeSpan ElapsedAndReset(this Stopwatch sw)
        {
            var el = sw.Elapsed;
            sw.Restart();
            return el;
        }

        public static TimeSpan PrintElapsedMs(this TimeSpan ts)
        {
            Console.WriteLine($"{ts.TotalMilliseconds} ms");
            return ts;
        }

        public static void AddTo(this TimeSpan ts, ref TimeSpan total) => total = total.Add(ts);
    }
}
