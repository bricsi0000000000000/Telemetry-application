using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;

namespace ART_TELEMETRY_APP
{
    class Chart
    {
        string group_name;
        string title;
        ChartValues<float> datas = new ChartValues<float>();

        public Chart(string group_name, string title, ChartValues<float> datas)
        {
            this.group_name = group_name;
            this.title = title;
            this.datas = datas;
        }

        public string Title { get => title; }
        public ChartValues<float> Datas { get => datas; }
        public string GroupName { get => group_name; }
    }
}
