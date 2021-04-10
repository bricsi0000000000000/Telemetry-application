using System.Windows.Media;

namespace PresentationLayer.Converters
{
    public static class ConvertColor
    {
        public static SolidColorBrush ConvertStringColorToSolidColorBrush(string colorCode) =>
            new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
    }
}
