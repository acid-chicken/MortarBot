using System;
using System.Collections.Generic;

namespace MortarBot
{
    public static class MortarMath
    {
        private static Random _randomizer
            = new Random();
        private static IDictionary<string, Func<decimal, decimal>> _functions
            = new Dictionary<string, Func<decimal, decimal>>()
            {
                { "d", x => _randomizer.Next(decimal.ToInt32(x)) + 1 }
            };
        private static IDictionary<string, decimal> _constants
            = new Dictionary<string, decimal>()
            {
                { "pi", new decimal(Math.PI) }
            };
    }
}
