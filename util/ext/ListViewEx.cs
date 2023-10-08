using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace util.ext
{
    public static class ListViewEx
    {
        public static void drawOnce(this ListView ui, Action func)
        {
            try
            {
                ui.BeginUpdate();
                func();
            }
            finally
            {
                ui.EndUpdate();
            }
        }

        public static void autoSpan(this ListView ui)
        {
            if (ui.Columns.Count > 0)
                ui.Columns[ui.Columns.Count - 1].Width = -2;
        }

        public static void autoSpan(this ColumnHeader header)
        {
            header.Width = -2;
        }

        public static bool isEmpty(this ListView list)
        {
            return list.Items.Count == 0;
        }

        public static ListViewItem selItem(this ListView list)
            => list.SelectedItems.Count == 0 ? null
            : list.SelectedItems[0];

        public static ListViewItem icon(this ListViewItem it, string key)
        {
            it.ImageKey = key;
            return it;
        }

        public static bool pick(this ListView list, out ListViewItem item)
        {
            return (item = list.selItem()) != null;
        }
    }
}
