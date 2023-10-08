using Fsp.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileInfo = Fsp.Interop.FileInfo;

namespace Fsp
{
    public class FspNodesPack
    {
        public IntPtr data;
        public uint total;
        public uint actual;

        public delegate void SetFileInfo(ref FileInfo info);

        public bool add(string name, SetFileInfo set)
        {
            var info = new DirInfo();
            info.SetFileNameBuf(name);
            set(ref info.FileInfo);
            return Api.FspFileSystemAddDirInfo(
                ref info, data, total, out actual);
        }
    }
}
