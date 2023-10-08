using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;

namespace util.rep
{
    public abstract class LocalReposit : Reposit
    {
        public override bool exist(string path)
            => locateToItem(path) != null;

        public override RepItem getItem(string path)
        {
            if (!locateToItem(path, out var item))
                return null;
            else if (item.isDir())
                return new RepItem
                {
                    path = parsePath(item),
                    isDir = true,
                };
            else
                return newFileItem<RepItem>
                    (new FileInfo(item.FullName), parsePath(item))
                    .use(e => e.isDir = false);
        }

        public override DirItem getDir(string path)
            => new LocalDirItem
            {
                rep = this,
                dir = locateToDir(path),
            }.init();

        public T newFileItem<T>(FileInfo file, string path)
            where T : FileItem, new()
            => new T
            {
                path = path,
                size = getFileSize(file),
                createTime = file.createTime(),
                writeTime = file.writeTime(),
            };

        public abstract string decryptName(string name);

        public abstract string parsePath(FileSystemInfo fi);

        protected T getSubItem<T>(IEnumerable<T> items,
            string name) where T : FileSystemInfo
        {
            name = name.low();
            return items.first(f =>
                decryptName(f.Name).low() == name);
        }

        protected DirectoryInfo getSubDir(DirectoryInfo dir, string name)
            => getSubItem(dir.EnumerateDirectories(), name);

        protected FileInfo getSubFile(DirectoryInfo dir, string name)
            => getSubItem(dir.EnumerateFiles(), name);

        protected FileSystemInfo getSubItem(DirectoryInfo dir, string name)
            => getSubItem(dir.EnumerateFileSystemInfos(), name);

        protected bool getParent(string path,
            out DirectoryInfo dir, out string name)
            => (dir = locateToParent(path, out name)) != null
            && name != null;

        protected bool locateToFile(string path, out FileInfo file)
            => (file = locateToFile(path)) != null;

        protected FileInfo locateToFile(string path)
            => getParent(path, out var dir, out var name) 
            ? getSubFile(dir, name) : null;

        protected bool locateToDir(string path, out DirectoryInfo dir)
            => (dir = locateToDir(path)) != null;

        protected DirectoryInfo locateToDir(string path)
            => getParent(path, out var dir, out var name)
            ? getSubDir(dir, name) : dir;

        protected bool locateToItem(string path, out FileSystemInfo item)
            => (item = locateToItem(path)) != null;

        protected FileSystemInfo locateToItem(string path)
            => getParent(path, out var dir, out var name)
            ? getSubItem(dir, name) : null;

        protected abstract DirectoryInfo addSubDir(DirectoryInfo dir,
            string name);

        protected abstract string rootPath { get; }

        DirectoryInfo rdir;
        protected DirectoryInfo rootDir
            => rdir ?? (rdir = new DirectoryInfo(rootPath));

        protected DirectoryInfo locateToParent(string path,
                                    out string name,
                                    bool create = false)
        {
            var nodes = splitPath(path, out name);
            var dir = rootDir;
            for (int i = 0; i < nodes?.Length - 1; i++)
            {
                if (nodes[i].Length == 0)
                    continue;
                var sub = getSubDir(dir, nodes[i]);
                if (null != sub)
                    dir = sub;
                else if (create)
                    dir = addSubDir(dir, nodes[i]);
                else
                    return null;
            }
            return dir;
        }

        protected virtual long getFileSize(FileInfo fi)
            => fi.Length;
    }
}
