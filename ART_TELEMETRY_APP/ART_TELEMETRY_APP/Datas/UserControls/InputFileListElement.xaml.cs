using ART_TELEMETRY_APP.Maps;
using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Settings;
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

namespace ART_TELEMETRY_APP.Pilots
{
    /// <summary>
    /// Interaction logic for InputFileListElement_UI.xaml
    /// </summary>
    public partial class InputFileListElement : UserControl
    {
        string pilots_name;
        string file_name;

        public InputFileListElement(string file_name, string pilots_name)
        {
            InitializeComponent();

            this.pilots_name = pilots_name;
            this.file_name = file_name;
            file_name_lbl.Content = this.file_name;

            updateMapsCmbbox();
        }

        private void updateMapsCmbbox()
        {
            foreach (Map map in MapManager.Maps)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = map.Name;
                maps_cmbbox.Items.Add(item);
            }
        }

        private void deleteInputFile_Click(object sender, RoutedEventArgs e)
        {
            PilotManager.GetPilot(pilots_name).RemoveInputFile(file_name_lbl.Content.ToString());
            ((PilotsMenuContent)TabManager.GetTab("Pilots").Content).InitPilots();
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).InitInputFileCmbbox();
        }

        private void settingsInputFile_Click(object sender, RoutedEventArgs e)
        {
            TabManager.GetTab("Settings").IsSelected = true; //TODO: ha több settings van mint csak a Maps, akkor még azt is selected-re kell tenni. :)
        }

        private void mapsCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = maps_cmbbox.SelectedItem.ToString().Split(':').Last();
            name = name.Substring(1, name.Length - 1);
            PilotManager.GetPilot(pilots_name).GetInputFile(file_name).MapName = name;
        }
    }
}
