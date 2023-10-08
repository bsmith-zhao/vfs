using util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;
using util.rep;

namespace util.rep
{
    public class RepView : View
    {
        Reposit rep;
        public RepView(Reposit rep)
        {
            this.rep = rep;
        }

        public string match;
        public string @lock;
        public Backup bak;

        public override IBaseView inner
            => rep;

        public override string ToString()
        => new
        {
            rep = rep.uri,
            root,
            flt,
            match = matchPath,
            @lock = lockPath,
            unlock = unlockPath,
            bak,
        }.desc();

        public override IBaseView writer 
            => canWrite ? rep 
            : throw new Error(this, "IsLocked", lockPath);

        bool canWrite = false;
        public override void needWrite()
            => canWrite = true;

        public override Reposit getRep() => rep;

        public string matchPath;
        public string lockPath;
        public override void open(Action<Func<byte[], bool>> queryPwd)
        {
            rep.open(queryPwd);

            matchPath = getMatch();
            canWrite = (lockPath = getLock()) == null;
            checkUnlock();
        }

        string getMatch()
        {
            if (match.empty())
                return null;

            if (null != rep.LocalPath)
            {
                var path = $"{rep.LocalPath}/{match}";
                if (File.Exists(path) || Directory.Exists(path))
                    return path;
            }

            return rep.getPath(match) 
                ?? throw new Error(this, "Mismatch", match);
        }

        string getLock()
        {
            if (@lock.empty())
                return null;

            if (null != rep.LocalPath)
            {
                var baseDir = new DirectoryInfo(rep.LocalPath);
                while (baseDir != null)
                {
                    var path = $"{baseDir.FullName}\\{@lock}";
                    if (Directory.Exists(path))
                        return path.locUnify();
                    baseDir = baseDir.Parent;
                }
            }

            if (rep.getPath(@lock, out var lk))
                return lk;

            foreach (var p in baseDirs())
                if (rep.getPath($"{p}/{@lock}", out lk))
                    return lk;

            var lockLow = @lock.low();
            foreach (var p in subDirs())
                if (rep.enumAllDirs(p).first(d 
                    => d.locName().low() == lockLow, out lk))
                    return lk;

            return null;
        }

        IEnumerable<string> baseDirs()
        {
            foreach (var p in baseDirs(root))
                yield return p;
            if (root != null)
                yield return root;
            if (!flt.incs.empty())
                foreach (var p in flt.incs)
                    foreach (var d in baseDirs(p))
                        yield return toBasePath(d);
        }

        IEnumerable<string> baseDirs(string path)
        {
            var ns = path.locSplit();
            int n = 1;
            while (n < ns?.Length)
                yield return string.Join("/", ns, 0, n++);
        }

        IEnumerable<string> subDirs()
        {
            if (flt.incs.empty())
                yield return root;
            else
                foreach (var d in flt.incs)
                    yield return toBasePath(d);
        }

        public override void removeFile(string path)
        {
            var wr = writer as Reposit;
            path = toBasePath(path);
            if (bak?.enable == true)
            {
                var bakPath = $"{bak.dir}/{path}";
                if (bak.all)
                    bakPath = bakPath.pathAppend(wr.exist, "-r");
                else
                    bakPath = bakPath.pathBackup(bak.rev, 
                        wr.exist, wr.deleteFile, "-r");

                wr.moveFile(path, bakPath);
            }
            else
                wr.deleteFile(path);
        }

        public override void Dispose()
        {
            rep.Dispose();
        }
    }
}
