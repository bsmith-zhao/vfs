using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using util.ext;

namespace util.rep
{
    public abstract class DirItem
    {
        public string path;
        public abstract IEnumerable<DirItem> enumDirs();
        public abstract IEnumerable<T> enumFiles<T>() 
            where T : FileItem, new();

        public IEnumerable<DirItem> enumAllDirs(PathConv conv = null)
        {
            var stack = new Stack<IEnumerable<DirItem>>();
            stack.Push(enumDirs());
            while (stack.Count > 0)
            {
                foreach (var di in stack.Pop())
                {
                    if (conv == null || (di.path = conv(di.path)) != null)
                    {
                        yield return di;
                        stack.Push(di.enumDirs());
                    }
                }
            }
        }
    }
}
