using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using util.crypt;
using util.ext;
using util.rep;

namespace util.rep.aead
{
    public class AeadReposit : LocalReposit
    {
        public const string Type = "aead";
        public const int Version = 1;
        public const string KeyDomain = "dir-key";

        public string confPath;

        public AeadReposit(string dir)
        {
            repPath = dir.locUnify();
            confPath = $"{repPath}/aead.conf";
        }

        public override string uri => $"aead@{repPath}";
        public override string LocalPath => repPath;

        protected override string rootPath => $"{repPath}/0";

        string repPath;
        AeadConf conf;
        int pathSkip;
        DirCrypt dirEnc;

        public override void open()
        {
            if (null != dirEnc)
                return;

            if (!confPath.fileExist())
                throw new Error(this, "NotExist", confPath);

            conf = loadConf(confPath);
            checkPwd(conf.decrypt);

            rootDir.Create();
            pathSkip = rootDir.FullName.Length + 1;

            dirEnc = conf.newDirCrypt(KeyDomain.utf8());
        }

        public static AeadConf loadConf(string path)
        {
            if (!path.fileExist())
                return null;
            var conf = path.readText().obj<AeadConf>();
            if (conf.Type != Type)
                throw new Error<AeadReposit>("InvalidType", conf.Type);
            if (conf.Version > Version)
                throw new Error<AeadReposit>("InvalidVersion", 
                    conf.Version, Version);

            return conf;
        }

        public override void addDir(string path)
        {
            var dir = locateToParent(path, out var name, true);
            if (null == name)
                throw new Error(this, "EmptyName", path);

            var sub = getSubItem(dir, name);
            if (sub == null)
                dir.CreateSubdirectory(settleName(dir, encryptName(name)));
            else if (!sub.isDir())
                throw new Error(this, "DstIsFile", path);
        }

        string settleName(DirectoryInfo dirInfo, string name)
        {
            var dir = dirInfo.FullName;
            if (!pathExist($"{dir}\\{name}"))
                return name;

            int idx = 0;
            while (idx++ < 1000)
            {
                var newName = $"{name}&{idx}";
                if (!pathExist($"{dir}\\{newName}"))
                    return newName;
            }
            throw new Error(this, "SettleOverflow", idx);
        }

        string settlePath(DirectoryInfo dirInfo, string name)
        {
            name = settleName(dirInfo, name);
            return $"{dirInfo.FullName}\\{name}";
        }

