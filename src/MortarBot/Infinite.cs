using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MortarBot
{
    public class Infinite<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;
        private IEnumerator<T> _cachedEnumerator;


        public Infinite(IEnumerable<T> source)
        {
            if (source is IList<T> list)
            {
                _cachedEnumerator = source.GetEnumerator();
            }
            else
            {
                _enumerator = source.GetEnumerator();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_cachedEnumerator is null)
            {
                var enumerabledCacher = new List<T>();
                do
                {
                    enumerabledCacher.Add(_enumerator.Current);
                    yield return _enumerator.Current;
                }
                while (_enumerator.MoveNext());
                _cachedEnumerator = enumerabledCacher.GetEnumerator();
            }
            while (true)
            {
                do yield return _cachedEnumerator.Current;
                while (_cachedEnumerator.MoveNext());
                _cachedEnumerator.Reset();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public static class InfiniteExtension
    {

        public static IEnumerable<T> ToInfinite<T>(this IEnumerable<T> source)
            => new Infinite<T>(source);
    }
}
