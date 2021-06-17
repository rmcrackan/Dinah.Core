using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Dinah.Core.Drawing
{
	/// <summary>
	/// The name ImageConverter is already taken by System.Drawing
	/// </summary>
	public static class ImageReader
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

		public static byte[] ToBytes(this Image image, ImageFormat format)
		{
			using var ms = new System.IO.MemoryStream();
			{
				image.Save(ms, format);
				return ms.ToArray();
			}
		}
	}
}
