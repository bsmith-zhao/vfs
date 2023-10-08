using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;
using util.rep;
using util.rep.aead;
using util.rep.vfs;

namespace vfs
{
    public class VfsApp
    {
        public RepType Type;
        public string Path;
        public string Pwd = "";
        public string Mount = "V:";
        public VfsDrive drive;

        public void mount()
        {
            Reposit rep = null;
            if (Type == RepType.Folder)
                rep = new DirReposit(Path);
            else if (Type == RepType.AeadFS)
                rep = new AeadReposit(Path)
                {
                    pwd = Pwd.utf8(),
                };
            rep.open();

            (drive = new VfsDrive
            {
                path = Mount,
                rep = rep,
            }).mount();
        }
    }
}
