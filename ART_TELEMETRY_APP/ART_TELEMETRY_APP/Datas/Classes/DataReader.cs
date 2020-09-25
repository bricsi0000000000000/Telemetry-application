using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.InputFiles.UserControls;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Settings.UserControls;
using LiveCharts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static ART_TELEMETRY_APP.InputFiles.Classes.InputFile;

namespace ART_TELEMETRY_APP.Datas.Classes
{
    class DataReader
    {
        #region instance
        private static DataReader instance = null;
        private DataReader() { }
        public static DataReader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataReader();
                }
                return instance;
            }
        }
        #endregion

        private Driver driver;
        private string fileName;
        private Grid progressBarGrid;
        private ProgressBar progressBar;
        private Snackbar errorSnackbar;
        private long fileLength;
        private BackgroundWorker worker;
        private List<Channel> addChannels;
        private InputFileListElement inputFileListElement;

        public void ReadData(Driver driver,
                             string fileName,
                             Grid progressBarGrid,
                             ProgressBar progressBar,
                             Snackbar errorSnackbar,
                             ref InputFileListElement inputFileListElement
                             )
        {
            this.driver = driver;
            this.fileName = fileName;
            this.progressBarGrid = progressBarGrid;
            this.progressBar = progressBar;
            this.errorSnackbar = errorSnackbar;
            this.inputFileListElement = inputFileListElement;

            progressBarGrid.Visibility = Visibility.Visible;
            this.progressBar.IsIndeterminate = false;
            this.progressBar.Value = 0;

            fileLength = File.ReadLines(fileName).Count();

            StartWorker();
        }

        private void StartWorker()
        {
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += WorkerDoWork;
            worker.ProgressChanged += WorkerProgressChanged;
            worker.RunWorkerCompleted += WorkerCompleted;
            worker.RunWorkerAsync();
        }

        private string FileNameWithoutPath => fileName.Split('\\').Last();

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            NumberFormatInfo numberFormatInfo = new CultureInfo("hu-HU", false).NumberFormat;

            addChannels = new List<Channel>();

            using var reader = new StreamReader(fileName, Encoding.Default);

            string[] channelNames = reader.ReadLine().Split(';');
            foreach (var channelName in channelNames)
            {
                //channel.AllData = new ChartValues<double>();
                /* channel.Option = new LineSerieOptions
                 {
                     StrokeThickness = .7f,
                     StrokeColor = Brushes.Black
                 };*/
                //channel.InputFileName = fileNameWithoutPath;
                //channel.DriverName = driver.Name;
                addChannels.Add(new Channel(channelName));
            }

            uint progressIndex = 0;

            while (!reader.EndOfStream)
            {
                string[] row = reader.ReadLine().Split(';');
                for (ushort i = 0; i < addChannels.Count; i++)
                {
                    if (row[i].Equals(string.Empty))
                    {
                        addChannels[i].AddChannelData(float.NaN);
                    }
                    else
                    {
                        addChannels[i].AddChannelData(float.Parse(row[i], numberFormatInfo));
                    }
                }

                worker.ReportProgress(Convert.ToInt32(progressIndex++ / (float)fileLength * 100));
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                progressBar.Value = 0;
                progressBar.IsIndeterminate = true;
            });

        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBarGrid.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = true;


            InputFileManager.AddInputFile(new InputFile(FileNameWithoutPath, driver.Name, addChannels, ref errorSnackbar, (bool found) =>
            {
                inputFileListElement.ChangeBackground(found);
            }));

            ((SelectDriversAndInputFiles)MenuManager.GetTab(TextManager.DiagramsSettingsMenuName).Content).InitDriversItems();
            ((SelectDriversAndInputFiles)MenuManager.GetTab(TextManager.DiagramsSettingsMenuName).Content).InitLapItems();

            //((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilot.Name).Content).GetTab(TextManager.DiagramCustomTabName).Content).InitInputFileCmbbox();
            /*foreach (TabItem item in ((DriverContentTab)((DiagramsMenu)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(driver.Name).Content).Tabs)
            {
                try
                {
                    ((LapsContent)item.Content).InitInputFileCmbbox();
                }
                catch (Exception) { }
            }*/

            /*  if (driver.InputFiles.Last().Latitude == null || driver.InputFiles.Last().Longitude == null)
              {
                  ((DriversMenu)MenuManager.GetTab(TextManager.DriversMenuName).Content).ShowError("No longitude or latitude data found!");
              }*/
        }
    }
}
