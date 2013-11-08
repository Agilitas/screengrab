using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenGrab.Controls {
    class CanvasWithOverlay : Canvas {

        public event AddHighlightEventHandler HighlightAdded = delegate { };

        #region dependency properties

        public static readonly DependencyProperty ImageProperty;
        public static readonly DependencyProperty HighlightColorProperty;
        public static readonly DependencyProperty HighlightsProperty;

        static CanvasWithOverlay() {
            ImageProperty = DependencyProperty.Register("Image",
                                                        typeof(ImageSource),
                                                        typeof(CanvasWithOverlay),
                                                        new FrameworkPropertyMetadata(ImagePropertyChangedCallback));

            HighlightColorProperty = DependencyProperty.Register("HighlightColor",
                                                        typeof(string),
                                                        typeof(CanvasWithOverlay));

            HighlightsProperty = DependencyProperty.Register("Highlights",
                                                             typeof(ObservableCollection<AnnotationHighlight>),
                                                             typeof(CanvasWithOverlay));
        }

        public CanvasWithOverlay() {
            Highlights = new ObservableCollection<AnnotationHighlight>();
        }

        private static void ImagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            //((IllustrationClients)d)._headerText.Text = e.NewValue.ToString();
            CanvasWithOverlay obj = (CanvasWithOverlay)d;
            //obj.RemoveAllHighlights();
            ImageSource img = e.NewValue as ImageSource;
            if (img == null) {
                obj.Background = null;
                return;
            }
            obj.Width = img.Width;
            obj.Height = img.Height;
            obj.Background = new ImageBrush(img);
            obj.RedrawHighlights();
        }

        public ImageSource Image {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public string HighlightColor {
            get { return (string)GetValue(HighlightColorProperty); }
            set { SetValue(HighlightColorProperty, value); }
        }

        public ObservableCollection<AnnotationHighlight> Highlights {
            get { return (ObservableCollection<AnnotationHighlight>)GetValue(HighlightsProperty); }
            set { SetValue(HighlightsProperty, value); }
        }


        Brush HighlightColorBrush {
            get { return (SolidColorBrush)new BrushConverter().ConvertFromString(HighlightColor); }
        }

        #endregion


        #region selection handling code

        private Point _startPoint;
        private Rectangle _currentRectangle;
        //private readonly List<AnnotationHighlight> _highlights = new List<AnnotationHighlight>();

        private AnnotationHighlight _highlightBeingAdded;

        void RedrawHighlights() {
            RemoveAllHighlights();
            if (Highlights == null) {
                return;
            }
            foreach (var hl in Highlights) {
                AddNewRectangle(hl.Color, hl.TopLeft.X, hl.TopLeft.Y, hl.Rectangle.Width, hl.Rectangle.Height);
            }
        }

        public void RemoveAllHighlights() {
            if (Highlights == null) {return;}
            Children.RemoveRange(0, Children.Count);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            _startPoint = e.GetPosition(this);

            _currentRectangle =  AddNewRectangle(HighlightColorBrush, _startPoint.X, _startPoint.Y);

            _highlightBeingAdded = new AnnotationHighlight {
                Rectangle = _currentRectangle,
                TopLeft = _startPoint,
                Color = HighlightColorBrush
            };

        }

        private Rectangle AddNewRectangle(Brush color, double x, double y, double w = 0, double h = 0) {
            var r = new Rectangle {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 1,
                Fill = color,
                Opacity = 0.5
            };
            SetLeft(r, x);
            SetTop(r, y);
            r.Width = w;
            r.Height = h;
            Children.Add(r);
            return r;
        }

        //void _currentRectangle_MouseEnter(object sender, MouseEventArgs e) {
        //    //Rectangle rect = sender as Rectangle;
        //    //rect.Opacity += 0.1;
        //}

        //void _currentRectangle_MouseLeave(object sender, MouseEventArgs e) {
        //    //Rectangle rect = sender as Rectangle;
        //    //rect.Opacity -= 0.1;
        //}

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            HighlightAdded(this, new AddHighlightEventArgs(_highlightBeingAdded));
            _highlightBeingAdded = null;
            //_currentRectangle.MouseEnter += _currentRectangle_MouseEnter;
            //_currentRectangle.MouseLeave += _currentRectangle_MouseLeave;
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Released || _currentRectangle == null)
                return;

            bool controlkey = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            var pos = e.GetPosition(this);
            var x = Math.Min(pos.X, _startPoint.X);
            var y = Math.Min(pos.Y, _startPoint.Y);
            var w = Math.Max(pos.X, _startPoint.X) - x;
            var h = Math.Max(pos.Y, _startPoint.Y) - y;

            if (controlkey) {
                //make square based on the smallest
                var sml = Math.Min(w, h);
                w = sml;
                h = sml;
            }
            _currentRectangle.Width = w;
            _currentRectangle.Height = h;

            SetLeft(_currentRectangle, x);
            SetTop(_currentRectangle, y);
        }

        #endregion

        //public Stream ImageAsPng() {

        //    DrawingVisual drawingVisual = new DrawingVisual();
        //    DrawingContext drawingContext = drawingVisual.RenderOpen();
        //    drawingContext.DrawImage(Image, new Rect(0, 0, Image.Width, Image.Height));

        //    //Brush rb
        //    drawingContext.PushOpacity(0.5);

        //    foreach (AnnotationHighlight highlight in Highlights) {
        //        var rectangle = highlight.Rectangle;
        //        drawingContext.DrawRectangle(highlight.Color, null, new Rect(GetLeft(rectangle), GetTop(rectangle), rectangle.Width, rectangle.Height));
        //    }

        //    drawingContext.Close();
        //    RenderTargetBitmap rtb = new RenderTargetBitmap((int)Image.Width, (int)Image.Height, 96, 96, PixelFormats.Pbgra32);
        //    rtb.Render(drawingVisual);

        //    PngBitmapEncoder encoder = new PngBitmapEncoder();
        //    MemoryStream stream = new MemoryStream();
        //    encoder.Frames.Add(BitmapFrame.Create(rtb));
        //    encoder.Save(stream);
        //    stream.Position = 0;
        //    return stream;
        //}


    }

    public delegate void AddHighlightEventHandler(object sender, AddHighlightEventArgs args);

    public class AddHighlightEventArgs : EventArgs {
        public AnnotationHighlight TheHighlight { get; private set; }
        public AddHighlightEventArgs(AnnotationHighlight theHighlight) {
            TheHighlight = theHighlight;
        }
    }
}
