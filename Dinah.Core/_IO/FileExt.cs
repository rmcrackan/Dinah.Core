using System;
using System.IO;
using System.Threading;

namespace Dinah.Core.IO
{
    public static class FileExt
    {
        public static void SafeDelete(string source)
        {
            while (true)
            {
                try
                {
                    // deletes file if it exists. no error if it doesn't exist
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
