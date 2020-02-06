using LiveCharts;
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

        long file_length;
        ProgressBar importFileProgressBar;
        BackgroundWorker worker;
        string file_name;
        Grid importFileDarkening;

        public void ReadData(string file_name, ProgressBar importFileProgressBar, Grid importFileDarkening)
        {
            this.file_name = file_name;
            this.file_length = File.ReadLines(file_name).Count();
            this.importFileProgressBar = importFileProgressBar;
            this.importFileDarkening = importFileDarkening;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += workerDoWork;
            worker.ProgressChanged += workerProgressChanged;
            worker.RunWorkerCompleted += workerCompleted;
            worker.RunWorkerAsync(10000);
        }

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            NumberFormatInfo number_format_info = new CultureInfo("hu-HU", false).NumberFormat;

            List<Data.SingleData> new_datas = new List<Data.SingleData>();

            StreamReader read_file = new StreamReader(file_name, Encoding.Default);

            string[] attributes = read_file.ReadLine().Split(';');
            foreach (string attribute in attributes)
            {
                Data.SingleData single_data = new Data.SingleData();
                single_data.Name = attribute;
                single_data.Datas = new ChartValues<float>();
                single_data.Option = new Data.LineSerieOptions { line_smoothness = false, stroke_color = Brushes.Black, stroke_thickness = .7f };
                new_datas.Add(single_data);
            }

            int index = 0;

            while (!read_file.EndOfStream)
            {
                string[] row = read_file.ReadLine().Split(';');
                for (int i = 0; i < new_datas.Count; i++)
                {
                    if (row[i] != "")
                    {
                        new_datas[i].Datas.Add(float.Parse(row[i], number_format_info));
                    }
                    else
                    {
                        new_datas[i].Datas.Add(float.NaN);
                    }
                }

                worker.ReportProgress(Convert.ToInt32((index / (float)file_length) * 100));
                index++;
            }

            read_file.Close();

            Datas.Instance.AddInputData(new Data(file_name.Split('\\').Last(), new_datas));
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            importFileProgressBar.Value = e.ProgressPercentage;
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            importFileDarkening.Visibility = Visibility.Hidden;
        }
    }
}
