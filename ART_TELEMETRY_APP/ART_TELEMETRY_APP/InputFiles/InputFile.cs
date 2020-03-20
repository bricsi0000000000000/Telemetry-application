using ART_TELEMETRY_APP.InputFiles;
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
    public class InputFile
    {
        string file_name;
        List<Data> datas = new List<Data>();
        float filter_percent = .6f;
        int act_lap = 0;
        ChartValues<double> distances;
        public List<Tuple<List<Tuple<double, double>>, int, int>> laps = new List<Tuple<List<Tuple<double, double>>, int, int>>();

        public InputFile(string input_data_name, List<Data> datas)
        {
            this.file_name = input_data_name;
            this.datas = datas;
        }

        public void InitDistances()
        {
            distances = new ChartValues<double>();
            ChartValues<double> filtered_speed = filteredData(GetSingleData("speed").Datas);
            ChartValues<double> filtered_time = filteredData(GetSingleData("Time").Datas);
            distances.Add(0);
            for (int i = 1; i < filtered_speed.Count; i++)
            {
                if (i - 1 >= 0)
                {
                    distances.Add(distances[i - 1] + distance(filtered_time[i - 1], filtered_time[i], filtered_speed[i - 1] / 3.6, filtered_speed[i] / 3.6));
                }
            }
        }

        double distance(double time1, double time2, double speed1, double speed2)
        {
            return ((speed1 + speed2) / 2) * (time2 - time1);
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

        public Data GetSingleData(string name)
        {
            return datas.Find(n => n.Name == name);
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

        public ChartValues<ObservablePoint> GetChartValues(string attribute, int lap = 0)
        {
            return convertToObservablePoints(    filteredData(GetLapValues(datas.Find(attr => attr.Name == attribute).Datas, lap)));
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
