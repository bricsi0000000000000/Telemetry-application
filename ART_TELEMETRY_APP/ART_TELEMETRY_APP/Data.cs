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
        float filter_percent = .7f;
        int act_lap = 0;
        ChartValues<double> time_datas;
        public List<Tuple<List<Tuple<double, double>>, int, int>> laps = new List<Tuple<List<Tuple<double, double>>, int, int>>();

        public class SingleData
        {
            public string Name;
            public ChartValues<double> Datas;
            public LineSerieOptions Option;
            public List<ChartValues<ObservablePoint>> DatasInLaps = new List<ChartValues<ObservablePoint>>();
        }

        public class LineSerieOptions
        {
            public bool line_smoothness;
            public float stroke_thickness;
            public Brush stroke_color;
        }

        public void InitTimeDatas()
        {
            time_datas = filteredData(datas[0].Datas);
        }

        public List<Tuple<List<Tuple<double, double>>, int, int>> Laps
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

        public void MakeDatasInLaps(string attribute)
        {
            for (int i = 0; i < laps.Count; i++)
            {
                GetSingleData(attribute).DatasInLaps.Add(GetChartValues(attribute, i));
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

        public ChartValues<ObservablePoint> GetChartValues(string attribute, int lap = 0)
        {
            return convertToObservablePoints(filteredData(GetLapValues(datas.Find(attr => attr.Name == attribute).Datas, lap)));
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

        ChartValues<double> timeDatas
        {
            get
            {
                return time_datas;
            }
        }

        ChartValues<ObservablePoint> convertToObservablePoints(ChartValues<double> datas)
        {
            ChartValues<ObservablePoint> return_datas = new ChartValues<ObservablePoint>();

            ChartValues<double> time = timeDatas;

            for (int i = 0; i < datas.Count; i++)
            {
                return_datas.Add(new ObservablePoint
                {
                    X = time[i],
                    Y = datas[i]
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
