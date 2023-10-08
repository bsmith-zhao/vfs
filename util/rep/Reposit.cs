using System;
using System.Collections.Generic;
using System.IO;
using util.ext;

namespace util.rep
{
    public abstract class Reposit : IBaseView
    {
        public abstract bool exist(string path);
        public abstract RepItem getItem(string path);

        public abstract DirItem getDir(string path);
        public abstract void addDir(string path);
        public abstract void moveDir(string src, string dst);
        public abstract void deleteDir(string path, bool recurse);

        public abstract Stream addFile(string path);
        public abstract Stream readFile(string path);
        public abstract Stream writeFile(string path);
        public abstract void moveFile(string src, string dst);
        public abstract void deleteFile(string path);

        public byte[] pwd;
        public Action<Func<byte[], bool>> queryPwd;
        protected void checkPwd(Func<byte[], bool> verify)
        {
            if (verify(pwd))
                return;

            if (queryPwd != null)
                queryPwd(verify);
            else
                throw new Error(this, "PwdFail");
        }

        public virtual bool seek => true;
        public virtual string LocalPath => null;
        public virtual void open(Action<Func<byte[], bool>> queryPwd)
        {
            this.queryPwd = queryPwd;
            open();
        }
        public virtual void open() { }
        public virtual void close() { }
        public virtual void getSpace(out long total, out long free)
        {
            total = 0;
            free = 0;
            if (null != LocalPath)
                $"{LocalPath}/".drvSpace(out total, out free);
        }

        public void Dispose() => close();

        protected string[] splitPath(string path, out string name)
        {
            var nodes = path?.Split('/', '\\');
            name = (nodes?.Length > 0) ? nodes[nodes.Length - 1] : null;
            if (name?.Length == 0)
                name = null;
            return nodes;
        }

        public void removeFile(string path)
            => deleteFile(path);

        public void removeDir(string path)
            => deleteDir(path, false);

        public Reposit getRep() => this;

        public void needWrite() { }

        public IEnumerable<string> enumAllDirs(string path, 
            PathConv conv = null)
            => getDir(path).enumAllDirs(conv)
            .conv(d => d.path);

        public IEnumerable<T> enumAllFiles<T>(string path,
            PathConv dirConv = null, PathConv fileConv = null) 
            where T : FileItem, new()
        {
            var dir = getDir(path);
            foreach (var f in dir.enumFiles<T>())
                if (allow(ref f.path, fileConv))
                    yield return f;

            foreach (var d in dir.enumAllDirs(dirConv))
            {
                foreach (var f in d.enumFiles<T>())
                    if (allow(ref f.path, fileConv))
                        yield return f;
            }
        }

        public IEnumerable<string> enumDirs(string path,
            PathConv conv = null)
            => getDir(path).enumDirs()
            .pick(f => allow(ref f.path, conv))
            .conv(d => d.path);

        public IEnumerable<T> enumFiles<T>(string path,
            PathConv conv = null)
            where T : FileItem, new()
            => getDir(path).enumFiles<T>()
            .pick(f => allow(ref f.path, conv));

        bool allow(ref string s, PathConv conv)
            => null == conv || (s = conv(s)) != null;

        public bool readOnly { set { } }

        public abstract string uri { get; }

        public override string ToString()
            => uri;
    }
}
