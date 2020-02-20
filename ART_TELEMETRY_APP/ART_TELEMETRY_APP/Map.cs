using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    class Map
    {
        List<string> svg_pathes = new List<string>();
        string name;
        BackgroundWorker worker;
        ProgressBar map_progressbar;
        ColorZone map_progressbar_colorzone;
        Path map_svg;
        ColorZone map_nothing;

        public Map(string name,
                   ProgressBar map_progressbar,
                   ColorZone map_progressbar_colorzone,
                   Path map_svg,
                   ColorZone map_nothing
                  )
        {
            this.name = name;
            this.map_progressbar = map_progressbar;
            this.map_progressbar_colorzone = map_progressbar_colorzone;
            this.map_svg = map_svg;
            this.map_nothing = map_nothing;
            map_progressbar.Visibility = System.Windows.Visibility.Visible;
            map_progressbar_colorzone.Visibility = System.Windows.Visibility.Visible;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += calculateLaps;
            worker.ProgressChanged += workerProgressChanged;
            worker.RunWorkerCompleted += workerCompleted;
            worker.RunWorkerAsync(1000);
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public List<string> SvgPathes
        {
            get
            {
                return svg_pathes;
            }
        }

        private void calculateLaps(object sender, DoWorkEventArgs e)
        {
            Datas.Instance.GetData().Laps.Clear();

            double latitude_min = double.MaxValue;
            double longitude_min = double.MaxValue;
            foreach (var item in Datas.Instance.GetData().Latitude)
            {
                if (item != 0 && !Double.IsNaN(item))
                {
                    if (item < latitude_min)
                    {
                        latitude_min = item;
                    }
                }
            }
            foreach (var item in Datas.Instance.GetData().Longitude)
            {
                if (item != 0 && !Double.IsNaN(item))
                {
                    if (item < longitude_min)
                    {
                        longitude_min = item;
                    }
                }
            }

            Console.WriteLine(longitude_min + " " + latitude_min);

            double scale = Math.Pow(10, 5);

            List<double> latitude = new List<double>();
            List<double> longitude = new List<double>();

            List<double> raw_latitude = Datas.Instance.GetData().Latitude;
            List<double> raw_longitude = Datas.Instance.GetData().Longitude;

            for (int i = 0; i < raw_latitude.Count; i++)
            {
                if (raw_latitude[i] != 0 && !Double.IsNaN(raw_latitude[i]))
                {
                    latitude.Add(Math.Round((raw_latitude[i] - latitude_min) * scale));
                }
                if (raw_longitude[i] != 0 && !Double.IsNaN(raw_latitude[i]))
                {
                    longitude.Add(Math.Round((raw_longitude[i] - longitude_min) * scale));
                }
            }

            int after = 500;
            int radius = 40;

            List<Tuple<double, double>> act_lap = new List<Tuple<double, double>>();

            bool last_in_circle = true;
            int last_circle_index = 0;

            int lap_start_index = 0;
            int lap_end_index = 0;

            for (int i = 0; i < latitude.Count; i++)
            {
                act_lap.Add(new Tuple<double, double>(latitude[i], longitude[i]));
                if (i >= last_circle_index + after)
                {
                    last_in_circle = false;
                }

                if (!last_in_circle)
                {
                    if (Math.Sqrt(Math.Pow(latitude[i] - latitude[0], 2) + Math.Pow(longitude[i] - longitude[0], 2)) < radius)
                    {
                        last_in_circle = true;
                        last_circle_index = i;

                        if (i + 1 < latitude.Count)
                        {
                            act_lap.Add(new Tuple<double, double>(latitude[i + 1], longitude[i + 1]));
                        }

                        List<Tuple<double, double>> add = new List<Tuple<double, double>>(act_lap);

                        lap_end_index = i;
                        Datas.Instance.GetData().Laps.Add(new Tuple<List<Tuple<double, double>>, int, int>(add, lap_start_index, lap_end_index));
                        lap_start_index = i;
                        act_lap.Clear();
                    }
                }

                //worker.ReportProgress(Convert.ToInt32((i / (double)latitude.Count) * 100));
            }
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // map_progressbar.Value = e.ProgressPercentage;
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            for (int lap = 0; lap < Datas.Instance.GetData().Laps.Count; lap++)
            {
                string act_svg_path = string.Format("M{0} {1}",
                                      Datas.Instance.GetData().Laps[lap].Item1[0].Item1,
                                      Datas.Instance.GetData().Laps[lap].Item1[0].Item2);

                for (int i = 0; i < Datas.Instance.GetData().Laps[lap].Item1.Count; i++)
                {
                    act_svg_path += string.Format(" L{0} {1}",
                                    Datas.Instance.GetData().Laps[lap].Item1[i].Item1,
                                    Datas.Instance.GetData().Laps[lap].Item1[i].Item2);
                }

                svg_pathes.Add(act_svg_path);
            }

            map_progressbar_colorzone.Visibility = System.Windows.Visibility.Hidden;
            MapBuilder.Instance.Build(map_svg, map_nothing);
        }
    }
}
