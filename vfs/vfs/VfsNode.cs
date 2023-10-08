using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;
using FileInfo = Fsp.Interop.FileInfo;

namespace util.rep.vfs
{
    public class VfsNode
    {
        public string path;
        public FileInfo info;

        public BuffStream fs;
        public IEnumerator<VfsItem> items;

        public void open(out object fsNode, out FileInfo info)
        {
            fsNode = this;
            info = this.info;
        }

        public void close()
        {
            items = null;
        }
    }
}
