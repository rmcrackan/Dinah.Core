using System;
using System.Drawing;

namespace Dinah.Core.Drawing
{
    public static class ImageConverter
    {
        // these seemingly intuitive ways of loading an image will lock the file
        // - with a stream
        // - Image.FromFile()
        // use this method instead: https://stackoverflow.com/a/8701772
        public static Image ToImage(string filepath)
        {
			using var bmpTemp = new Bitmap(filepath);
			return new Bitmap(bmpTemp);
		}

		public static Image ToImage(byte[] bytes)
		{
			using var memoryStream = new System.IO.MemoryStream(bytes);
			return Image.FromStream(memoryStream);
		}
	}
}
