using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;

namespace util.rep
{
    public class LocalDirItem : DirItem
    {
        public LocalReposit rep;
        public DirectoryInfo dir;
        public string prefix;

        public LocalDirItem init()
        {
            path = prefix = rep.parsePath(dir);
            return this;
        }

        public override IEnumerable<DirItem> enumDirs()
        {
            if (dir != null)
            {
                foreach (var sub in dir.EnumerateDirectories())
                {
                    if (sub.isSystem() && sub.isHidden())
                        continue;
                    if (!getPath(sub, out var subPath))
                        continue;
                    yield return new LocalDirItem
                    {
                        rep = rep,
                        dir = sub,
                        path = subPath,
                        prefix = subPath,
                    };
                }
            }
        }

        bool getPath(FileSystemInfo sub, out string p)
        {
            p = rep.decryptName(sub.Name);
            if (p != null && prefix != null)
                p = $"{prefix}/{p}";
            return p != null;
        }

        public override IEnumerable<T> enumFiles<T>()
        {
            if (dir != null)
            {
                foreach (var fi in dir.EnumerateFiles())
                {
                    if (fi.isHidden() && fi.isSystem())
                        continue;
                    if (!getPath(fi, out var subPath))
                        continue;
                    fi.Refresh();
                    yield return rep.newFileItem<T>(fi, subPath);
                }
            }
        }
    }
}
