using System;
using System.IO;

#nullable enable
namespace Dinah.Core.IO
{
    public class FileLogger
    {
        private string fileName { get; }

        public FileLogger(string logFolder = "Logs")
        {
            Directory.CreateDirectory(logFolder);

            string makeFileName() => Path.Combine(logFolder, "Debug_") + DateTime.Now.ToString("yyyyMMdd_HHmmssffff") + ".log";
            for (fileName = makeFileName(); File.Exists(fileName); fileName = makeFileName())
                System.Threading.Thread.Sleep(100);
        }

        // use with Console.WriteLine
        private object fileLocker { get; } = new object();
        public void TextWriterLogger(string? text)
        {
			lock (fileLocker)
			{
				using var file = new StreamWriter(fileName, true);
				file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + ": " + text);
			}
        }
    }
}
