using ART_TELEMETRY_APP.Laps;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// Interaction logic for AllLapListElement.xaml
    /// </summary>
    public partial class AllLapListElement : UserControl
    {
        private readonly List<string> channels;
        private readonly string pilots_name;
        private readonly List<string> all_selected_channels;
        private bool all_laps_active;

        public AllLapListElement(List<string> channels, TimeSpan all_time, string pilots_name, List<string> all_selected_channels)
        {
            InitializeComponent();

            this.channels = channels;
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

            //((LapsContent)((DriverContentTab)((DiagramsMenu)MenuManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).ChangeAllLapsActive(all_laps_active);
           // ((LapsContent)((DriverContentTab)((DiagramsMenu)MenuManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).BuildCharts();
        }

        private void settingsLap_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AllLapChannels lap_channels_window = new AllLapChannels(pilots_name, channels, all_selected_channels);
            lap_channels_window.Show();
        }
    }
}
