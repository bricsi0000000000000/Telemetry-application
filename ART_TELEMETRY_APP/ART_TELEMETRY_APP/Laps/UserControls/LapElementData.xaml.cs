using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Laps.UserControls
{
    /// <summary>
    /// Interaction logic for LapElementData.xaml
    /// </summary>
    public partial class LapElementData : UserControl
    {
        private readonly BrushConverter converter = new BrushConverter();

        public LapElementData(string fileName, string driverName, TimeSpan lapTime, LapState lapState)
        {
            InitializeComponent();

            NameLbl.Content = $"{driverName} - {fileName}";
            TimeLbl.Content = $"{lapTime.Minutes:00}:{lapTime.Seconds:00}:{lapTime.Milliseconds:00}";

            switch (lapState)
            {
                case LapState.Best:
                    BackgroundCard.Background = (Brush)converter.ConvertFromString("#14b719");
                    break;
                case LapState.Worst:
                    BackgroundCard.Background = (Brush)converter.ConvertFromString("#b51f17");
                    break;
            }
        }
    }
}
