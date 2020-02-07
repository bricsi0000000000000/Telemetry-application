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

        public class SingleData
        {
            public string Name;
            public ChartValues<float> Datas;
            public LineSerieOptions Option;
        }

        public class LineSerieOptions
        {
            public bool line_smoothness;
            public float stroke_thickness;
            public Brush stroke_color;
        }

        public SingleData GetSingleData(string name)
        {
            return datas.Find(n => n.Name == name);
        }

        public Data(string input_data_name, List<SingleData> datas)
        {
            this.file_name = input_data_name;
            this.datas = datas;

            /*Options.line_smoothness = true;
            Options.stroke_thickness = .7f;
            Options.stroke_color = Brushes.Black;*/
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
            ChartValues<float> values = datas.Find(attr => attr.Name == attribute).Datas;
            return convertToObservablePoints(filteredData(values));
        }

        ChartValues<float> timeDatas
        {
            get
            {
                return datas[0].Datas;
            }
        }

        ChartValues<ObservablePoint> convertToObservablePoints(ChartValues<float> filtered_datas)
        {
            ChartValues<ObservablePoint> return_datas = new ChartValues<ObservablePoint>();

            ChartValues<float> time = filteredData(timeDatas);

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

        ChartValues<float> filteredData(ChartValues<float> datas)
        {
            ChartValues<float> input_datas = new ChartValues<float>(datas);
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
