using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ART_TELEMETRY_APP
{
    public class InputFile
    {
        string file_name;
        List<Data> datas = new List<Data>();
        //float filter_percent = .6f;
        int act_lap = 0;
        ChartValues<double> distances;
        string pilot_name;
        List<Lap> laps = new List<Lap>();
        string all_lap_svg;
        List<Point> average_lap = new List<Point>();
        List<Point> map_points = new List<Point>();
        List<string> lapsSVGs = new List<string>();
        List<bool> active_laps = new List<bool>();

        public InputFile(string input_data_name, List<Data> datas, string pilot_name)
        {
            this.file_name = input_data_name;
            this.datas = datas;
            this.pilot_name = pilot_name;

            makeAllLapsSVG();
            InitDistances();
        }

        public void MakeLapTimes()
        {
            List<double> times = datas.Find(n => n.Attribute == "Time").Datas.ToList();
            foreach (Lap lap in laps)
            {
                int from = (times.Count * lap.FromIndex) / laps.Sum(a => a.Points.Count);
                int to = (times.Count * lap.ToIndex) / laps.Sum(a => a.Points.Count);

                lap.Time = TimeSpan.FromSeconds(times[to] - times[from]);
                //Console.WriteLine(string.Format("{0:D2}m:{1:D2}s:{2:D3}ms", time.Minutes, time.Seconds, time.Milliseconds));
            }
        }

        public void InitActiveLaps()
        {
            active_laps.Clear();
            for (int i = 0; i < laps.Count; i++)
            {
                active_laps.Add(false);
            }
        }

        private void makeAllLapsSVG()
        {
            List<double> latitude = Latitude;
            List<double> longitude = Longitude;

            if (latitude.Count > 0 && longitude.Count > 0)
            {
                double x_min = double.MaxValue;
                double y_min = double.MaxValue;
                foreach (double item in latitude)
                {
                    if (item != 0 && !double.IsNaN(item))
                    {
                        if (item < x_min)
                        {
                            x_min = item;
                        }
                    }
                }
                foreach (double item in longitude)
                {
                    if (item != 0 && !double.IsNaN(item))
                    {
                        if (item < y_min)
                        {
                            y_min = item;
                        }
                    }
                }

                double scale = Math.Pow(10, 5);
                List<double> scaled_latitude = new List<double>();
                List<double> scaled_longitude = new List<double>();
                for (int i = 0; i < latitude.Count; i++)
                {
                    if (latitude[i] != 0)
                    {
                        if (!double.IsNaN(latitude[i]))
                        {
                            scaled_latitude.Add(Math.Round((latitude[i] - x_min) * scale));
                        }
                        if (longitude[i] != 0)
                        {
                            if (!double.IsNaN(longitude[i]))
                            {
                                scaled_longitude.Add(Math.Round((longitude[i] - y_min) * scale));
                            }
                        }
                    }
                }

                for (int i = 0; i < scaled_latitude.Count; i++)
                {
                    map_points.Add(new Point(scaled_latitude[i], scaled_longitude[i]));
                }

                all_lap_svg = string.Format("M{0} {1}", map_points[0].X, map_points[0].Y);
                for (int i = 0; i < map_points.Count; i++)
                {
                    all_lap_svg += string.Format(" L{0} {1}", map_points[i].X, map_points[i].Y);
                }
            }
        }

        public void MakeAvgLap()
        {
            int shortest_lap_length = 0;
            int index = 0;
            for (int i = 1; i < laps.Count - 1; i++)
            {
                if (laps[i].Points.Count > shortest_lap_length)
                {
                    shortest_lap_length = laps[i].Points.Count;
                    index = i;
                }
            }

            average_lap = laps[index].Points;
            for (int i = 0; i < shortest_lap_length; i++)
            {
                try
                {
                    double avg_x = 0;
                    double avg_y = 0;
                    for (int j = 1; j < laps.Count - 1; j++)
                    {
                        avg_x += laps[j].Points[i].X;
                        avg_y += laps[j].Points[i].Y;
                    }
                    avg_x /= laps.Count - 2;
                    avg_y /= laps.Count - 2;
                    average_lap[i] = new Point(avg_x, avg_y);
                }
                catch (Exception)
                {
                }
            }
        }

        public void InitDistances()
        {
            distances = new ChartValues<double>();
            ChartValues<double> speed = GetData("speed").Datas;
            ChartValues<double> time = GetData("Time").Datas;
            distances.Add(0);
            for (int i = 1; i < speed.Count; i++)
            {
                if (i - 1 >= 0)
                {
                    distances.Add(distances[i - 1] + distance(time[i - 1], time[i], speed[i - 1] / 3.6, speed[i] / 3.6));
                }
            }
        }

        double distance(double time1, double time2, double speed1, double speed2)
        {
            //return ((speed1 + speed2) / 2) * (time2 - time1);
            return speed2 * (time2 - time1);
        }

        public ChartValues<double> Distances
        {
            get
            {
                return distances;
            }
        }

        public string PilotName
        {
            get
            {
                return pilot_name;
            }
        }

        public List<Lap> Laps
        {
            get
            {
                return laps;
            }
        }

        public int ActLap
        {
            get
            {
                return act_lap;
            }
            set
            {
                act_lap = value;
            }
        }

        public List<double> Latitude
        {
            get
            {
                return datas.Find(n => n.Attribute == "Latitude").Datas.ToList();
            }
        }

        public List<double> Longitude
        {
            get
            {
                return datas.Find(n => n.Attribute == "Longitude").Datas.ToList();
            }
        }

        public Data GetData(string name)
        {
            return datas.Find(n => n.Attribute == name);
        }

        public string FileName
        {
            get
            {
                return this.file_name;
            }
        }

        public List<Data> Datas
        {
            get
            {
                return this.datas;
            }
        }

        public string AllLapsSVG
        {
            get
            {
                return all_lap_svg;
            }
        }

        public List<Point> AvgLap
        {
            get
            {
                return average_lap;
            }
        }

        public List<Point> MapPoints
        {
            get
            {
                return map_points;
            }
            set
            {
                map_points = value;
            }
        }

        public List<string> LapsSVGs
        {
            get
            {
                return lapsSVGs;
            }
            set
            {
                lapsSVGs = value;
            }
        }

        public string AvgLapSVG
        {
            get
            {
                string svg_path = string.Format("M{0} {1}", average_lap[0].X, average_lap[0].Y);
                for (int i = 0; i < average_lap.Count; i++)
                {
                    svg_path += string.Format(" L{0} {1}", average_lap[i].X, average_lap[i].Y);
                }

                return svg_path;
            }
        }

        public List<bool> ActiveLaps
        {
            get
            {
                return active_laps;
            }
            set
            {
                active_laps = value;
            }
        }

        /*  public ChartValues<ObservablePoint> GetChartValues(string attribute, int lap = 0)
          {
              return convertToObservablePoints(filteredData(GetLapValues(datas.Find(attr => attr.Attribute == attribute).Datas, lap)));
          }

          ChartValues<double> GetLapValues(ChartValues<double> values, int lap = 0)
          {
              int get_lap = lap == 0 ? act_lap : lap;

              ChartValues<double> datas = new ChartValues<double>();

              for (int i = laps[get_lap].Item2; i < laps[get_lap].Item3; i++)
              {
                  datas.Add(values[i]);
              }

              return datas;
          }

          ChartValues<ObservablePoint> convertToObservablePoints(ChartValues<double> datas)
          {
              ChartValues<ObservablePoint> return_datas = new ChartValues<ObservablePoint>();

              for (int i = 0; i < datas.Count; i++)
              {
                  return_datas.Add(new ObservablePoint
                  {
                      X = distances[i],
                      Y = datas[i]
                  });
              }

              return return_datas;
          }*/

        ChartValues<double> filteredData(ChartValues<double> datas)
        {
            ChartValues<double> input_datas = new ChartValues<double>(datas);
            /*int total = input_datas.Count;
            Random rand = new Random(DateTime.Now.Millisecond);
            while (input_datas.Count / (double)total > filter_percent)
            {
                try
                {
                    input_datas.RemoveAt(rand.Next(1, input_datas.Count - 1));
                }
                catch (Exception)
                {
                }
            }*/

            return input_datas;
        }
    }
}
