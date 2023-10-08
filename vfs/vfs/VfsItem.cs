using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileInfo = Fsp.Interop.FileInfo;

namespace util.rep.vfs
{
    public class VfsItem : RepItem { }

    public static class VfsItemEx
    {
        public static void copyTo(this RepItem item, ref FileInfo info)
        {
            info.FileSize = (ulong)item.size;
            info.CreationTime = (ulong)item.createTime;
            info.ChangeTime =
                info.LastWriteTime
                = (ulong)item.writeTime;

            if (item.isDir)
                info.FileAttributes = (uint)FileAttributes.Directory;
            else
                info.FileAttributes = (uint)FileAttributes.Normal;
        }
    }
}
