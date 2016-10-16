using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace ImageResizer
{
	class MainClass
	{
		private static float scale;

		public static void Main (string [] args)
		{
			Assembly myAssembly = Assembly.GetEntryAssembly ();
			var exePath = Path.GetDirectoryName (myAssembly.Location);
			var scaleTxtPath = Path.Combine (exePath, @"image_resize_scale.txt");
			if (File.Exists (scaleTxtPath)) {
				var readScale = File.ReadAllText (scaleTxtPath);
				if (!float.TryParse (readScale, out scale)) {
					scale = 0.5f;
				}
			} else {
				scale = 0.5f;
			}

			Console.WriteLine (String.Format (@"Image Resizing!: scale {0}", scale));

			foreach (string arg in args) {
				searchFile (arg);
			}

			Console.WriteLine (@"Done!");
			Console.ReadKey ();
		}

		private static void searchFile (String path)
		{
			if (Directory.Exists (path)) {
				var filePaths = Directory.EnumerateFileSystemEntries (path);
				foreach (string filePath in filePaths) {
					searchFile (filePath);
				}
			} else if (File.Exists (path)) {
				var extension = Path.GetExtension (path);
				if (extension.ToLower () == @".png" ||
					extension.ToLower () == @".jpg" ||
					extension.ToLower () == @".jpeg" ||
					extension.ToLower () == @".bmp" ||
					extension.ToLower () == @".gif") {
					resize (path);
				}
			}
		}

		private static void resize (String path)
		{
			Console.WriteLine (String.Format (@"Resize: {0}", path));

			var directoryName = Path.GetDirectoryName (path);
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension (path);

			var baseImage = new Bitmap (path);
			var targetHeight = (int)Math.Round (baseImage.Height * scale);
			var targetWidth = (int)Math.Round (baseImage.Width * scale);

			var resizeBitmap = new Bitmap (targetWidth, targetHeight);
			var g = Graphics.FromImage (resizeBitmap);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			g.DrawImage (baseImage, 0, 0, targetWidth, targetHeight);

			// output
			resizeBitmap.Save (Path.Combine (directoryName, fileNameWithoutExtension + String.Format (@"_{0}x{1}.png", targetWidth, targetHeight)), ImageFormat.Png);

			g.Dispose ();
			resizeBitmap.Dispose ();
			baseImage.Dispose ();
		}
	}
}
