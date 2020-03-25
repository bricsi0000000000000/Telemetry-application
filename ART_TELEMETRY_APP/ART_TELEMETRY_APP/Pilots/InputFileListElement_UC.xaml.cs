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
    public partial class InputFileListElement_UC : UserControl
    {
        Pilot pilot;
        StackPanel files_stackpanel;

        public InputFileListElement_UC(string file_name, Pilot pilot, ref StackPanel files_stackpanel)
        {
            InitializeComponent();

            file_name_lbl.Content = file_name;
            this.pilot = pilot;
            this.files_stackpanel = files_stackpanel;
        }

        private void deleteInputFile_Click(object sender, RoutedEventArgs e)
        {
            PilotManager.GetPilot(pilot.Name).RemoveInputFile(fileNameWithoutPath(file_name_lbl.Content.ToString()));
            files_stackpanel.Children.Remove(this);
            SettingsManager.UpdatePilotsInGroups();
            SettingsManager.UpdateSelectedChannelsInGroups();
        }

        private string fileNameWithoutPath(string file)
        {
            return file.Split('\\').Last();
        }
    }
}
