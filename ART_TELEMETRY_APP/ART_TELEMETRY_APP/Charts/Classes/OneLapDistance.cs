using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace ART_TELEMETRY_APP.Charts.Classes
{
    public class OneLapDistance
    {
        public List<double> DistanceValues { get; set; } = new List<double>();
        public double DistanceSum { get; set; }
        public int FromIndex{ get; set; }
        public int ToIndex{ get; set; }
    }
}
