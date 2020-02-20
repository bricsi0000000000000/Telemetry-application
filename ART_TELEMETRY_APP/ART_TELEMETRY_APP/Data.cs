using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ART_TELEMETRY_APP
{
    class Data
    {
        string file_name;
        List<SingleData> datas = new List<SingleData>();
        float filter_percent = .03f;
        List<Tuple<List<Tuple<double, double>>, int, int>> laps = new List<Tuple<List<Tuple<double, double>>, int, int>>();
        int act_lap = 0;

        public class SingleData
        {
            public string Name;
            public ChartValues<double> Datas;
            public LineSerieOptions Option;
        }

        public class LineSerieOptions
        {
            public bool line_smoothness;
            public float stroke_thickness;
            public Brush stroke_color;
        }

        public List<Tuple<List<Tuple<double, double>>, int, int>> Laps
        {
            get
            {
                return laps;
            }
            set
            {
                laps = value;
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
                if (value > 0 && value <= laps.Count)
                {
                    act_lap = value;
                }
            }
        }


        public List<double> Latitude
        {
            get
            {
                return datas.Find(n => n.Name == "Latitude").Datas.ToList();
            }
        }

        public List<double> Longitude
        {
            get
            {
                return datas.Find(n => n.Name == "Longitude").Datas.ToList();
            }
        }

        public SingleData GetSingleData(string name)
        {
            return datas.Find(n => n.Name == name);
        }

        public Data(string input_data_name, List<SingleData> datas)
        {
            this.file_name = input_data_name;
            this.datas = datas;
        }

        public string FileName
        {
            get
            {
                return this.file_name;
            }
        }

        public List<SingleData> Datas
        {
            get
            {
                return this.datas;
            }
        }

        public ChartValues<ObservablePoint> GetChartValues(string attribute)
        {
            ChartValues<double> values = datas.Find(attr => attr.Name == attribute).Datas;
            return convertToObservablePoints(filteredData(values));
        }

        List<double> GetLapValues(int lap = 0)
        {
            int get_lap = lap == 0 ? act_lap : lap;

            foreach (var item in laps)
            {
                Console.WriteLine(item.Item1.Count + " " + item.Item2 + " " + item.Item3);
            }

            return null;
        }

        ChartValues<double> timeDatas
        {
            get
            {
                return datas[0].Datas;
            }
        }

        ChartValues<ObservablePoint> convertToObservablePoints(ChartValues<double> filtered_datas)
        {
            ChartValues<ObservablePoint> return_datas = new ChartValues<ObservablePoint>();

            ChartValues<double> time = filteredData(timeDatas);

            for (int i = 0; i < filtered_datas.Count; i++)
            {
                return_datas.Add(new ObservablePoint
                {
                    X = time[i],
                    Y = filtered_datas[i]
                });
            }

            return return_datas;
        }

        ChartValues<double> filteredData(ChartValues<double> datas)
        {
            ChartValues<double> input_datas = new ChartValues<double>(datas);
            int total = input_datas.Count;
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
            }

            return input_datas;
        }
    }
}
