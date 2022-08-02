using System;
using System.Diagnostics;

namespace Dinah.Core
{
    public static class Go
    {
        public static bool IsWindows { get; } = OperatingSystem.IsWindows();
        public static bool IsLinux { get; } = OperatingSystem.IsLinux();
        public static bool IsMacOS { get; } = OperatingSystem.IsMacOS();

        public static class To
        {
            // from: https://stackoverflow.com/a/43232486
            /// <summary>Platform agnostic. Open your system's browser and go to url</summary>
            public static void Url(string url)
            {
                try
                {
                    Process.Start(url);
                }
                catch
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (IsWindows)
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else if (IsLinux)
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (IsMacOS)
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            /// <summary>Platform agnostic. Open folder, select file. If a path is a folder: open parent folder, select folder</summary>
            /// <param name="path"></param>
            /// <returns>False if file/folder not exist</returns>
            public static bool File(string path)
            {
                if (!System.IO.File.Exists(path) && !System.IO.Directory.Exists(path))
                    return false;

                if (IsWindows)
                {
                    Process.Start("explorer.exe", $"/select, \"{path}\"");
                    return true;
                }

                return Folder(System.IO.Path.GetDirectoryName(path));
            }

            /// <summary>Platform agnostic. Open folder</summary>
            /// <param name="path"></param>
            /// <returns>False if folder does not exist</returns>
            public static bool Folder(string path)
            {
                if (!System.IO.Directory.Exists(path))
                    return false;

                if (IsWindows)
                {
                    Process.Start("explorer.exe", $"\"{path}\"");
                    return true;
                }

                var fileName
                    = IsLinux ? "xdg-open"
                    : "open";

                var proc = Process.Start(new ProcessStartInfo()
                {
                    FileName = fileName,
                    Arguments = path is null ? string.Empty : $"\"{path}\"",
                    UseShellExecute = false,
                });
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
        }
    }
}
