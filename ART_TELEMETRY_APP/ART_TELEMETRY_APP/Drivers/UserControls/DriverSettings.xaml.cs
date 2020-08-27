using ART_TELEMETRY_APP.Settings.Classes;
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
    /// This User Control represents one <seealso cref="Driver"/> on <seealso cref="DriversMenuContent"/>
    /// </summary>
    public partial class DriverSettings : UserControl
    {
        private readonly List<InputFileListElement> input_files = new List<InputFileListElement>();
        private readonly Driver driver;

        public string DriverName { get; private set; }

        public DriverSettings(string driver_name)
        {
            InitializeComponent();

            DriverName = driver_name;
            driver = DriverManager.GetDriver(driver_name);
            pilots_name_lbl.Content = driver_name;

            initInputFiles();
        }

        private void initInputFiles()
        {
            if (driver != null)
            {
                input_files_stackpanel.Children.Clear();
                foreach (InputFile input_file in driver.InputFiles)
                {
                    InputFileListElement input_file_list_element = new InputFileListElement(input_file.FileName,
                                                                                            driver.Name,
                                                                                            ref progressbar_grid,
                                                                                            ref progressbar,
                                                                                            ref progressbar_lbl
                                                                                            );
                    input_files_stackpanel.Children.Add(input_file_list_element);
                    input_files.Add(input_file_list_element);
                }
            }
        }

        private void deletePilot_Click(object sender, RoutedEventArgs e)
        {
            DriverManager.RemoveDriver(DriverName);
            ((DriversMenuContent)TabManager.GetTab(TextManager.DriversMenuName).Content).InitDrivers();
            ((DatasMenuContent)TabManager.GetTab(TextManager.DiagramCustomTabName).Content).InitDriversTabs();
        }

        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            progressbar_lbl.Content = "Reading file..";

            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Title = "Add file";
            open_file_dialog.DefaultExt = ".csv";
            open_file_dialog.Multiselect = false;
            open_file_dialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";

            if (open_file_dialog.ShowDialog() == true)
            {
                string file_name = open_file_dialog.FileName.Split('\\').Last();
                InputFileListElement input_file_list_element = new InputFileListElement(file_name,
                                                                                        driver.Name,
                                                                                        ref progressbar_grid,
                                                                                        ref progressbar,
                                                                                        ref progressbar_lbl
                                                                                        );
                input_files_stackpanel.Children.Add(input_file_list_element);
                ((DriversMenuContent)TabManager.GetTab(TextManager.DriversMenuName).Content).DisableAllDrivers(true, DriverName);
                DataReader.Instance.ReadData(driver, open_file_dialog.FileName, progressbar_grid, ref progressbar);
            }
        }
    }
}
