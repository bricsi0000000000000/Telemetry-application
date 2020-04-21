using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for PilotSettings.xaml
    /// </summary>
    public partial class PilotSettings : UserControl
    {
        string pilots_name;
        List<InputFileListElement> input_files = new List<InputFileListElement>();
        Pilot pilot;

        public PilotSettings(string pilots_name)
        {
            InitializeComponent();

            this.pilots_name = pilots_name;
            pilot = PilotManager.GetPilot(pilots_name);
            pilots_name_lbl.Content = pilots_name;

            initInputFiles();
        }

        private void initInputFiles()
        {
            if (pilot != null)
            {
                input_files_stackpanel.Children.Clear();
                foreach (InputFile input_file in pilot.InputFiles)
                {
                    InputFileListElement input_file_list_element = new InputFileListElement(input_file.FileName, pilot.Name);
                    input_files_stackpanel.Children.Add(input_file_list_element);
                    input_files.Add(input_file_list_element);
                }
            }
        }

        private void deletePilot_Click(object sender, RoutedEventArgs e)
        {
            PilotManager.RemovePilot(pilots_name);
            ((PilotsMenuContent)TabManager.GetTab("Pilots").Content).InitPilots();
            ((DatasMenuContent)TabManager.GetTab("Datas").Content).InitPilotsTabs();
        }

        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";

            if (open_file_dialog.ShowDialog() == true)
            {
                string file_name = open_file_dialog.FileName.Split('\\').Last();
                InputFileListElement input_file_list_element = new InputFileListElement(file_name, pilot.Name);
                input_files_stackpanel.Children.Add(input_file_list_element);
                DataReader.Instance.ReadData(pilot, open_file_dialog.FileName, progressbar_grid, ref progressbar);
            }
        }
    }
}
