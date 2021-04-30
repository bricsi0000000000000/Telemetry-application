using System;
using System.Collections.Generic;

namespace DataLayer.Models
{
    public class Package
    {
        public int ID { get; set; }
        public List<Speed> SpeedValues { get; set; } = new List<Speed>();
        public List<Time> TimeValues { get; set; } = new List<Time>();
        public List<Yaw> YawValues { get; set; } = new List<Yaw>();
        public TimeSpan SentTime { get; set; }
    }
}
