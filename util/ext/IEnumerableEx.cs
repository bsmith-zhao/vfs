﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.ext
{
    public static class IEnumerableEx
    {
        public static IEnumerable<T> all<T>(this IEnumerable<T> iter, Func<T, IEnumerable<T>> sub)
        {
            var stack = new Stack<IEnumerable<T>>();
            stack.Push(iter);
            while (stack.Count > 0)
            {
                foreach (var item in stack.Pop())
                {
                    yield return item;
                    var subs = sub(item);
                    if (subs != null)
                        stack.Push(subs);
                }
            }
        }

        public static IEnumerable<T> exc<T>(this IEnumerable<T> iter, Func<T, bool> func)
        {
            foreach (T elem in iter)
                if (!func(elem))
                    yield return elem;
        }

        public static IEnumerable<T> pick<T>(this IEnumerable<T> iter, Func<T, bool> func)
        {
            foreach (T elem in iter)
                if (func(elem))
                    yield return elem;
        }

        public static IEnumerable<T> pick<T>(this IEnumerable iter, Func<T, bool> func)
        {
            foreach (T elem in iter)
                if (func(elem))
                    yield return elem;
        }

        public static IEnumerable<O> conv<I, O>(this IEnumerable iter, Func<I, O> func)
        {
            foreach (I e in iter)
                yield return func(e);
        }

        public static IEnumerable<O> conv<I, O>(this IEnumerable<I> iter, Func<I, O> func)
        {
            foreach (I e in iter)
                yield return func(e);
        }

        public static T first<T>(this IEnumerable<T> iter, 
            Func<T, bool> func)
            => iter != null ? iter.FirstOrDefault(func)
            : default(T);

        public static bool first<T>(this IEnumerable<T> iter, Func<T, bool> func, out T res)
        {
            if (null != iter)
            {
                foreach (var item in iter)
                {
                    if (func(item))
                    {
                        res = item;
                        return true;
                    }
                }
            }
            res = default(T);
            return false;
        }

        public static T first<T>(this IEnumerable<T> iter)
            => iter != null ? iter.FirstOrDefault() : default(T);

        public static List<T> toList<T>(this IEnumerable<T> iter)
        {
            return new List<T>(iter);
        }

        public static List<T> toList<T>(this IEnumerable iter)
        {
            return new List<T>(iter.OfType<T>());
        }

        public static IEnumerable each<T>(this IEnumerable iter, Action<T> func)
        {
            if (null != iter)
            {
                foreach (T e in iter)
                    func(e);
            }
            return iter;
        }

        public static IEnumerable<T> each<T>(this IEnumerable<T> iter, Action<T> func)
        {
            if (null != iter)
                foreach (var e in iter)
                    func(e);
            return iter;
        }

        public static Dictionary<K, V> toMap<K, V>(this IEnumerable<V> iter, Action<V, Dictionary<K,V>> func)
        {
            var map = new Dictionary<K, V>();
            foreach (var e in iter)
                func(e, map);
            return map;
        }

        public static Dictionary<K, V> toMap<K, V>(this IEnumerable<V> iter, Func<V, K> func)
        {
            var map = new Dictionary<K, V>();
            foreach (var e in iter)
                map[func(e)] = e;
            return map;
        }

        public static bool exist<T>(this IEnumerable<T> iter, Func<T, bool> func)
        {
            if (null != iter)
                foreach (var e in iter)
                    if (func(e))
                        return true;
            return false;
        }

        public static bool exist<T>(this IEnumerable iter, Func<T, bool> check)
        {
            if (null != iter)
            {
                foreach (T e in iter)
                {
                    if (check(e))
                        return true;
                }
            }
            return false;
        }
    }
}
