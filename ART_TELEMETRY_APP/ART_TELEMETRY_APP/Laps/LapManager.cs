using ART_TELEMETRY_APP.Pilots;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Laps
{
    public static class LapManager
    {
        static List<Lap> laps = new List<Lap>();
        public static List<Lap> Laps
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

        public static string AllLapSVG = "";

        public static List<string> LapsSVG = new List<string>();
       
        public static Lap GetLap(int index)
        {
            return laps[index];
        }
       
    }
}
