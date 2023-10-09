using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.ext
{
    public static class ExceptionEx
    {
        public static Error conv(this Exception err, 
            Type type, string item)
            => new Error(type, item, err.Message);

        public static Error conv<T>(this Exception err, string item)
            => new Error<T>(item, err.Message);
    }
}
