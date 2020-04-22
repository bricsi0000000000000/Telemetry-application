using ART_TELEMETRY_APP.Settings;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for PilotTab.xaml
    /// </summary>
    public partial class PilotTab_UC : UserControl
    {
        Pilot pilot;
        Snackbar error_snack_bar;

        public PilotTab_UC(Pilot pilot, Snackbar error_snack_bar = null)
        {
            InitializeComponent();

            this.pilot = pilot;
            this.error_snack_bar = error_snack_bar;

            InitInputFiles();
        }

        public void InitInputFiles()
        {
            foreach (InputFile file in pilot.InputFiles)
            {
               // files_stackpanel.Children.Add(new InputFileListElement(file.FileName, pilot, ref files_stackpanel));
            }
        }

        private void addFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";

            if (open_file_dialog.ShowDialog() == true)
            {
                string file_name = open_file_dialog.FileName.Split('\\').Last();
                file_name_lbl.Content = file_name;
               /* DataReader.Instance.ReadData(pilot,
                                             open_file_dialog.FileName,
                                             progressbar_grid,
                                             ref progressbar,
                                             files_stackpanel,
                                             error_snack_bar,
                                             add_file_btn
                                             );*/
            }
        }
    }
}
