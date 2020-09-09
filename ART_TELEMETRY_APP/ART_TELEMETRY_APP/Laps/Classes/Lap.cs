using System;
using System.Collections.Generic;
using System.Windows;

namespace ART_TELEMETRY_APP.Laps
{
    public class Lap
    {
        public List<string> SelectedChannels { get; set; }

        public void AddPoint(Point point) => Points.Add(point);

        public Point GetPoint(int index) => Points[index];

        public List<Point> Points { get; } = new List<Point>();

        public int Index { get; set; }

        public int FromIndex { get; set; }

        public int ToIndex { get; set; }

        public string Svg { get; set; }

        public TimeSpan Time { get; set; }

        public ushort LapLength => (ushort)Points.Count;
    }
}
