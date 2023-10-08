using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using util.ext;

namespace util
{
    public static class Dir
    {
        public static string AppTrunk => AppPath.locTrunk();
        public static string AppPath => Application.ExecutablePath.locUnify();
        public static string AppDir => Application.StartupPath.locUnify();

        public static bool dirExist(this string dir)
            => !dir.empty() && Directory.Exists(dir);

        public static void dirCreate(this string path)
        {
            Directory.CreateDirectory(path);
        }

        public static void dirDelete(this string path, bool recursive = false)
        {
            Directory.Delete(path, recursive);
        }

        public static void dirOpen(this string dir)
        {
            dir = dir.Replace("/", "\\");
            Process.Start("explorer.exe", dir);
        }

        public static void drvSpace(this string path, out long total, out long free)
        {
            var di = new DriveInfo(path);
            total = di.TotalSize;
            free = di.AvailableFreeSpace;
        }
    }
}
