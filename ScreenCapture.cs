using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenGrab {
    public class ScreenCapture {

        public ImageSource Source { get; private set; }
        public string DateCaptured { get; private set; }

        public Screen SourceScreen { get; set; }
        public Int32Rect SourcePosition { get; set; }

        private ObservableCollection<AnnotationHighlight> _highlights;
        public ObservableCollection<AnnotationHighlight> Highlights {
            get {
                if (_highlights == null) {
                    _highlights = new ObservableCollection<AnnotationHighlight>();
                }
                return _highlights;
            }
            set { _highlights = value; }
        }

        public ScreenCapture(ImageSource source) {
            Source = source;
            DateCaptured = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Highlights = new ObservableCollection<AnnotationHighlight>();
        }

        public Stream ImageAsPng() {

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(Source as BitmapSource));

            encoder.Save(stream);
            return stream;
        }

        //public Stream ImageAsJpg2() {
        //    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
        //    MemoryStream stream = new MemoryStream();
        //    encoder.Frames.Add(BitmapFrame.Create(Source as BitmapSource));
        //    encoder.Save(stream);
        //    return stream;
        //}

        public Stream ImageAsJpg() {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(Source, new Rect(0, 0, Source.Width, Source.Height));

            //Brush rb
            drawingContext.PushOpacity(0.5);

            foreach (AnnotationHighlight highlight in Highlights) {
                var rectangle = highlight.Rectangle;
                drawingContext.DrawRectangle(highlight.Color, null, new Rect(highlight.TopLeft.X, highlight.TopLeft.Y, rectangle.Width, rectangle.Height));
            }

            drawingContext.Close();
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Source.Width, (int)Source.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = 75;
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);
            stream.Position = 0;
            return stream;

        }

        private const double ThMax = 110;

        Rect ImageDimensions() {
            return new Rect(0, 0, Source.Width, Source.Height);
        }

        BitmapImage GetThumbnail() {

            var d = ImageDimensions();

            double tw;
            double th;
            double ratio = (d.Width / d.Height);
            if (d.Width >= d.Height) {
                //landscape
                tw = ThMax;
                th = ThMax / ratio;
            } else {
                //portrait
                th = ThMax;
                tw = ThMax * ratio;

            }

            Image.GetThumbnailImageAbort myCallback = () => false;
            Bitmap myBitmap = new Bitmap(ImageAsPng());
            Image myThumbnail = myBitmap.GetThumbnailImage((int)tw, (int)th, myCallback, IntPtr.Zero);

            MemoryStream ms = new MemoryStream();
            myThumbnail.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }

        private BitmapImage _thumbnail;
        public BitmapImage Thumbnail {
            get { return _thumbnail ?? (_thumbnail = GetThumbnail()); }
        }


    }
}