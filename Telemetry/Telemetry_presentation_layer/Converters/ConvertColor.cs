
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;

namespace Telemetry_presentation_layer.Converters
{
    public static class ConvertColor
    {
        public static SolidColorBrush ConvertStringColorToSolidColorBrush(string colorCode) =>
            new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
    }
}
