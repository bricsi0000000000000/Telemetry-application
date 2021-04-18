using System;
using System.Collections.Generic;

namespace DataLayer.Models
{
    public class Package
    {
        public List<Speed> Speeds { get; set; } = new List<Speed>();
        public List<Time> Times { get; set; } = new List<Time>();
        public TimeSpan SentTime { get; set; }
    }
}
