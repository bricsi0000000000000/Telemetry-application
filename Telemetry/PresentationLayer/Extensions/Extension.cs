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
