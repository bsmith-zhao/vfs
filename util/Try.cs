using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.ext
{
    public static class Try
    {
        public static Action<Exception> notify = showMessage;

        static void showMessage(Exception err)
            => err.Message.msg();

        public static void handleError(Exception err,
            Action<Exception> notify = null)
        {
            err.log();
            (notify ?? Try.notify)?.Invoke(err);
        }

        public static void trydo(this object obj, Action func,
            Action<Exception> notify = null)
        {
            try { func(); }
            catch (Exception err)
            {
                handleError(err);
            }
        }

        public static T tryget<T>(this object obj, Func<T> func,
            Action<Exception> notify = null)
        {
            try { return func(); }
            catch (Exception err)
            {
                handleError(err);
            }
            return default(T);
        }
    }
}
