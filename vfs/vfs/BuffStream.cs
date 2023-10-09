using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using util.ext;

namespace vfs
{
    public class BuffStream : IDisposable
    {
        public Stream rs;

        public int readTo(
            IntPtr dst,
            long offset,
            int count)
        {
            rs.Position = offset;
            var buff = getBuff(count);
            var actual = rs.Read(buff, 0, count);
            if (actual > 0)
                Marshal.Copy(buff, 0, dst, actual);
            return actual;
        }

        byte[] bf;
        byte[] getBuff(int size)
            => (bf?.Length >= size) ? bf
            : (bf = new byte[size]);

        public void Dispose()
        {
            this.free(ref rs);
        }
    }
}
