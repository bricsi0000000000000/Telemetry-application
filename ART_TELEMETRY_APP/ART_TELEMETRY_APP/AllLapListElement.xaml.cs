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
    /// Interaction logic for AllLapListElement.xaml
    /// </summary>
    public partial class AllLapListElement : UserControl
    {
        List<string> channels;
        TimeSpan all_time;
        string pilots_name;
        bool all_laps_active;
        List<string> all_selected_channels;

        public AllLapListElement(List<string> channels, TimeSpan all_time, string pilots_name, List<string> all_selected_channels)
        {
            InitializeComponent();

            this.channels = channels;
            this.all_time = all_time;
            this.pilots_name = pilots_name;
            this.all_selected_channels = all_selected_channels;

            /*check_icon.Kind = all_laps_active ?
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBox :
                              check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline;
*/
            lap_lbl.Content = string.Format("All lap\t{0:D2}:{1:D2}:{2:D2}", all_time.Minutes, all_time.Seconds, all_time.Milliseconds);
        }

        private void checkLap_Click(object sender, RoutedEventArgs e)
        {
            check_icon.Kind = check_icon.Kind == MaterialDesignThemes.Wpf.PackIconKind.CheckBox ?
                               check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline :
                               check_icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBox;

            all_laps_active = !all_laps_active;

            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).ChangeAllLapsActive(all_laps_active);
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).BuildCharts();
        }

        private void settingsLap_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AllLapChannels lap_channels_window = new AllLapChannels(pilots_name, channels, all_selected_channels);
            lap_channels_window.Show();
        }
    }
}
