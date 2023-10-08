using util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;

namespace util.rep
{
    public class DirReposit : LocalReposit
    {
        public DirReposit(string dir)
        {
            repPath = dir.locUnify();
            pathSkip = repPath.Length + 1;
        }

        public override string uri => $"dir@{repPath}";
        public override string LocalPath => repPath;

        protected override string rootPath => $"{repPath}/";

        string repPath;
        int pathSkip;

        public override void open()
        {
            if (!rootPath.dirExist())
                throw new Error(this, "NotExist", repPath);
        }

        public override bool exist(string path)
        {
            path = toFullPath(path);
            return File.Exists(path) 
                || Directory.Exists(path);
        }

        public override void addDir(string path)
        {
            Directory.CreateDirectory(toFullPath(path));
        }

        public override void moveDir(string src, string dst)
        {
            var srcLoc = toFullPath(src);
            if (src.ToLower() == dst.ToLower())
            {
                if (src.locName() == dst.locName())
                    return;
                srcLoc = relocDir(srcLoc);
            }
            var dstLoc = toFullPath(dst);
            Directory.CreateDirectory(dstLoc.locDir());
            Directory.Move(srcLoc, dstLoc);
        }

        string relocDir(string path)
        {
            int idx = 0;
            while (idx++ < 1000)
            {
                var newPath = $"{path}{idx}";
                if (Directory.Exists(newPath) == false
                    && File.Exists(newPath) == false)
                {
                    Directory.Move(path, newPath);
                    return newPath;
                }
            }
            throw new Error(this, "RelocateOverflow", idx);
        }

        public override void deleteDir(string path, bool recurse)
        {
            path = toFullPath(path);
            if (Directory.Exists(path) == false)
                throw new Error(this, "DirNotExist", path);
            Directory.Delete(path, recurse);
        }

        public override Stream addFile(string path)
        {
            var loc = toFullPath(path);
            Directory.CreateDirectory(loc.locDir());
            return new FileStream(loc, 
                FileMode.CreateNew, 
                FileAccess.ReadWrite);
        }

        public override void moveFile(string src, string dst)
        {
            var dstLoc = toFullPath(dst);
            Directory.CreateDirectory(dstLoc.locDir());
            File.Move(toFullPath(src), dstLoc);
        }

        public override void deleteFile(string path)
        {
            path = toFullPath(path);
            if (File.Exists(path))
                File.Delete(path);
            else
                throw new Error(this, "FileNotExist", path);
        }

        public override Stream readFile(string path)
        {
            return new FileStream(toFullPath(path), 
                FileMode.Open, FileAccess.Read);
        }

        public override Stream writeFile(string path)
        {
            return new FileStream(toFullPath(path), 
                FileMode.Open, 
                FileAccess.ReadWrite);
        }

        protected override DirectoryInfo addSubDir(DirectoryInfo dir, 
                                                    string name) 
            => dir.CreateSubdirectory(name);

        public override string parsePath(FileSystemInfo fi) 
            => fi?.FullName.TrimEnd('\\', '/').skip(pathSkip)
            ?.Replace("\\", "/");

        string toFullPath(string path) 
            => $"{repPath}/{path}";

        public override string decryptName(string name) 
            => name;
    }
}
