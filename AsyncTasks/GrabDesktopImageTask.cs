using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ScreenGrab.AsyncTasks {
    class GrabDesktopImageTask : IDisposable {

        public Screen SelectedScreen { get; private set; }

        public GrabDesktopImageTask(Screen selectedScreen) {
            SelectedScreen = selectedScreen;
        }

        private Dispatcher _dispatcher;
        public BitmapSource Image { get; private set; }
        public event EventHandler Finished;

        public void Run() {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Thread t = new Thread(DoTask) { IsBackground = true };
            t.Start();
        }

        void DoTask() {
            Thread.Sleep(250);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Bitmap screen = new Bitmap(SelectedScreen.Bounds.Width, SelectedScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            // float dpi = 96F; //
            // float dpi = 157F * 1.5F;
            // screen.SetResolution(dpi, dpi);
            using (Graphics g = Graphics.FromImage(screen)) {
                //g.CopyFromScreen(0, 0, 0, 0, screen.Size);
                g.CopyFromScreen(SelectedScreen.Bounds.X, SelectedScreen.Bounds.Y, 0, 0, screen.Size, CopyPixelOperation.SourceCopy);
            }
            //Console.WriteLine("CopyFromScreen: {0}", sw.ElapsedMilliseconds);
            Image = BitmapToSource(screen);
            sw.Stop();
            //Console.WriteLine("TimeToGrab: {0}", sw.ElapsedMilliseconds);
            _dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => Finished(this, EventArgs.Empty)));

        }

        private BitmapSource BitmapToSource(Bitmap bitmap) {

            // Transform the image for CaptureScreen method
            // if (bitmap == null)
            //     throw new ArgumentNullException("bitmap");
            //
            // var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //
            // var bitmapData = bitmap.LockBits(
            //     rect,
            //     ImageLockMode.ReadWrite,
            //     System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //
            // try {
            //     var size = (rect.Width * rect.Height) * 4;
            //
            //     return BitmapSource.Create(
            //         bitmap.Width,
            //         bitmap.Height,
            //         bitmap.HorizontalResolution,
            //         bitmap.VerticalResolution,
            //         PixelFormats.Bgra32,
            //         null,
            //         bitmapData.Scan0,
            //         size,
            //         bitmapData.Stride);
            // } finally {
            //     bitmap.UnlockBits(bitmapData);
            // }
            IntPtr hBitmap = bitmap.GetHbitmap();
            //BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height);
            // sizeOptions.PixelHeight = bitmap.Height;
            BitmapSource destination = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            destination.Freeze();
            return destination;
        }


        public void Dispose() {
            Image = null;
        }
    }
}