using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles
{
    public class Data
    {
        public string Name;
        public ChartValues<double> Datas;
        public LineSerieOptions Option;
        public List<ChartValues<ObservablePoint>> DatasInLaps = new List<ChartValues<ObservablePoint>>();
    }
}
