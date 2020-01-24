using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LiveCharts;

namespace ART_TELEMETRY_APP
{
    class DataTable
    {
        List<Tuple<string, ChartValues<float>>> datas = new List<Tuple<string, ChartValues<float>>>();

        public DataTable(string attribute, ChartValues<float> entities)
        {
            datas.Add(new Tuple<string, ChartValues<float>>(attribute, entities));
        }

        public List<Tuple<string, ChartValues<float>>> Datas { get => datas; set => datas = value; }

        public ChartValues<float> getFiltered
        {
            get
            {
                ChartValues<float> filtered = new ChartValues<float>(datas[0].Item2);

                int total = filtered.Count;
                Random rand = new Random(DateTime.Now.Millisecond);
                while (filtered.Count / (float)total > .004)
                {
                    filtered.RemoveAt(rand.Next(1, filtered.Count - 1));
                }

                return filtered;
            }
        }
    }
}
