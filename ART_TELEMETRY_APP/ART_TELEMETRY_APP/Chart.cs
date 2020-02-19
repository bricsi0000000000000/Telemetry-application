using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class Chart
    {
        string name;
        List<LineSeries> line_series = new List<LineSeries>();

        public Chart(string name)
        {
            this.name = name;
        }

        public void AddLineSerie(LineSeries line_serie)
        {
            line_series.Add(line_serie);
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                name = value;
            }
        }

        public List<LineSeries> LineSeries
        {
            get
            {
                return this.line_series;
            }
            set
            {
                line_series = value;
            }
        }
    }
}
