using System;
using System.Collections.Generic;
using System.IO;

namespace util.rep
{
    public delegate string PathConv(string path);

    public interface IView : IDisposable
    {
        string uri { get; }

        void open(Action<Func<byte[], bool>> queryPwd = null);

        Reposit getRep();

        bool readOnly { set; }

        IEnumerable<string> enumDirs(string path = null,
            PathConv conv = null);

        IEnumerable<T> enumFiles<T>(string path = null,
            PathConv conv = null)
            where T : FileItem, new();

        IEnumerable<string> enumAllDirs(string path = null,
            PathConv conv = null);

        IEnumerable<T> enumAllFiles<T>(string path = null,
            PathConv dirConv = null, PathConv fileConv = null)
            where T : FileItem, new();

        bool exist(string path);
        RepItem getItem(string path);

        Stream addFile(string path);
        void removeFile(string path);
        void moveFile(string src, string dst);
        Stream writeFile(string path);
        Stream readFile(string path);

        void addDir(string path);
        void removeDir(string path);
        void moveDir(string src, string dst);
    }
}
