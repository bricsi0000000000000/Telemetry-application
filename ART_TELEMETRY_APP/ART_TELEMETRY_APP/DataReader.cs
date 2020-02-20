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

        long file_length;
        ProgressBar importFileProgressBar;
        BackgroundWorker worker;
        string file_name;
        Grid importFileDarkening;
        Card input_file_nothing;
        Path map_svg;
        ColorZone map_nothing;

        public void ReadData(string file_name, ProgressBar importFileProgressBar, Grid importFileDarkening, Card input_file_nothing, Path map_svg, ColorZone map_nothing)
        {
            this.input_file_nothing = input_file_nothing;
            this.file_name = file_name;
            this.file_length = File.ReadLines(file_name).Count();
            this.importFileProgressBar = importFileProgressBar;
            this.importFileDarkening = importFileDarkening;
            this.map_svg = map_svg;
            this.map_nothing = map_nothing;

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
                single_data.Datas = new ChartValues<double>();
                single_data.Option = new Data.LineSerieOptions
                {
                    line_smoothness = false,
                    stroke_thickness = .7f,
                    stroke_color = Brushes.Black
                };
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
                        new_datas[i].Datas.Add(double.Parse(row[i], number_format_info));
                    }
                }

                worker.ReportProgress(Convert.ToInt32((index / (double)file_length) * 100));
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
            if (Datas.Instance.DatasCount > 0)
            {
                input_file_nothing.Visibility = Visibility.Hidden;
                Datas.Instance.ActiveFileName = file_name.Split('\\').Last();
                MapBuilder.Instance.Make(file_name.Split('\\').Last());
            }

            Console.WriteLine(Datas.Instance.ActiveFileName);

        }
    }
}
