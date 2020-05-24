using ART_TELEMETRY_APP.Maps;
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

        public InputFileListElement(string file_name, string pilots_name)
        {
            InitializeComponent();

            file_name_lbl.Content = file_name;
            this.pilots_name = pilots_name;
        }

        private void deleteInputFile_Click(object sender, RoutedEventArgs e)
        {
            PilotManager.GetPilot(pilots_name).RemoveInputFile(file_name_lbl.Content.ToString());
            ((PilotsMenuContent)TabManager.GetTab("Pilots").Content).InitPilots();
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).InitInputFileCmbbox();
        }

        private void settingsInputFile_Click(object sender, RoutedEventArgs e)
        {
            MapEditor map_editor = new MapEditor(PilotManager.GetPilot(pilots_name).GetInputFile(file_name_lbl.Content.ToString()));
            map_editor.Show();
        }
    }
}
