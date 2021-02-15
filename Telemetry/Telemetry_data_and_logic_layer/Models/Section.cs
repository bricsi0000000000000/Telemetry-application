
using System;

namespace Telemetry_data_and_logic_layer.Models
{
    public class Section
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsLive { get; set; }
        public string DateString => Date.ToString();
    }
}
