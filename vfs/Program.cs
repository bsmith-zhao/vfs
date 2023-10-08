using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util;
using util.ext;

namespace vfs
{
    class Program
    {
        static void Main(string[] args)
        {
            Msg.output = Console.WriteLine;
            try
            {
                if (args.Length == 0)
                    throw new Error(typeof(VfsApp), "EmptyArgs");

                var vfs = args[0].b64().utf8().obj<VfsApp>();
                vfs.mount();
            }
            catch (Error err)
            {
                Console.Error.WriteLine(err.Message);
                return;
            }
            catch (Exception err)
            {
                Console.Error.WriteLine(err.Message);
                return;
            }

            string cmd;
            while ((cmd = Console.ReadLine()) != null)
            {
                if (cmd == "exit")
                    break;
            }
        }
    }
}
