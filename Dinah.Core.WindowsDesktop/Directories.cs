using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Dinah.Core.WindowsDesktop
{
    // https://github.com/dimuththarindu/FIC-Folder-Icon-Changer/blob/master/project/FIC/Classes/IconCustomizer.cs
    public static class Directories
    {
        public enum FolderTypes { Generic, Documents, Pictures, Music, Videos }

        public static void SetIcon(this DirectoryInfo directoryInfo, string icoPath, FolderTypes folderType = FolderTypes.Generic)
            => SetIcon(directoryInfo.FullName, icoPath, folderType);
        public static void SetIcon(string dir, string icoPath, FolderTypes folderType = FolderTypes.Generic)
        {
            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();

            var desktop_ini = Path.Combine(dir, "desktop.ini");
            var Icon_ico = Path.Combine(dir, "Icon.ico");
            var hidden = Path.Combine(dir, ".hidden");

            //deleting existing files
            DeleteIcon(dir);

            //copying Icon file //overwriting
            File.Copy(icoPath, Icon_ico, true);

            //writing configuration file
            string[] desktopLines = { "[.ShellClassInfo]", "IconResource=Icon.ico,0", "[ViewState]", "Mode=", "Vid=", $"FolderType={folderType}" };
            File.WriteAllLines(desktop_ini, desktopLines);

            //configure file 2            
            string[] hiddenLines = { "desktop.ini", "Icon.ico" };
            File.WriteAllLines(hidden, hiddenLines);

            //making system files
            File.SetAttributes(desktop_ini, File.GetAttributes(desktop_ini) | FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReadOnly);
            File.SetAttributes(Icon_ico, File.GetAttributes(Icon_ico) | FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReadOnly);
            File.SetAttributes(hidden, File.GetAttributes(hidden) | FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReadOnly);

            //File.SetAttributes(dir, File.GetAttributes(dir) | FileAttributes.ReadOnly);

            refresh();
        }

        public static void DeleteIcon(this DirectoryInfo directoryInfo) => DeleteIcon(directoryInfo.FullName);
        public static void DeleteIcon(string dir)
        {
            if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();

            var files = new[] { "desktop.ini", "Icon.ico", ".hidden" };
            foreach (var f in files)
            {
                var path = Path.Combine(dir, f);
                if (!File.Exists(path))
                    continue;

                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Normal);
                _ = new FileInfo(path) { IsReadOnly = false };
                File.Delete(path);
            }
            refresh();
        }

        private static void refresh() => SHChangeNotify(0x08000000, 0x0000, (IntPtr)null, (IntPtr)null); //SHCNE_ASSOCCHANGED SHCNF_IDLIST

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
