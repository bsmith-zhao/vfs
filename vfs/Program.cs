using System;
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
                    throw new Error<VfsDrive>("EmptyArgs");

                var vfs = args[0].b64().utf8()
                    .obj<VfsArgs>().create();
                vfs.mount();
            }
            catch (Error err)
            {
                Console.Error.WriteLine(err.Json);
                return;
            }
            catch (Exception err)
            {
                Console.Error.WriteLine(err.toError<VfsDrive>("Error").Json);
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
