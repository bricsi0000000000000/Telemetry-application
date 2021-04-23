using LocigLayer.Texts;
using System.IO;
using System.Windows.Media;

namespace PresentationLayer.Extensions
{
    public static class Extension
    {
        public static Color ConvertColor(this string colorText)
        {
            return (Color)ColorConverter.ConvertFromString(colorText);
        }

        public static System.Drawing.Color ConvertToChartColor(this string colorText)
        {
            var color = colorText.ConvertColor();
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }


        public static SolidColorBrush ConvertBrush(this string colorText)
        {
            return new SolidColorBrush(colorText.ConvertColor());
        }

        public static string MakePath(this string path, string folder)
        {
            return Path.Combine($"{TextManager.RootDirectory}/{folder}", path);
        }
    }
}
