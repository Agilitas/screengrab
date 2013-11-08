using System.Windows;
using Brush = System.Windows.Media.Brush;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace ScreenGrab {
    public class AnnotationHighlight {

        public Rectangle Rectangle { get; set; }
        public Point TopLeft { get; set; }
        public Brush Color { get; set; }

    }
}