using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.InputFiles.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Drivers.UserControls
{
    /// <summary>
    /// This User Control represents one <seealso cref="Pilots.Driver"/> on <seealso cref="DriversMenu"/>
    /// </summary>
    public partial class DriverCard : UserControl
    {
        private static readonly List<InputFileListElement> inputFileListElements = new List<InputFileListElement>();

        public Driver Driver { get; private set; }

        public DriverCard(Driver driver)
        {
            InitializeComponent();

            Driver = driver;
            DriverNameLbl.Content = driver.Name;

            InitInputFileListElements();
        }

        private void InitInputFileListElements()
        {
            if (Driver != null)
            {
                inputFileListElements.Clear();
                InputFilesStackPanel.Children.Clear();
                foreach (InputFile inputFile in InputFileManager.InputFiles)
                {
                    AddInputFileListElement(inputFile.FileName);
                }
            }
        }

        private void AddInputFileListElement(string fileName)
        {
            InputFileListElement inputFileListElement = new InputFileListElement(fileName,
                                                                                 ref ReadFileProgressBarGrid,
                                                                                 ref ReadFileProgressBar,
                                                                                 ref ReadFileProgressBarLbl
                                                                                 );
            InputFilesStackPanel.Children.Add(inputFileListElement);
            inputFileListElements.Add(inputFileListElement);
        }

        private void DeleteDriver_Click(object sender, RoutedEventArgs e)
        {
            DriverManager.RemoveDriver(Driver.Name);
            ((DriversMenu)MenuManager.GetTab(TextManager.DriversMenuName).Content).DeleteDriver(Driver.Name);
            ((DriversMenu)MenuManager.GetTab(TextManager.DriversMenuName).Content).InitDriverCards();
            //((DiagramsMenu)MenuManager.GetTab(TextManager.DiagramCustomTabName).Content).InitDriversTabs();
        }

        private void AddInputFile_Click(object sender, RoutedEventArgs e)
        {
            ReadFileProgressBarLbl.Content = "Reading file..";

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Add file",
                DefaultExt = ".csv",
                Multiselect = false,
                Filter = "csv files (*.csv)|*.csv|txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName.Split('\\').Last();
                if (InputFileManager.GetInputFile(fileName) == null)
                {
                    AddInputFileListElement(fileName);

                    ((DriversMenu)MenuManager.GetTab(TextManager.DriversMenuName).Content).DisableAllDrivers(disable: true, Driver.Name);
                    DataReader.Instance.ReadData(Driver, openFileDialog.FileName, ref ReadFileProgressBarGrid, ref ReadFileProgressBar);
                }
                else
                {
                    MessageBox.Show(string.Format("'{0}' has already been read!", fileName), "File already been read", MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }
        }
    }
}
