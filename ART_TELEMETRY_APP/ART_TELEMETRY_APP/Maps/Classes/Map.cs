using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ART_TELEMETRY_APP.Maps.Classes
{
    public class Map
    {
        string name;
        string year;
        Point start_point;
        bool processed;

        public Map(string name, string year)
        {
            this.name = name;
            this.year = year;
            processed = false;
        }

        public string Name { get => name; set => name = value; }
        public string Year { get => year; set => year = value; }
        public Point StarPoint { get => start_point; set => start_point = value; }
        public bool Processed { get => processed; set => processed = value; }
    }
}
