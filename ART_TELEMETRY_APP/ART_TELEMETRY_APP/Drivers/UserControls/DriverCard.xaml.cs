using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.InputFiles.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using MaterialDesignThemes.Wpf;
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
        private readonly Snackbar errorSnackbar;
        private readonly Grid readFileProgressBarGrid;
        private readonly ProgressBar readFileProgressBar;
        private readonly Label readFileProgressBarLbl;

        public DriverCard(Driver driver,
                          ref Snackbar errorSnackbar,
                          ref Grid readFileProgressBarGrid,
                          ref ProgressBar readFileProgressBar,
                          ref Label readFileProgressBarLbl)
        {
            InitializeComponent();

            Driver = driver;
            DriverNameLbl.Content = driver.Name;

            this.errorSnackbar = errorSnackbar;
            this.readFileProgressBarGrid = readFileProgressBarGrid;
            this.readFileProgressBar = readFileProgressBar;
            this.readFileProgressBarLbl = readFileProgressBarLbl;

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
                    if (inputFile.DriverName.Equals(Driver.Name))
                    {
                        AddInputFileListElement(inputFile.FileName);
                    }
                }
            }
        }

        private void AddInputFileListElement(string fileName)
        {
            InputFileListElement inputFileListElement = new InputFileListElement(fileName,
                                                                                 Driver.Name,
                                                                                 readFileProgressBarGrid,
                                                                                 readFileProgressBar,
                                                                                 readFileProgressBarLbl
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
                readFileProgressBarLbl.Content = $"Reading \"{fileName}\" for {Driver.Name}";
                if (InputFileManager.GetInputFile(fileName, Driver.Name) == null)
                {
                    AddInputFileListElement(fileName);

                    InputFileListElement listElement = inputFileListElements.Last();
                    DataReader.Instance.ReadData(Driver,
                                                 openFileDialog.FileName,
                                                 readFileProgressBarGrid,
                                                 readFileProgressBar,
                                                 errorSnackbar,
                                                 ref listElement);
                }
                else
                {
                    ShowError.ShowErrorMessage(errorSnackbar, string.Format("'{0}' has already been read!", fileName), 3);
                }
            }
        }
    }
}
