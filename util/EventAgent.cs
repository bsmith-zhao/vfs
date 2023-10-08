using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util
{
    public class EventAgent<T> where T : EventArgs
    {
        Action<object, T> disp;
        public EventAgent(Action<object, T> disp)
        {
            this.disp = disp;
        }

        bool stop = false;
        public void suspend(Action func)
        {
            try
            {
                stop = true;
                func();
            }
            finally
            {
                stop = false;
            }
        }

        public void trigger(object s, T e)
        {
            if (stop)
                return;
            disp(s, e);
        }
    }
}
