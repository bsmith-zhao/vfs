using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.ext
{
    public static class Try
    {
        public static Action<Exception> notify = showError;

        static void showError(Exception err)
            => err.Message.msg();

        public static void procError(Exception err)
        {
            err.log();
            notify?.Invoke(err);
        }

        static void handle(Exception err,
            Action<Exception> notify)
            => (notify ?? procError).Invoke(err);

        public static void trydo(this object src, Action func,
            Action<Exception> notify = null)
        {
            try { func(); }
            catch (Exception err)
            {
                handle(err, notify);
            }
        }

        public static T tryget<T>(this object src, Func<T> func,
            Action<Exception> notify = null)
        {
            try { return func(); }
            catch (Exception err)
            {
                handle(err, notify);
            }
            return default(T);
        }
    }
}
