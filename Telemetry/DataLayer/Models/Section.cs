using System;

namespace DataLayer.Models
{
    public class Section
    {
        public int ID { get; set; }
        public bool IsLive { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string DateString => Date.ToString();
        public string SensorNames { get; set; }
    }
}
