using Fsp;
using Fsp.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using util;
using util.ext;
using util.rep;
using FileInfo = Fsp.Interop.FileInfo;

namespace vfs
{
    public class VfsFileBase : FileSystemBase, IDisposable
    {
        public Reposit rep;
        public string volumeLabel;

        const string RootPath = @"\";
        const uint DirType = (uint)FileAttributes.Directory;
        const uint FileType = (uint)FileAttributes.Normal;
        const int NotSupport = STATUS_NOT_SUPPORTED;
        const int OK = STATUS_SUCCESS;
        const int NotFound = STATUS_OBJECT_NAME_NOT_FOUND;

        public override Int32 GetVolumeInfo(
            out VolumeInfo vinfo)
        {
            vinfo = default(VolumeInfo);
            vinfo.SetVolumeLabel(volumeLabel);
            rep.getSpace(out var totalSize, out var freeSize);
            vinfo.TotalSize = (ulong)totalSize;
            vinfo.FreeSize = (ulong)freeSize;
            return OK;
        }

        public override int GetSecurityByName(
            string path,
            out uint attr,
            ref byte[] security)
        {
            attr = DirType;
            if (path == RootPath)
                return OK;
            if (!getItem(path, out var item))
                return NotFound;
            else if (!item.isDir)
                attr = FileType;
            return OK;
        }

        bool getItem(string path, out RepItem item)
            => (item = rep.getItem(path)) != null;

        //public override bool ReadDirectoryEntry(
        //    object fsNode, 
        //    object fsDesc, 
        //    string Pattern, 
        //    string marker, 
        //    ref object ctx, 
        //    out string name, 
        //    out FileInfo info)
        //{
        //    name = null;
        //    info = default(FileInfo);

        //    var node = fsNode as VfsNode;

        //    if (marker == null)
        //        node.items = getSubItems(node.path).GetEnumerator();

        //    var items = node.items;
        //    if (items == null)
        //        return false;

        //    VfsItem item = items.Current;
        //    if (!items.MoveNext())
        //        node.items = null;

        //    name = item.path.locName();
        //    item.copyTo(ref info);

        //    return true;
        //}

        public override int ReadDirectory(
            object fsNode,
            object fsDesc,
            string pattern,
            string marker,
            IntPtr dst,
            uint size,
            out uint actual)
        {
            FspNodesPack pack = new FspNodesPack
            {
                data = dst,
                total = size,
            };
            getSubItems(fsNode as VfsNode, marker, pack);
            actual = pack.actual;
            return OK;
        }

        void getSubItems(
            VfsNode node,
            string marker,
            FspNodesPack pack)
        {
            if (marker == null)
            {
                node.items = getSubItems(node.path)
                            .GetEnumerator();
            }

            var items = node.items;
            if (items == null)
                return;

            VfsItem item;
            while (true)
            {
                item = items.Current;
                if (null != item
                    && !pack.add(item.path.locName(),
                                item.copyTo))
                    break;
                if (!items.MoveNext())
                {
                    node.items = null;
                    break;
                }
            }
        }

        List<VfsItem> getSubItems(string path)
        {
            var list = new List<VfsItem>();
            foreach (var d in rep.enumDirs(path))
                list.Add(new VfsItem
                {
                    path = d,
                    isDir = true,
                });
            foreach (var f in rep.enumFiles<VfsItem>(path))
                list.Add(f);
            return list;
        }

        VfsNode root = new VfsNode
        {
            path = RootPath,
            info = new FileInfo { FileAttributes = DirType }
        };

        public override int Open(
            string path,
            uint option,
            uint access,
            out object fsNode,
            out object fsDesc,
            out FileInfo info,
            out string unifyPath)
        {
            fsNode = null;
            fsDesc = null;
            info = new FileInfo();
            unifyPath = path;

            VfsNode node = null;
            if (path == RootPath)
                node = root;
            else
            {
                if (!getItem(path, out var item))
                {
                    return NotFound;
                }

                node = new VfsNode { path = path };
                item.copyTo(ref node.info);
            }
            node.open(out fsNode, out info);

            return OK;
        }

        public override int GetFileInfo(
            object fsNode,
            object fsDesc,
            out FileInfo info)
        {
            info = (fsNode as VfsNode).info;
            return OK;
        }

        public override int Read(
            object fsNode,
            object fsDesc,
            IntPtr dst,
            ulong off,
            uint count,
            out uint actual)
        {
            actual = (uint)read(fsNode as VfsNode,
                dst, (long)off, (int)count);
            return OK;
        }

        HashSet<BuffStream> opens = new HashSet<BuffStream>();

        public int read(
            VfsNode file,
            IntPtr dst,
            long offset,
            int count)
        {
            if (file.fs == null)
            {
                file.fs = new BuffStream
                {
                    rs = rep.readFile(file.path)
                };
                opens.Add(file.fs);
            }
            return file.fs.readTo(dst, offset, count);
        }

        public override void Close(
            object fsNode,
            object fsDesc)
        {
            var node = fsNode as VfsNode;
            if (node.fs != null)
            {
                opens.Remove(node.fs);
                this.free(ref node.fs);
            }
            node.close();
        }

        public void Dispose()
        {
            opens.each(fs => fs.Dispose());
            opens.Clear();
        }
    }
}
