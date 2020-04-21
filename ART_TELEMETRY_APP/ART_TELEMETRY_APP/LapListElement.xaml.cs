using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
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
        public bool Active = false;
        List<string> channels;

        public LapListElement(Lap lap, string pilots_name, bool active, List<string> channels)
        {
            InitializeComponent();

            this.pilots_name = pilots_name;
            this.lap = lap;
            this.channels = channels;

            Active = active;
            check_icon.Kind = active ?
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBox :
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline;

            lap_lbl.Content = string.Format("{0}. lap\t{1:D2}:{2:D2}:{3:D2}", lap.Index, lap.Time.Minutes, lap.Time.Seconds, lap.Time.Milliseconds);
        }

        private void checkLap_Click(object sender, RoutedEventArgs e)
        {
            check_icon.Kind = check_icon.Kind == MaterialDesignThemes.Wpf.PackIconKind.CheckBox ?
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline :
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBox;

            Active = !Active;

            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Datas").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).InitLapListElements();
        }

        private void settingsLap_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LapChannels lap_channels_window = new LapChannels(lap, channels, pilots_name);
            lap_channels_window.Show();
        }
    }
}
