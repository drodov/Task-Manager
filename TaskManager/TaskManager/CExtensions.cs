using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;


namespace TaskManager
{
    /// <summary>
    /// Class for extension methods.
    /// </summary>
    public static class CExtensions
    {
        /// <summary>
        /// Converting System.Drawing.Bitmap to System.Windows.Media.Imaging.BitmapImage.
        /// </summary>
        /// <param name="bitmap">Image for converting.</param>
        /// <returns>Coverted image.</returns>
        public static BitmapImage ToBitmapImage(this System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
