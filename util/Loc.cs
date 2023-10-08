using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;

namespace util
{
    public static class Loc
    {
        public static string locOwner(this string path, int maxLevel, out int level)
        {
            var ns = path.locSplit();
            ns = ns.head(maxLevel.min(ns.Length - 1));
            level = ns.Length;
            return ns.join("/");
        }

        public static string[] locSplit(this string path)
            => path?.Split('/', '\\');

        public static string[] locUnify(this string[] paths)
            => paths?.conv(p => p.locUnify())
            .pick(p => p?.Length > 0).ToArray()
            .keep(arr => arr.Length > 0);

        public static string locUnify(this string path)
            => path?.Split('/', '\\')
                .conv(nd => nd.Trim())
                .exc(nd => nd.Length == 0).join("/")
                .keep(p => !p.empty());

        public static string locDir(this string path)
        {
            if (null == path)
                return null;
            var pos = path.LastIndexOf("/");
            if (-1 == pos)
                return null;
            return path.Substring(0, pos);
        }

        public static string locName(this string path)
        {
            if (null == path)
                return null;
            int pos = path.LastIndexOf('/');
            if (pos == -1)
                return path;
            return path.Substring(pos + 1, path.Length - pos - 1);
        }

        public static string locTrunk(this string path)
        {
            if (null == path)
                return null;
            var pos = path.Length - 1;
            pos = path.LastIndexOf('.', pos, pos - path.LastIndexOf('/'));
            if (pos == -1)
                return path;
            return path.Substring(0, pos);
        }

        public static string locExt(this string path)
        {
            if (null == path)
                return null;
            var pos = path.Length - 1;
            pos = path.LastIndexOf('.', pos, pos - path.LastIndexOf('/'));
            if (pos == -1)
                return null;
            return path.Substring(pos, path.Length - pos);
        }

        public static string locMerge(this string dir, string name, 
            params string[] others)
            => others.Length == 0 
            ? $"{dir}/{name}" 
            : $"{dir}/{name}/{string.Join("/", others)}";
    }
}
