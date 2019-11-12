using System;
using System.Collections.Generic;
using System.Drawing;

namespace Dinah.Core.Drawing
{
	public static class ImageCache
	{
		private static Dictionary<byte[], Image> pictureCache { get; } = new Dictionary<byte[], Image>();

		public static Image GetPicture(byte[] pictureBytes)
		{
			if (!pictureCache.ContainsKey(pictureBytes))
			{
				var img = ImageConverter.ToImage(pictureBytes);
				pictureCache.Add(pictureBytes, img);
			}

			return pictureCache[pictureBytes];
		}
	}
}
