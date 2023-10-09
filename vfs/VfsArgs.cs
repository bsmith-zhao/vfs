using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util;
using util.ext;
using util.rep;
using util.rep.aead;

namespace vfs
{
    public class VfsArgs
    {
        public RepType type;
        public string rep;
        public string pwd = "";
        public string mount = "V:";

        public VfsDrive create()
        {
            if (mount.dirExist())
                throw new Error<VfsDrive>("MountExist", mount);

            Reposit rep = null;
            if (type == RepType.Folder)
                rep = new DirReposit(this.rep);
            else if (type == RepType.AeadFS)
                rep = new AeadReposit(this.rep)
                {
                    pwd = pwd.utf8(),
                };
            rep.open();

            return new VfsDrive
            {
                path = mount,
                rep = rep,
            };
        }
    }
}
