using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class Maps
    {
        List<string> svg_pathes = new List<string>();

        public void CalculateSvgPathes()
        {
            foreach (List<Tuple<double, double>> lap in Datas.Instance.GetData().Laps)
            {
               
            }
        }
    }
}
