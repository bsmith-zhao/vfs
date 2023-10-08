using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util
{
    public class Error : Exception
    {
        public Type type;
        public string act;
        public object[] args;

        public Error(object obj, string act, params object[] args)
        {
            this.type = obj.GetType();
            this.act = act;
            this.args = args;
        }

        public Error(Type type, string act, params object[] args)
        {
            this.type = type;
            this.act = act;
            this.args = args;
        }

        public override string Message 
            => type.trans(act, args);
    }

    public class Error<T> : Error
    {
        public Error(string act, params object[] args)
            : base(typeof(T), act, args)
        {
        }
    }
}
