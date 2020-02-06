using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class Charts
    {
        #region instance
        private static Charts instance = null;
        private Charts() { }

        public static Charts Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Charts();
                }
                return instance;
            }
        }
        #endregion

        List<Chart> charts = new List<Chart>();

        public void AddChart(Chart chart)
        {
            charts.Add(chart);
        }

        public Chart GetChart(string name)
        {
            return charts.Find(n => n.Name == name);
        }
    }
}
