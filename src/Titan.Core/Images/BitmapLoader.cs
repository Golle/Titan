using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Titan.Core.Images
{
    internal class BitmapLoader : IBitmapLoader
    {
        public ImageAsset Load(string filename)
        {
            var image = Image.FromFile(filename);
            var bitmap = new Bitmap(image);
            
            var data = bitmap.LockBits(new Rectangle(0,0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            var size = data.Height * data.Stride;
            var bytes = new byte[size];
            Marshal.Copy(data.Scan0, bytes, 0, size);
            bitmap.UnlockBits(data);
            
            return new ImageAsset
            {
                Height = (uint) data.Height,
                Width = (uint) data.Width,
                Bytes = bytes,
                PixelFormat = bitmap.PixelFormat
            };
        }
    }
}
