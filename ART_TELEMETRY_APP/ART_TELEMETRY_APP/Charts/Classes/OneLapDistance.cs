using LiveCharts;
using System.Linq;

namespace ART_TELEMETRY_APP.Charts.Classes
{
    public class OneLapDistance
    {
        public ChartValues<double> DistanceValues { get; set; } = new ChartValues<double>();
        public double DistanceSum { get; set; }
    }
}
