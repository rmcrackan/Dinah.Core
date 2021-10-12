using System;
using System.IO;
using System.Threading;

namespace Dinah.Core.IO
{
    public static class FileExt
    {
        /// <summary>Delete file. No error when file does not exist.
        /// Exceptions are logged, not thrown.</summary>
        /// <param name="source">File to delete</param>
        public static void SafeDelete(string source)
        {
            if (!File.Exists(source))
                return;

            while (true)
            {
                try
                {
                    File.Delete(source);
					Serilog.Log.Logger.Information($"File successfully deleted: {source}");
                    break;
                }
                catch (Exception e)
                {
                    Thread.Sleep(100);
					Serilog.Log.Logger.Error(e, $"Failed to delete: {source}");
                }
            }
        }

        /// <summary>
		/// Moves a specified file to a new location, providing the option to specify a newfile name.
		/// Exceptions are logged, not thrown.
		/// </summary>
		/// <param name="source">The name of the file to move. Can include a relative or absolute path.</param>
		/// <param name="target">The new path and name for the file.</param>
		public static void SafeMove(string source, string target)
        {
            while (true)
            {
                try
                {
                    if (File.Exists(source))
                    {
                        File.Delete(target);
                        File.Move(source, target);
						Serilog.Log.Logger.Information($"File successfully moved from '{source}' to '{target}'");
					}

                    break;
                }
                catch (Exception e)
                {
                    Thread.Sleep(100);
					Serilog.Log.Logger.Error(e, $"Failed to move '{source}' to '{target}'");
                }
            }
        }

        public static void CreateFile(string file, byte[] bytes)
        {
			using var memoryStream = new MemoryStream(bytes);
			using var fileStream = File.Create(file);
			memoryStream.Seek(0, SeekOrigin.Begin);
			memoryStream.CopyTo(fileStream);
		}
    }
}
