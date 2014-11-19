using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace perspectivePlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmapWrapper _bitmap;
        private WriteableBitmapWrapper _sourceImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private Float2 GetThumbPosition(Thumb thumb)
        {
            var x = Canvas.GetLeft(thumb);
            var y = Canvas.GetTop(thumb);
            return new Float2((float)x, (float)y);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _sourceImage = new WriteableBitmapWrapper(@"..\..\Sydney_New_Year's_Eve_2.jpg");
            SetThumbPosition(ThumbTopLeft, new Float2(0, 0));
            SetThumbPosition(ThumbTopRight, new Float2(_sourceImage.Width, 0));
            SetThumbPosition(ThumbBottomLeft, new Float2(0, _sourceImage.Height));
            SetThumbPosition(ThumbBottomRight, new Float2(_sourceImage.Width, _sourceImage.Height));
            _bitmap = new WriteableBitmapWrapper(_sourceImage.Width, _sourceImage.Height);
            Image.Source = _bitmap.Bitmap;
        }

        private void SetThumbPosition(Thumb thumb, Float2 pos)
        {
            Canvas.SetLeft(thumb, pos.X);
            Canvas.SetTop(thumb, pos.Y);
        }

        private void ThumbTopLeft_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
            Update();
        }

        private void Update()
        {
            var t1 = GetThumbPosition(ThumbTopLeft);
            var t2 = GetThumbPosition(ThumbTopRight);
            var t3 = GetThumbPosition(ThumbBottomLeft);
            var t4 = GetThumbPosition(ThumbBottomRight);
            var transform = Float3x3.Perspective(0, 0, _sourceImage.Width, _sourceImage.Height, t1, t2, t3, t4);
            transform = transform.Invert();
            for (var i = 0; i < 1; i++)
            {
                using (_bitmap.Edit())
                {
                    for (var x = 0; x < _sourceImage.Width; x++)
                    {
                        for (var y = 0; y < _sourceImage.Height; y++)
                        {
                            var tileCoord = transform.TransformPoint(new Float2(x, y));
                            var c = _sourceImage.GetPixelOrDefault(tileCoord);
                            _bitmap.SetPixel(x, y, c);
                        }
                    }
                }
            }
        }
    }
}