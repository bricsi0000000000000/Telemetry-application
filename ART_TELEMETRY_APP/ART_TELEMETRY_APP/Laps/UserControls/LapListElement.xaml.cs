using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings.Classes;
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

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// Interaction logic for LapListElement.xaml
    /// </summary>
    public partial class LapListElement : UserControl
    {
        Lap lap;
        string pilots_name;
        List<string> channels;
        bool active = false;
        string group_name;
        float kalman_sensitivity = 0.2f;

        /// <summary>
        /// 0=worst, 1=best, 2=none
        /// </summary>
        /// <param name="lap"></param>
        /// <param name="pilots_name"></param>
        /// <param name="active"></param>
        /// <param name="channels"></param>
        /// <param name="time_state"></param>
        /// <param name="last_lap"></param>
        public LapListElement(Lap lap, string pilots_name, bool active, List<string> channels, int time_state, string group_name, bool last_lap = false)
        {
            InitializeComponent();

            this.pilots_name = pilots_name;
            this.lap = lap;
            this.channels = channels;
            this.group_name = group_name;

            Active = active;
            check_icon.Kind = active ?
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBox :
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline;

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

        private void checkLap_Click(object sender, RoutedEventArgs e)
        {
            Active = !Active;

            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(group_name).Content).InitLapListElements();
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(group_name).Content).BuildCharts();
        }

        private void settingsLap_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LapChannels lap_channels_window = new LapChannels(lap, channels, pilots_name, group_name);
            lap_channels_window.Show();
        }

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                check_icon.Kind = check_icon.Kind == MaterialDesignThemes.Wpf.PackIconKind.CheckBox ?
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline :
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBox;
                active = value;
            }
        }

        public float KalmanSensitivity { get => kalman_sensitivity; set => kalman_sensitivity = value; }
    }
}
