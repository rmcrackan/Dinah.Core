using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Dinah.Core.WindowsDesktop.Drawing
{
	/// <summary>
	/// The name ImageConverter is already taken by System.Drawing
	/// </summary>
	public static class ImageReader
    {
        // these seemingly intuitive ways of loading an image can lock the file
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
            using var memoryStream = new MemoryStream(bytes);
            return Image.FromStream(memoryStream);
        }

        public static byte[] ToBytes(this Image image, ImageFormat format)
        {
            using var ms = new MemoryStream();
            image.Save(ms, format);
            return ms.ToArray();
        }

        public static Icon ToIcon(string filepath) => ToImage(filepath).ToIcon();

        // https://stackoverflow.com/a/21389253
        public static Icon ToIcon(this Image img)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            // Header
            bw.Write((short)0);   // 0-1 : reserved
            bw.Write((short)1);   // 2-3 : 1=ico, 2=cur
            bw.Write((short)1);   // 4-5 : number of images
                                  // Image directory
            var w = img.Width;
            if (w >= 256) w = 0;
            bw.Write((byte)w);    // 0 : width of image
            var h = img.Height;
            if (h >= 256) h = 0;
            bw.Write((byte)h);    // 1 : height of image
            bw.Write((byte)0);    // 2 : number of colors in palette
            bw.Write((byte)0);    // 3 : reserved
            bw.Write((short)0);   // 4 : number of color planes
            bw.Write((short)0);   // 6 : bits per pixel
            var sizeHere = ms.Position;
            bw.Write((int)0);     // 8 : image size
            var start = (int)ms.Position + 4;
            bw.Write(start);      // 12: offset of image data
                                  // Image data
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var imageSize = (int)ms.Position - start;
            ms.Seek(sizeHere, SeekOrigin.Begin);
            bw.Write(imageSize);
            ms.Seek(0, SeekOrigin.Begin);

            // And load it
            return new Icon(ms);
        }

        public static void Save(this Icon icon, string path)
        {
            using var fs = new FileStream(path, FileMode.Create);
            icon.Save(fs);
        }
    }
}
