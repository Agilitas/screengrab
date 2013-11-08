using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ScreenGrab.Windows {
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class ColorBrushConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return new BrushConverter().ConvertFromString(value.ToString());
            //string color = value.ToString();
            //Color c = Color.FromName(color);

            //var c = Color.FromName()
            //var x = new SolidColorBrush(Color.FromName(color));
            //return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return null;
        }
    }
}