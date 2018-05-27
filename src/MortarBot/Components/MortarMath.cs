using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MortarBot
{
    public static class MortarMath
    {
        private static readonly Random _randomizer
            = new Random();
        private static IDictionary<string, Func<decimal, decimal>> _functions
            = new Dictionary<string, Func<decimal, decimal>>()
            {
                { "abs", x => checked(Math.Abs(x)) },
                { "acos", x => checked(new decimal(Math.Acos(decimal.ToDouble(x)))) },
                { "acosh", x => checked(new decimal(Math.Acosh(decimal.ToDouble(x)))) },
                { "asin", x => checked(new decimal(Math.Asin(decimal.ToDouble(x)))) },
                { "asinh", x => checked(new decimal(Math.Asinh(decimal.ToDouble(x)))) },
                { "atan", x => checked(new decimal(Math.Atan(decimal.ToDouble(x)))) },
                { "atanh", x => checked(new decimal(Math.Atanh(decimal.ToDouble(x)))) },
                { "cbrt", x => checked(new decimal(Math.Cbrt(decimal.ToDouble(x)))) },
                { "ceiling", x => checked(Math.Ceiling(x)) },
                { "cos", x => checked(new decimal(Math.Cos(decimal.ToDouble(x)))) },
                { "cosh", x => checked(new decimal(Math.Cosh(decimal.ToDouble(x)))) },
                { "d", x => checked(_randomizer.Next(decimal.ToInt32(x)) + 1) },
                { "exp", x => checked(new decimal(Math.Exp(decimal.ToDouble(x)))) },
                { "floor", x => checked(Math.Floor(x)) },
                { "log", x => checked(new decimal(Math.Log(decimal.ToDouble(x)))) },
                { "logten", x => checked(new decimal(Math.Log10(decimal.ToDouble(x)))) },
                { "round", x => checked(Math.Round(x)) },
                { "sign", x => checked(new decimal(Math.Sign(x))) },
                { "sin", x => checked(new decimal(Math.Sin(decimal.ToDouble(x)))) },
                { "sinh", x => checked(new decimal(Math.Sinh(decimal.ToDouble(x)))) },
                { "sqrt", x => checked(new decimal(Math.Sqrt(decimal.ToDouble(x)))) },
                { "tan", x => checked(new decimal(Math.Tan(decimal.ToDouble(x)))) },
                { "tanh", x => checked(new decimal(Math.Tanh(decimal.ToDouble(x)))) },
                { "truncate", x => checked(Math.Truncate(x)) }
            };
        private static IDictionary<string, decimal> _constants
            = new Dictionary<string, decimal>()
            {
                { "e", checked(new decimal(Math.E)) },
                { "maxcpu", checked(2000m) },
                { "pi", checked(new decimal(Math.PI)) }
            };

        public static decimal Calculate(string formula)
        {
            var items = new List<decimal>(formula.Length);
            var buffer = new StringBuilder(formula.Length);
            var depth = 0;
            var isAdding = true;
            var isSignAllowed = true;
            foreach (var c in formula)
            {
                switch (c)
                {
                    case '+' when !isSignAllowed && depth == 0:
                    {
                        var value = CalculateItem(buffer.ToString());
                        items.Add(isAdding ? value : -value);
                        buffer.Clear();
                        isAdding =
                        isSignAllowed = true;
                        break;
                    }
                    case '-' when !isSignAllowed && depth == 0:
                    {
                        var value = CalculateItem(buffer.ToString());
                        items.Add(isAdding ? value : -value);
                        buffer.Clear();
                        isAdding = false;
                        isSignAllowed = true;
                        break;
                    }
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    {
                        buffer.Append(c);
                        isSignAllowed = true;
                        break;
                    }
                    case '(' when depth++ == 0:
                    case ')' when --depth == 0:
                    default:
                    {
                        buffer.Append(c);
                        isSignAllowed = false;
                        break;
                    }
                }
            }
            if (buffer.Length != 0)
            {
                var value = CalculateItem(buffer.ToString());
                items.Add(isAdding ? value : -value);
            }
            return items.Sum();
        }

        private static decimal CalculateItem(string itemFormula)
        {
            var factors = new List<(Func<decimal> value, bool isMultiplying)>(itemFormula.Length);
            var buffer = new StringBuilder(itemFormula.Length);
            var depth = 0;
            var nameLength = 0;
            var isEndingWithNumber = true;
            var isMultiplying = true;
            var isNumberOnly = true;
            foreach (var c in itemFormula)
            {
                switch (c)
                {
                    case '*' when depth == 0:
                    {
                        var value = buffer.ToString();
                        var result = isNumberOnly ?
                            () => decimal.Parse(value) :
                            isEndingWithNumber ?
                                (Func<decimal>)(() => _functions[value.Substring(0, nameLength)](decimal.Parse(value.Substring(nameLength)))):
                                () => _constants[value];
                        factors.Add((result, isMultiplying));
                        buffer.Clear();
                        isEndingWithNumber =
                        isMultiplying =
                        isNumberOnly = true;
                        break;
                    }
                    case '/' when depth == 0:
                    {
                        var value = buffer.ToString();
                        var result = isNumberOnly ?
                            () => decimal.Parse(value) :
                            isEndingWithNumber ?
                                (Func<decimal>)(() => _functions[value.Substring(0, nameLength)](decimal.Parse(value.Substring(nameLength)))):
                                () => _constants[value];
                        factors.Add((result, isMultiplying));
                        buffer.Clear();
                        isEndingWithNumber =
                        isNumberOnly = true;
                        isMultiplying = false;
                        break;
                    }
                    case '(' when depth++ == 0 && buffer.Length != 0:
                    {
                        var value = buffer.ToString();
                        var result = isNumberOnly ?
                            () => decimal.Parse(value) :
                            isEndingWithNumber ?
                                (Func<decimal>)(() => _functions[value.Substring(0, nameLength)](decimal.Parse(value.Substring(nameLength)))):
                                () => _constants[value];
                        factors.Add((result, isMultiplying));
                        buffer.Clear();
                        isEndingWithNumber =
                        isMultiplying =
                        isNumberOnly = true;
                        break;
                    }
                    case '(':
                    case ',':
                    {
                        break;
                    }
                    case ')' when --depth == 0:
                    {
                        var value = buffer.ToString();
                        var result = (Func<decimal>)(() => Calculate(value));
                        factors.Add((result, isMultiplying));
                        buffer.Clear();
                        isEndingWithNumber =
                        isMultiplying =
                        isNumberOnly = true;
                        break;
                    }
                    case '+' when depth == 0:
                    case '-' when depth == 0:
                    case '.' when depth == 0:
                    case '0' when depth == 0:
                    case '1' when depth == 0:
                    case '2' when depth == 0:
                    case '3' when depth == 0:
                    case '4' when depth == 0:
                    case '5' when depth == 0:
                    case '6' when depth == 0:
                    case '7' when depth == 0:
                    case '8' when depth == 0:
                    case '9' when depth == 0:
                    {
                        if (!isEndingWithNumber)
                        {
                            nameLength = buffer.Length;
                        }
                        isEndingWithNumber = true;
                        buffer.Append(c);
                        break;
                    }
                    default:
                    {
                        if (depth == 0)
                        {
                            if (isEndingWithNumber && buffer.Length != 0)
                            {
                                var value = buffer.ToString();
                                var result = isNumberOnly ?
                                    () => decimal.Parse(value) :
                                    (Func<decimal>)(() => _functions[value.Substring(0, nameLength)](decimal.Parse(value.Substring(nameLength))));
                                factors.Add((result, isMultiplying));
                                buffer.Clear();
                                isMultiplying = true;
                            }
                            isEndingWithNumber =
                            isNumberOnly = false;
                        }
                        buffer.Append(c);
                        break;
                    }
                }
            }
            if (buffer.Length != 0)
            {
                var value = buffer.ToString();
                var result = isNumberOnly ?
                    () => decimal.Parse(value) :
                    isEndingWithNumber ?
                        (Func<decimal>)(() => _functions[value.Substring(0, nameLength)](decimal.Parse(value.Substring(nameLength)))) :
                        () => _constants[value];
                factors.Add((result, isMultiplying));
            }
            return factors.Aggregate((value: 1m, previous: 1m), (x, n) =>
            {
                checked
                {
                    if (x.value == 0m)
                        return (0m, 0m);
                    var now = n.isMultiplying && x.previous % 1m == 0m ?
                        Enumerable.Repeat(0, decimal.ToInt32(Math.Abs(x.previous)))
                            .Select(_ => n.value())
                            .Sum() / Math.Abs(x.previous) :
                        n.value();
                    return (n.isMultiplying ?
                        x.value * now :
                        x.value / now, now);
                }
            }).value;
        }
    }
}
