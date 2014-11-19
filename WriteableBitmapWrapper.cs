using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace perspectivePlayground
{
    public class WriteableBitmapWrapper
    {
        public readonly WriteableBitmap Bitmap;

        public WriteableBitmapWrapper(int w, int h)
        {
            Bitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgra32, null);
            Height = Bitmap.PixelHeight;
            Width = Bitmap.PixelWidth;

        }

        public WriteableBitmapWrapper(string path)
        {
            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            BitmapSource b = src;
            if (src.Format != PixelFormats.Bgra32) 
                b = new FormatConvertedBitmap(b, PixelFormats.Bgra32, null, 0);

            Bitmap = new WriteableBitmap(b);
            Height = Bitmap.PixelHeight;
            Width = Bitmap.PixelWidth;
        }

        public int Height { get; private set; }

        public int Width { get; private set; }


        public int GetPixel(int x, int y)
        {
            var offset = y * Bitmap.BackBufferStride + x * 4;
            var value = Marshal.ReadInt32(Bitmap.BackBuffer, offset);
            return value;
        }

        public IDisposable Edit()
        {
            Bitmap.Lock();
            return new LambdaDisposable(
                () =>
                    {
                        Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                        Bitmap.Unlock();
                    });
        }

        public void SetPixel(int x, int y, int color)
        {
            var offset = y * Bitmap.BackBufferStride + x * 4;
            Marshal.WriteInt32(Bitmap.BackBuffer, offset, color);
        }

        public int GetPixelOrDefault(Float2 point)
        {
            return GetPixelOrDefault((int)point.X, (int)point.Y);
        }

        public int GetPixelOrDefault(int x, int y)
        {
            if (x < 0 || x >= Width) { return 0;}
            if (y < 0 || y >= Height) { return 0; }
            return GetPixel(x, y);
        }
    }
}