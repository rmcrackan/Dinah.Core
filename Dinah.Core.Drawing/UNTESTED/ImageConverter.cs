using System;
using System.Collections.Generic;
using System.Drawing;

namespace Dinah.Core.Drawing
{
    public static class ImageConverter
    {
        private static Dictionary<byte[], Image> pictureCache = new Dictionary<byte[], Image>();

        public static Image GetPictureFromBytes(byte[] pictureBytes)
        {
            if (!pictureCache.ContainsKey(pictureBytes))
                using (var ms = new System.IO.MemoryStream(pictureBytes))
                    pictureCache.Add(pictureBytes, Image.FromStream(ms));

            return pictureCache[pictureBytes];
        }

        // these seemingly intuitive ways of loading an image will lock the file
        // - with a stream
        // - Image.FromFile()
        // use this method instead: https://stackoverflow.com/a/8701772
        public static Image GetPictureFromFile(string filepath)
        {
            using (var bmpTemp = new Bitmap(filepath))
                return new Bitmap(bmpTemp);
        }
    }
}
