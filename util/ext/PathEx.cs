using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.ext
{
    public static class PathEx
    {
        public static string pathBackup(this string path, 
            int total, Func<string, bool> exist, 
            Action<string> delete, string sep = null)
        {
            total = total.minLimit(2);
            var trk = path.locTrunk();
            var ext = path.locExt();
            string newPath = null;
            int idx = 0;
            while (idx++ < total)
            {
                newPath = getPath(trk, sep, idx, ext);
                if (!exist(newPath))
                    break;
            }
            if (idx > total)
            {
                idx = 1;
                newPath = getPath(trk, sep, idx, ext);
                delete(newPath);
            }
            var delPath = getPath(trk, sep, idx==total?1:(idx + 1), ext);
            if (exist(delPath))
                delete(delPath);

            return newPath;
        }

        static string getPath(string trk, string sep, int idx, string ext)
            => $"{trk}{sep}{idx}{ext}";

        public static void pathPush(this string path, int cnt, 
            Func<string, bool> exist, Action<string> delete, 
            Action<string, string> move, string sep = null)
        {
            var trk = path.locTrunk();
            var ext = path.locExt();
            var lastPath = $"{trk}{sep}{cnt}{ext}";
            if (exist(lastPath))
                delete(lastPath);
            while (cnt > 1)
            {
                var bakPath = $"{trk}{sep}{cnt - 1}{ext}";
                if (exist(bakPath))
                    move(bakPath, $"{trk}{sep}{cnt}{ext}");
                cnt--;
            }
            if (exist(path))
                move(path, $"{trk}{sep}{1}{ext}");
        }

        public static string pathSettle(this string path, 
            Func<string, bool> exist, string sep = null)
        {
            if (!exist(path))
                return path;
            return path.pathAppend(exist, sep);
        }

        public static string pathAppend(this string path, 
            Func<string, bool> exist, string sep = null)
        {
            var trk = path.locTrunk();
            var ext = path.locExt();
            int idx = 1;
            while (idx < 1000)
            {
                var newPath = getPath(trk, sep, idx++, ext);
                if (!exist(newPath))
                    return newPath;
            }
            throw new Error(typeof(Path), "AppendOverflow", idx);
        }

        public static bool pathExist(this string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public static string pathSettle(this string path, string sep = null)
        {
            return path.pathSettle(pathExist, sep);
        }
    }
}