        bool pathExist(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public override void moveDir(string src, string dst)
        {
            if (!locateToDir(src, out var srcItem))
                throw new Error(this, "DirNotExist", src);

            var dstDir = locateToParent(dst, out var dstName, true);
            if (null == dstName)
                throw new Error(this, "EmptyDstName", dst);
            var dstItem = getSubItem(dstDir, dstName);
            if (dstItem != null && src.ToLower() != dst.ToLower())
                throw new Error(this, "DstDirExist", dst);

            var encDstName = encryptName(dstName);

            if (srcItem.FullName == $"{dstDir.FullName}\\{encDstName}")
                return;

            var dstLoc = settlePath(dstDir, encDstName);
            srcItem.MoveTo(dstLoc);
        }

        public override void deleteDir(string path, bool recurse)
        {
            if (!locateToDir(path, out var dir))
                throw new Error(this, "DirNotExist", path);
            dir.Delete(recurse);
        }

        protected bool equalPath(string src, string dst)
        {
            return src.ToLower() == dst.ToLower()
                && src.locName() == dst.locName();
        }

        public override void moveFile(string src, string dst)
        {
            if (equalPath(src, dst))
                return;

            if (locateToFile(src, out var srcFile) == false)
                throw new Error(this, "FileNotExist", src);

            var dir = locateToParent(dst, out var dstName, true);
            if (null == dstName)
                throw new Error(this, "EmptyDstName", dst);

            // not rename, then check exist item
            if (src.ToLower() != dst.ToLower()
                && getSubItem(dir, dstName) != null)
                throw new Error(this, "DstFileExist", dst);

            var dstLoc = settlePath(dir, encryptName(dstName));

            srcFile.MoveTo(dstLoc);
        }

        public override void deleteFile(string path)
        {
            if (locateToFile(path, out var fi))
                fi.Delete();
            else
                throw new Error(this, "FileNotExist", path);
        }

        public override Stream addFile(string path)
        {
            var dir = locateToParent(path, out var name, true);
            if (null == name)
                throw new Error(this, "EmptyFileName", path);

            var node = getSubItem(dir, name);
            if (null != node)
                throw new Error(this, "FileExist", path);

            var encPath = settlePath(dir, encryptName(name));
            return new AeadStream
            {
                fs = new FileStream(encPath,
                                    FileMode.CreateNew,
                                    FileAccess.ReadWrite,
                                    FileShare.Read,
                                    conf.packSize()),
                conf = conf,
            }.create();
        }

        public override Stream writeFile(string path)
        {
            if (locateToFile(path, out var encFile) == false)
                throw new Error(this, "FileNotExist", path);

            var encPath = encFile.FullName;
            return new AeadStream
            {
                fs = new FileStream(encPath,
                                    FileMode.Open,
                                    FileAccess.ReadWrite,
                                    FileShare.Read,
                                    conf.packSize()),
                conf = conf,
            }.open();
        }

        public override Stream readFile(string path)
        {
            if (locateToFile(path, out var encFile) == false)
                throw new Error(this, "FileNotExist", path);

            var encPath = encFile.FullName;
            return new AeadStream
            {
                fs = new FileStream(encPath, 
                                    FileMode.Open, 
                                    FileAccess.Read,
                                    FileShare.ReadWrite,
                                    conf.packSize()),
                conf = conf,
            }.open();
        }

        string encryptName(string name)
        {
            byte[] data = name.utf8();

            if (name.isGbk(out var gbk) && gbk.Length < data.Length - 2)
                data = gbk;

            var cipher = dirEnc.encrypt(data);

            var enc = cipher.b64().TrimEnd('=').Replace('/', '%');
            if (data == gbk)
                enc = $"{enc}@";

            return enc;
        }

        public override string decryptName(string name)
        {
            if (name.Length < 20// encrypt base64 min size
                || !decodeName(name, out var cipher, out var gbk))
                return null;

            var data = dirEnc.decrypt(cipher);

            return gbk ? data.gbk() : data.utf8();
        }

        public bool decodeName(string name, out byte[] cipher, out bool gbk)
        {
            cipher = null;
            gbk = false;
            int i = name.Length, end = name.Length;
            char[] cs = null;
            char c;
            while (i-- > 0)
            {
                switch (c = name[i])
                {
                    case '&':   // for conflict name postfix
                        end = i;
                        break;
                    case '@':   // for gbk encoding
                        gbk = true;
                        end = i;
                        break;
                    case '%':   // replace '/' in base64
                        cs = cs ?? name.ToCharArray();
                        cs[i] = '/';
                        break;
                    case '+': break;
                    default:    // invalid characters
                        if (!((c >= 'A' && c <= 'Z')
                            || (c >= 'a' && c <= 'z')
                            || (c >= '0' && c <= '9')))
                        {
                            return false;
                        }
                        break;
                }
            }

            if (null != cs)
                name = new string(cs, 0, end);
            else if (end != name.Length)
                name = name.Substring(0, end);

            cipher = name.b64();
            return true;
        }

        protected override DirectoryInfo addSubDir(DirectoryInfo dir,
                                                    string name)
            => getSubFile(dir, name) == null
            ? dir.CreateSubdirectory(settleName(dir, encryptName(name)))
            : throw new Error(this, "SubIsFile", name);

        protected override long getFileSize(FileInfo fi)
            => AeadStream.dataSize(fi.Length, conf);

        public override string parsePath(FileSystemInfo fi)
            => fi?.FullName.TrimEnd('\\', '/').skip(pathSkip)
            ?.Split('\\').conv(n => decryptName(n)).join("/");
    }
}
