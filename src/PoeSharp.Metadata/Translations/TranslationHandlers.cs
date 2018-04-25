using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PoeSharp.Metadata.Translations
{
    public static class TranslationHandlers
    {
        public static Dictionary<string, IHandler> Handlers { get; }

        static TranslationHandlers() =>
            Handlers = typeof(TranslationHandlers).GetTypeInfo().DeclaredNestedTypes
                .Where(c => c.ImplementedInterfaces.Any(e => e.Name == nameof(IHandler)))
                .Select(e => (IHandler)Activator.CreateInstance(e.AsType()))
                .ToDictionary(h => h.Id, h => h);

        public interface IHandler
        {
            string Id { get; }
            decimal Handle(decimal value);
            decimal Reverse(decimal value);
        }

        public class SixtyPercentOfValueHandler : IHandler
        {
            public string Id => "60%_of_value";
            public decimal Handle(decimal value) => value * 0.6m;
            public decimal Reverse(decimal value) => value / 0.6m;
        }

        public class DeciSecondsToSeconds : IHandler
        {
            public string Id => "deciseconds_to_seconds";

            public decimal Handle(decimal value) => value * 10m;
            public decimal Reverse(decimal value) => value / 10m;
        }

        public class DivideByTwo0dp : IHandler
        {
            public string Id => "divide_by_two_0dp";
            public decimal Handle(decimal value) => Math.Round(value / 2, 0);
            public decimal Reverse(decimal value) => value * 2m;
        }

        public class DivideByTen0dp : IHandler
        {
            public string Id => "divide_by_ten_0dp";
            public decimal Handle(decimal value) => Math.Round(value / 10, 0);
            public decimal Reverse(decimal value) => value * 10m;
        }

        public class DivideByFifteen0dp : IHandler
        {
            public string Id => "divide_by_fifteen_0dp";
            public decimal Handle(decimal value) => Math.Round(value / 15, 0);
            public decimal Reverse(decimal value) => value * 15m;
        }

        //divide_by_twenty_then_double_0dp
        public class DivideByTwentyThenDouble : IHandler
        {
            public string Id => "divide_by_twenty_then_double_0dp";
            public decimal Handle(decimal value) => Math.Round(value / 20, 0) * 2;
            public decimal Reverse(decimal value) => value * 20 / 2;
        }

        public class DivideByOneHundred : IHandler
        {
            public string Id => "divide_by_one_hundred";
            public decimal Handle(decimal value) => value / 100m;
            public decimal Reverse(decimal value) => value * 100m;
        }

        public class DivideByOneHundredAndNegate : IHandler
        {
            public string Id => "divide_by_one_hundred_and_negate";

            public decimal Handle(decimal value) => -value / 100m;
            public decimal Reverse(decimal value) => -value * 100m;
        }

        public class DivideByOneHundred2dp : IHandler
        {
            public string Id => "divide_by_one_hundred_2dp";

            public decimal Handle(decimal value) => Math.Round(value / 100m, 2);
            public decimal Reverse(decimal value) => value * 100m;
        }

        public class MillisecondsToSeconds : IHandler
        {
            public string Id => "milliseconds_to_seconds";

            public decimal Handle(decimal value) => value / 1000;
            public decimal Reverse(decimal value) => value * 1000;
        }

        public class MillisecondsToSeconds0dp : IHandler
        {
            public string Id => "milliseconds_to_seconds_0dp";

            public decimal Handle(decimal value) => Math.Round(value / 1000, 0);
            public decimal Reverse(decimal value) => value * 1000;
        }

        public class MillisecondsToSeconds2dp : IHandler
        {
            public string Id => "milliseconds_to_seconds_2dp";

            public decimal Handle(decimal value) => Math.Round(value / 1000, 2);
            public decimal Reverse(decimal value) => value * 1000;
        }

        public class MultiplicativeDamageModifier : IHandler
        {
            public string Id => "multiplicative_damage_modifier";

            public decimal Handle(decimal value) => value + 100;
            public decimal Reverse(decimal value) => value - 100;
        }

        public class MultiplicativePermyriadDamageModifier : IHandler
        {
            public string Id => "multiplicative_permyriad_damage_modifier";

            public decimal Handle(decimal value) => value / 100 + 100;
            public decimal Reverse(decimal value) => (value - 100) * 100;
        }

        public class Negate : IHandler
        {
            public string Id => "negate";

            public decimal Handle(decimal value) => -value;
            public decimal Reverse(decimal value) => -value;
        }

        public class OldLeechPercent : IHandler
        {
            public string Id => "old_leech_percent";

            public decimal Handle(decimal value) => value / 5;
            public decimal Reverse(decimal value) => value * 5;
        }

        public class OldLeechPermyriad : IHandler
        {
            public string Id => "old_leech_permyriad";

            public decimal Handle(decimal value) => value / 500;
            public decimal Reverse(decimal value) => value * 500;
        }

        public class PerMinuteToPerSecond : IHandler
        {
            public string Id => "per_minute_to_per_second";

            public decimal Handle(decimal value) => Math.Round(value / 60m, 1);
            public decimal Reverse(decimal value) => value * 60;
        }

        public class PerMinuteToPerSecond0dp : IHandler
        {
            public string Id => "per_minute_to_per_second_0dp";

            public decimal Handle(decimal value) => Math.Round(value / 60m, 0);
            public decimal Reverse(decimal value) => value * 60;
        }

        public class PerMinuteToPerSecond2dp : IHandler
        {
            public string Id => "per_minute_to_per_second_2dp";

            public decimal Handle(decimal value) => Math.Round(value / 60m, 2);
            public decimal Reverse(decimal value) => value * 60;
        }

        public class PerMinutePerSecond2dpIfRequired : IHandler
        {
            public string Id => "per_minute_to_per_second_2dp_if_required";

            public decimal Handle(decimal value) => value % 60 != 0 ? Math.Round(value / 60, 2) : value / 60;
            public decimal Reverse(decimal value) => value * 60;
        }

        // TODO: Need to refactor Handlers to be able to return
        // strings, and then have ItemClasses.dat stuff in here, i.e.
        // not static, but instance with ItemClasses as constructor argument
        public class ModValueToItemClass : IHandler
        {
            public string Id => "mod_value_to_item_class";

            public decimal Handle(decimal value) => value;
            public decimal Reverse(decimal value) => value;
        }

    }
}
