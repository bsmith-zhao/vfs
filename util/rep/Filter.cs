using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util;
using util.ext;

namespace util.rep
{
    public class Filter
    {
        public string[] incs;
        public string[] excs;
        public string[] names;

        public override string ToString()
            => this.desc();

        public Filter init()
        {
            incs.each(inc =>
            {
                excs.each((i, exc) =>
                {
                    if (contain(inc, exc))
                        excs[i] = null;
                });
            });
            return this;
        }

        public bool allowDir(string path)
            => allowItem(path, true);

        public bool allowFile(string path)
            => allowItem(path, false);

        public bool allowItem(string path, bool isDir)
        {
            path = path.low();
            if (!include(path, isDir))
                return false;
            return !exclude(path);
        }

        bool include(string path, bool isDir)
            => !(incs?.Length > 0
                && !incs.exist(inc => contain(path, inc)
                            || (isDir && contain(inc, path))));

        bool exclude(string path)
            => excs.exist(exc => null != exc && contain(path, exc))
            || names.exist(key => path.Contains(key));

        bool contain(string path, string pre)
            => path.StartsWith(pre)
            && (path.Length == pre.Length 
                || path[pre.Length] == '/');
    }
}
