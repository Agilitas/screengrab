using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//using Rectangle = System.Drawing.Rectangle;

namespace ScreenGrab.Windows {
    /// <summary>
    /// Interaction logic for DesktopImage.xaml
    /// </summary>
    public partial class DesktopImage {

        public DesktopImage() {
            InitializeComponent();
            Loaded += DesktopImageLoaded;

            //var clip = new System.Drawing.Rectangle(0, 0, (int)this.Width, (int)this.Height);
            //Console.WriteLine("ClipCursor");
            //ClipCursor(ref clip);

        }

        private ImageSource _image;
        public ImageSource Image {
            set {
                _image = value;
                ImageDesktop.Width = _image.Width;
                ImageDesktop.Height = _image.Height;
                ImageDesktop.Background = new ImageBrush(_image);
            }
            get { return _image; }
        }

        public bool SelectOnMouseUp { get; set; }

        private ImageSource _returnedImage;
        public ImageSource ReturnedImage {
            get { return _returnedImage; }
            private set {
                _returnedImage = value;
            }
        }

        public Int32Rect ReturnedImageSource { get; private set; }

        void DesktopImageLoaded(object sender, RoutedEventArgs e) {
            ImageDesktop.PreviewMouseLeftButtonDown += ImageDesktopPreviewMouseLeftButtonDown;
            ImageDesktop.PreviewMouseLeftButtonUp += ImageDesktop_PreviewMouseLeftButtonUp;
            ImageDesktop.PreviewMouseMove += ImageDesktopPreviewMouseMove;

            PreviewKeyUp+=DesktopImagePreviewKeyUp;

        }


        private Point _startPoint;
        private Rectangle _rect;

        void ImageDesktopPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            _startPoint = e.GetPosition(ImageDesktop);

            RemoveRectangle();
            _rect = new Rectangle {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 1,
                Fill = FillColor
            };
            Canvas.SetLeft(_rect, _startPoint.X);
            Canvas.SetTop(_rect, _startPoint.X);
            ImageDesktop.Children.Add(_rect);
            //SelectionAttributes.Visibility=Visibility.Visible;
        }

        void ImageDesktopPreviewMouseMove(object sender, MouseEventArgs e) {

            var pos = e.GetPosition(ImageDesktop);

            if(pos.Y<0 || pos.X<0 || pos.Y > Height || pos.X > Width) {
                return;
            }

            Mouse.Capture(ImageDesktop);
            MousePosition.Text = pos.X + "," + pos.Y;
            Canvas.SetLeft(SelectionAttributes, pos.X);
            Canvas.SetTop(SelectionAttributes, pos.Y);

            if (e.LeftButton == MouseButtonState.Released || _rect == null)
                return;

            bool controlkey = (Keyboard.Modifiers & ModifierKeys.Control) > 0;

            var x = Math.Min(pos.X, _startPoint.X);
            var y = Math.Min(pos.Y, _startPoint.Y);

            var w = Math.Max(pos.X, _startPoint.X) - x;
            var h = Math.Max(pos.Y, _startPoint.Y) - y;

            if (controlkey) {
                //Snap to 10 pixels
                //var sml = Math.Min(w, h);
                w = SnapTo(w, 10.0);
                h = SnapTo(h, 10.0);
            }

            SelectionSize.Text = "(" + w + "," + h + ")";

            _rect.Width = w;
            _rect.Height = h;

            Canvas.SetLeft(_rect, x);
            Canvas.SetTop(_rect, y);
        }

        void ImageDesktop_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Mouse.Capture(null);
            //SelectionAttributes.Visibility = Visibility.Collapsed;
            if (AnythingSelected && SelectOnMouseUp) {
                GetSelectedImage();
            }
        }

        private void RemoveRectangle() {
            if (_rect != null) {
                ImageDesktop.Children.Remove(_rect);
            }
        }

        void DesktopImagePreviewKeyUp(object sender, KeyEventArgs e) {
            e.Handled = CheckKeyPress(e.Key);
        }

        bool CheckKeyPress(Key key) {

            Console.WriteLine(key);
            //var k = key;

            if (key == Key.Escape) {
                //RemoveRectangle();
                Visibility = Visibility.Hidden;
                return true;
                //e.Handled = true;
            }


            if (key == Key.Enter) {
                GetSelectedImage();
                return true;
            }

            return false;
        }


        bool AnythingSelected {
            get { return !(double.IsNaN(_rect.Width) || double.IsNaN(_rect.Height)); }
        }

        private Brush _fillColor;
        Brush FillColor {
            get { return _fillColor ?? (_fillColor = new SolidColorBrush(Color.FromArgb(125, 0, 0, 125))); }
        }


        double SnapTo(double value, double resolution) {

            var rem = value % resolution;
            var mid = resolution * 0.5;

            if (rem >= mid) {
                return (value - rem) + resolution;
            } else {
                return value - rem;
            }

        }

        void GetSelectedImage() {
            Int32Rect r;
            if (_rect == null) {
                r = new Int32Rect(0, 0, (int)Image.Width, (int)Image.Height);
            } else {
                int x = (int)Canvas.GetLeft(_rect);
                int y = (int)Canvas.GetTop(_rect);

                //this probably isn't needed anymore
                if (x < 0) {
                    x = 0;
                }
                if (x > Width) {
                    x = (int)Width;
                }
                if (y < 0) {
                    y = 0;
                }
                if (y > Height) {
                    y = (int)Height;
                }

                //_rect.cl

                r = new Int32Rect(x, y, (int)_rect.Width, (int)_rect.Height);
            }
            try {
                CroppedBitmap crop = new CroppedBitmap(Image as BitmapSource, r);
                ReturnedImage = crop;
                ReturnedImageSource = r;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            Visibility = Visibility.Hidden;
        }

        private void ImageDesktop_PreviewKeyUp(object sender, KeyEventArgs e) {
            var k = e.Key;

            if (k == Key.Escape) {
                //RemoveRectangle();
                Visibility = Visibility.Hidden;
            }


            if (!SelectOnMouseUp && k == Key.Enter) {
                GetSelectedImage();
            }

        }

        private void Window_KeyUp(object sender, KeyEventArgs e) {
            var k = e.Key;
        }

    }
}
