using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;

namespace util.rep
{
    public interface IBaseView : IView
    {
        void needWrite();
    }

    public class View : IBaseView
    {
        IBaseView iv;
        public virtual IBaseView inner
        {
            get => iv != null ? iv
                : throw new Error(this, "EmptyBase");
            set => iv = value;
        }

        public string root;
        public Filter flt;
        public string unlock;
        public string unlockPath;

        public virtual void open(Action<Func<byte[], bool>> queryPwd)
        {
            inner.open(queryPwd);
            checkUnlock();
        }

        public void checkUnlock()
        {
            if (unlock != null 
                && this.getPath(unlock, out unlockPath))
                needWrite();
        }

        public virtual IBaseView writer => inner;

        public virtual void needWrite()
            => inner.needWrite();

        public bool readOnly { get; set; } = false;

        IView wr => !readOnly ? writer 
            : throw new Error(this, "ReadOnly");

        public string uri 
            => root == null ? inner.uri : 
            $"{inner.uri}/<{root}>";

        public override string ToString()
            => new
            {
                @base = inner,
                root,
                flt,
                unlock = unlockPath,
            }.desc();

        public void addDir(string path)
            => wr.addDir(toBasePath(path));

        public Stream addFile(string path)
            => wr.addFile(toBasePath(path));

        public virtual void removeDir(string path)
            => wr.removeDir(toBasePath(path));

        public virtual void removeFile(string path)
            => wr.removeFile(toBasePath(path));

        public bool exist(string path)
            => inner.exist(toBasePath(path));

        public RepItem getItem(string path)
            => inner.getItem(toBasePath(path));

        public void moveDir(string src, string dst)
            => wr.moveDir(toBasePath(src), toBasePath(dst));

        public void moveFile(string src, string dst)
            => wr.moveFile(toBasePath(src), toBasePath(dst));

        public Stream readFile(string path)
            => inner.readFile(toBasePath(path));

        public Stream writeFile(string path)
            => wr.writeFile(toBasePath(path));

        //protected void checkPath(string path)
        //{
        //    if (!flt.allowItem(path))
        //        throw new Error(this, "notAllow", path);
        //}

        //protected string toCheckPath(string path)
        //{
        //    checkPath(path);
        //    return toBasePath(path);
        //}

        protected string toBasePath(string path)
            => root == null ? path : $"{root}/{path}";

        protected string toViewPath(string path)
            => root == null ? path 
            : path.Substring(root.Length + 1);

        public virtual void Dispose()
            => inner.Dispose();

        public virtual Reposit getRep() => inner.getRep();

        public IEnumerable<string> enumAllDirs(string path, 
                            PathConv conv)
            => inner.enumAllDirs(toBasePath(path), 
                p => allowDir(p, conv));

        public IEnumerable<T> enumAllFiles<T>(string path,
            PathConv dirConv, PathConv fileConv)
            where T : FileItem, new()
            => inner.enumAllFiles<T>(toBasePath(path),
                d => allowItem(d, true, dirConv), 
                f => allowItem(f, false, fileConv));

        string allowItem(string p, bool isDir, PathConv conv)
            => flt.allowItem(p = toViewPath(p), isDir)
            ? (null == conv ? p : conv(p)) : null;

        string allowDir(string p, PathConv conv)
            => allowItem(p, true, conv);

        string allowFile(string p, PathConv conv)
            => allowItem(p, false, conv);

        //string allowItem(string p, bool isDir, ItemConv conv)
        //    => flt.allowItem(p = toViewPath(p), isDir)
        //    ? (null == conv ? p : conv(p, isDir)) : null;

        //string allowDir(string p, PathConv conv)
        //    => flt.allowDir(p = toViewPath(p))
        //    ? (null == conv ? p : conv(p)) : null;

        //string allowFile(string p, PathConv conv)
        //    => flt.allowFile(p = toViewPath(p))
        //    ? (null == conv ? p : conv(p)) : null;

        public virtual IEnumerable<string> enumDirs(string path,
            PathConv conv)
            => inner.enumDirs(toBasePath(path), 
                p => allowDir(p, conv));

        public virtual IEnumerable<T> enumFiles<T>(string path,
            PathConv conv)
            where T : FileItem, new()
            => inner.enumFiles<T>(toBasePath(path),
                p => allowFile(p, conv));
    }
}
