using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Laps.UserControls
{
    /// <summary>
    /// Interaction logic for LapListElement.xaml
    /// </summary>
    public partial class LapListElement : UserControl
    {
        private readonly Lap lap;
        private readonly string pilots_name;
        private readonly List<string> channels;
        private readonly List<string> selected_channels;
        private readonly string group_name;
        private bool active = false;
        public float KalmanSensitivity { get; set; } = 0.2f;

        /// <summary>
        /// 0=worst, 1=best, 2=none
        /// </summary>
        /// <param name="lap"></param>
        /// <param name="pilots_name"></param>
        /// <param name="active"></param>
        /// <param name="channels"></param>
        /// <param name="time_state"></param>
        /// <param name="last_lap"></param>
        public LapListElement(Lap lap,
                              string pilots_name,
                              bool active,
                              List<string> channels,
                              List<string> selected_channels,
                              int time_state,
                              string group_name,
                              /*Brush lapColor,*/
                              bool last_lap = false
                             )
        {
            InitializeComponent();

            this.pilots_name = pilots_name;
            this.lap = lap;
            this.channels = channels;
            this.selected_channels = selected_channels;
            this.group_name = group_name;

           /* BackgroundCard.Background = lapColor;*/

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

           // ((LapsContent)((DriverContentTab)((DiagramsMenu)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(group_name).Content).InitLapListElements();
           // ((LapsContent)((DriverContentTab)((DiagramsMenu)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(group_name).Content).BuildCharts();
        }

        private void settingsLap_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LapChannels lap_channels_window = new LapChannels(lap, channels, selected_channels, pilots_name, group_name);
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

    }
}
