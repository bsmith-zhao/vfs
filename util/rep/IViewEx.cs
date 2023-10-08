using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;

namespace util.rep
{
    public static class IViewEx
    {
        public static void open(this IView rep, 
            Action<string, Func<byte[], bool>> queryPwd)
        {
            if (queryPwd != null)
                rep.open(vr => queryPwd(rep.uri, vr));
            else
                rep.open();
        }

        public static string relocFile(this IView rep, string path)
        {
            int idx = 0;
            while (idx++ <= 1000)
            {
                var newPath = $"{path}-{idx}";
                if (!rep.exist(newPath))
                {
                    rep.moveFile(path, newPath);
                    return newPath;
                }
            }
            throw new Error<IView>("RelocateOverflow", idx);
        }

        public static Dictionary<string, string> lowDirMap(this IView rep)
            => rep.enumAllDirs().toMap(d => d.low());

        public static string settle(this IView rep, string path, 
            string sep = null)
            => path.pathSettle(rep.exist, sep);

        public static string getPath(this IView v, string path)
            => v.getItem(path)?.path;

        public static bool getPath(this IView v,
            string s, out string p)
            => (p = getPath(v, s)) != null;
    }
}
