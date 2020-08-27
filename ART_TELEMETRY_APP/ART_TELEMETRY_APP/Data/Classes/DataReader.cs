using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings.Classes;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP
{
    class DataReader
    {
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

        private Driver driver;
        private string input_file_name;
        private Grid progressbar_grid;
        private ProgressBar progressbar;
        private long file_length;
        private BackgroundWorker worker;

        public void ReadData(Driver driver,
                             string input_file_name,
                             Grid progressbar_grid,
                             ref ProgressBar progressbar
                             )
        {
            this.driver = driver;
            this.input_file_name = input_file_name;

            if (isFileNew)
            {
                this.progressbar_grid = progressbar_grid;
                this.progressbar = progressbar;
                file_length = File.ReadLines(input_file_name).Count();

                this.progressbar_grid.Visibility = Visibility.Visible;
                this.progressbar.IsIndeterminate = false;
                this.progressbar.Value = 0;

                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += workerDoWork;
                worker.ProgressChanged += workerProgressChanged;
                worker.RunWorkerCompleted += workerCompleted;
                worker.RunWorkerAsync(10000);
            }
            else
            {
                // error_snack_bar.MessageQueue.Enqueue(string.Format("{0} already exists!", fileNameWithoutPath), null, null, null, false, true,
                // TimeSpan.FromSeconds(duration));
            }
        }

        private string fileNameWithoutPath => input_file_name.Split('\\').Last();

        private bool isFileNew => driver.GetInputFile(fileNameWithoutPath) == null;

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            NumberFormatInfo number_format_info = new CultureInfo("hu-HU", false).NumberFormat;

            List<Data> new_data = new List<Data>();

            StreamReader file_reader = new StreamReader(input_file_name, Encoding.Default);

            string[] attributes = file_reader.ReadLine().Split(';');
            foreach (string attribute in attributes)
            {
                Data single_data = new Data();
                single_data.Attribute = attribute;
                single_data.AllData = new ChartValues<double>();
                single_data.Option = new LineSerieOptions
                {
                    StrokeThickness = .7f,
                    StrokeColor = Brushes.Black
                };
                single_data.InputFileName = fileNameWithoutPath;
                single_data.DriverName = driver.Name;
                new_data.Add(single_data);
            }

            uint index = 0;

            while (!file_reader.EndOfStream)
            {
                string[] row = file_reader.ReadLine().Split(';');
                for (ushort i = 0; i < new_data.Count; i++)
                {
                    if (row[i].Equals(string.Empty))
                    {
                        new_data[i].AllData.Add(double.NaN);
                    }
                    else
                    {
                        new_data[i].AllData.Add(double.Parse(row[i], number_format_info));
                    }
                }

                worker.ReportProgress(Convert.ToInt32(index++ / (float)file_length * 100));
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                progressbar.Value = 0;
                progressbar.IsIndeterminate = true;
            });

            file_reader.Close();

            driver.AddInputFile(new InputFile(fileNameWithoutPath, new_data, driver.Name));
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressbar_grid.Visibility = Visibility.Hidden;
            progressbar.IsIndeterminate = false;
            //((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilot.Name).Content).GetTab(TextManager.DiagramCustomTabName).Content).InitInputFileCmbbox();
            foreach (TabItem item in ((DriverContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(driver.Name).Content).Tabs)
            {
                try
                {
                    ((LapsContent)item.Content).InitInputFileCmbbox();
                }
                catch (Exception) { }
            }
            ((DriversMenuContent)TabManager.GetTab(TextManager.DriversMenuName).Content).DisableAllDrivers(false, driver.Name);

            if (driver.InputFiles.Last().Latitude == null || driver.InputFiles.Last().Longitude == null)
            {
                ((DriversMenuContent)TabManager.GetTab(TextManager.DriversMenuName).Content).ShowError("No longitude or latitude data found!");
            }
        }
    }
}
