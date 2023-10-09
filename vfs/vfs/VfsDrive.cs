using Fsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util;
using util.ext;
using util.rep;

namespace vfs
{
    public class VfsDrive
    {
        public Reposit rep;
        public string path;
        public string label = "vfs";

        FileSystemHost host;
        VfsFileBase vfs;

        public void mount()
        {
            if (host != null)
                return;

            host = new FileSystemHost(vfs = new VfsFileBase
            {
                rep = rep,
                volumeLabel = label,
            });
            host.SectorSize = 4096;
            host.SectorsPerAllocationUnit = 1;
            host.VolumeCreationTime = (UInt64)DateTime.Now.ToFileTimeUtc();
            host.VolumeSerialNumber = (UInt32)(host.VolumeCreationTime / (10000 * 1000));
            host.CaseSensitiveSearch = true;
            host.CasePreservedNames = true;
            host.UnicodeOnDisk = true;
            host.ReadOnlyVolume = true;

            int err = host.Mount(path, Synchronized: true);
            if (err < 0)
                throw new Error(this, "MountFail", err);
        }

        public void unmount()
        {
            this.free(ref host)
                .free(ref vfs);
        }
    }
}
