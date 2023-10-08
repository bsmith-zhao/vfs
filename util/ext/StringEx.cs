using util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.crypt;

namespace util.ext
{
    public static class StringEx
    {
        public static string desc(this object obj, 
            Func<object, object> conv = null)
            => $"{{{string.Join(",", flds(obj, conv))}}}";

        static IEnumerable<string> flds(object obj, 
            Func<object, object> conv)
        {
            var tp = obj.GetType();
            string t;
            foreach (var f in tp.GetFields())
            {
                if ((t = text(f.GetValue(obj), conv)) == null)
                    continue;
                yield return $"{f.Name}:{t}";
            }
            foreach (var f in tp.GetProperties())
            {
                if (!f.CanRead 
                    || (t = text(f.GetValue(obj), conv)) == null)
                    continue;
                yield return $"{f.Name}:{t}";
            }
        }

        public static string text(this object v, 
            Func<object, object> conv = null)
        {
            if (conv != null)
                v = conv(v);
            if (v == null)
                return null;
            if (v is Array)
                return $"[{string.Join(",", v as object[])}]";
            return v.ToString();
        }

        public static bool empty(this string str)
            => str == null || str.Length == 0;

        public static string str(this object obj)
            => obj?.ToString();

        public static string str<T>(this T obj, Func<T, string> func)
            => func(obj);

        public static string low(this string str)
            => str?.ToLower();

        public static string skip(this string str, int count)
            => str?.Length >= count ? str.Substring(count) : null;

        public static string cut(this string str, int count)
            => str?.Length >= count
            ? str.Substring(0, str.Length - count)
            : "";

        public static string conv(this string str, Func<string, string> func)
            => str != null ? func(str) : null;

        public static string keep(this string str, Func<string, bool> func)
            => func(str) ? str : null;

        public static string join<T>(this IEnumerable<T> iter,
            string sep)
            => iter == null ? null : string.Join(sep, iter);

        public static byte[] utf8(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public static string utf8(this byte[] data)
        {
            if (data == null)
                return null;
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public static string utf8(this byte[] data, int offset, int count)
        {
            if (data == null)
                return null;
            return Encoding.UTF8.GetString(data, offset, count);
        }

        static Encoding GBK = Encoding.GetEncoding("GBK");
        public static byte[] gbk(this string str)
        {
            return GBK.GetBytes(str);
        }

        public static bool isGbk(this string str, out byte[] data)
        {
            data = str.gbk();
            return data.gbk() == str;
        }

        public static string gbk(this byte[] data)
        {
            return GBK.GetString(data);
        }

        public static char last(this string str)
        {
            return str[str.Length - 1];
        }
    }
}
