using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings;
using LiveCharts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

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

        Pilot pilot;
        string file_name;
        Grid progressbar_grid;
        ProgressBar progressbar;
        long file_length;
        BackgroundWorker worker;

        public void ReadData(Pilot pilot,
                             string file_name,
                             Grid progressbar_grid,
                             ref ProgressBar progressbar
                             )
        {
            this.pilot = pilot;
            this.file_name = file_name;

            if (isFileNew)
            {
                this.progressbar_grid = progressbar_grid;
                this.progressbar = progressbar;
                this.file_length = File.ReadLines(file_name).Count();

                this.progressbar_grid.Visibility = Visibility.Visible;

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

        string fileNameWithoutPath
        {
            get
            {
                return file_name.Split('\\').Last();
            }
        }

        bool isFileNew
        {
            get
            {
                return pilot.GetInputFile(fileNameWithoutPath) == null;
            }
        }

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            NumberFormatInfo number_format_info = new CultureInfo("hu-HU", false).NumberFormat;

            List<Data> new_datas = new List<Data>();

            StreamReader read_file = new StreamReader(file_name, Encoding.Default);

            string[] attributes = read_file.ReadLine().Split(';');
            foreach (string attribute in attributes)
            {
                Data single_data = new Data();
                single_data.Attribute = attribute;
                single_data.Datas = new ChartValues<double>();
                single_data.Option = new LineSerieOptions
                {
                    stroke_thickness = .7f,
                    stroke_color = Brushes.Black
                };
                single_data.InputFileName = fileNameWithoutPath;
                single_data.PilotsName = pilot.Name;
                new_datas.Add(single_data);
            }

            uint index = 0;

            while (!read_file.EndOfStream)
            {
                string[] row = read_file.ReadLine().Split(';');
                for (int i = 0; i < new_datas.Count; i++)
                {
                    if (row[i] == "")
                    {
                        new_datas[i].Datas.Add(double.NaN);
                    }
                    else
                    {
                        new_datas[i].Datas.Add(double.Parse(row[i], number_format_info));
                    }
                }

                worker.ReportProgress(Convert.ToInt32((index++ / (float)file_length) * 100));
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                progressbar.Value = 0;
                progressbar.IsIndeterminate = true;
            });

            read_file.Close();

            pilot.AddInputFile(new InputFile(fileNameWithoutPath, new_datas, pilot.Name));
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressbar_grid.Visibility = Visibility.Hidden;
            progressbar.IsIndeterminate = false;
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilot.Name).Content).GetTab("Laps").Content).InitInputFileCmbbox();
            ((PilotsMenuContent)TabManager.GetTab("Pilots").Content).DisableAllPilots(false, pilot.Name);

            if (pilot.InputFiles.Last().Latitude == null || pilot.InputFiles.Last().Longitude == null)
            {
                ((PilotsMenuContent)TabManager.GetTab("Pilots").Content).ShowError("No longitude or latitude data found!");
            }
        }
    }
}
