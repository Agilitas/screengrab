using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ScreenGrab.Controls {
    class WindowOverlay : Adorner {
        public WindowOverlay(UIElement adornedElement) : base(adornedElement) { }

        protected override void OnRender(DrawingContext drawingContext) {

            Brush bg = new SolidColorBrush(Colors.Black);

            bg.Opacity = .35;
            drawingContext.DrawRectangle(bg, null, new Rect(new Point(0, 0), DesiredSize));
            base.OnRender(drawingContext);
        }
    }
}
