using LiveCharts;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace ART_TELEMETRY_APP.Charts.Classes
{
    public class OneLapDistance
    {
        public List<float> DistanceValues { get; set; } = new List<float>();
        public float DistanceSum { get; set; }
        public int FromIndex{ get; set; }
        public int ToIndex{ get; set; }
    }
}
