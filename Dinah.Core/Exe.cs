namespace Dinah.Core
{
    public static class Exe
    {
        /// <summary>Location of Dinah.Core.dll</summary>
        public static string FileLocationOnDisk => System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("file:///", "");
    }
}
