using ART_TELEMETRY_APP.Laps.Classes;
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
    /// Interaction logic for OnlyLapListElement.xaml
    /// </summary>
    public partial class OnlyLapListElement : UserControl
    {
        public OnlyLapListElement(Lap lap, int time_state, bool last_lap = false)
        {
            InitializeComponent();

            if (lap.Index == 0)
            {
                lap_lbl.Content = string.Format("In lap\t{0:D2}:{1:D2}:{2:D2}", lap.Time.Minutes, lap.Time.Seconds, lap.Time.Milliseconds);
            }
            else if (last_lap)
            {
                lap_lbl.Content = string.Format("Out lap\t{0:D2}:{1:D2}:{2:D2}", lap.Time.Minutes, lap.Time.Seconds, lap.Time.Milliseconds);
            }
            else
            {
                lap_lbl.Content = string.Format("{0}. lap\t{1:D2}:{2:D2}:{3:D2}", lap.Index, lap.Time.Minutes, lap.Time.Seconds, lap.Time.Milliseconds);
            }

            switch (time_state)
            {
                case 0:
                    main_grid.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#b51316"));
                    break;
                case 1:
                    main_grid.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#57b513"));
                    break;
                default:
                    break;
            }
        }

        public OnlyLapListElement(TimeSpan all_time)
        {
            InitializeComponent();

            lap_lbl.Content = string.Format("All lap\t{0:D2}:{1:D2}:{2:D2}", all_time.Minutes, all_time.Seconds, all_time.Milliseconds);
        }
    }
}
