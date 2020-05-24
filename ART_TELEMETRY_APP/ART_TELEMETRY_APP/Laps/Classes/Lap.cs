using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ART_TELEMETRY_APP.Laps
{
    public class Lap
    {
        List<Point> points = new List<Point>();
        int index;
        int from_index;
        int to_index;
        string svg;
        TimeSpan time;
        List<string> selected_channels = new List<string>();

        public void AddPoint(Point point)
        {
            points.Add(point);
        }

        public Point GetPoint(int index)
        {
            return points[index];
        }

        public List<Point> Points
        {
            get
            {
                return points;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        public int FromIndex
        {
            get
            {
                return from_index;
            }
            set
            {
                from_index = value;
            }
        }

        public int ToIndex
        {
            get
            {
                return to_index;
            }
            set
            {
                to_index = value;
            }
        }

        public string Svg
        {
            get
            {
                return svg;
            }
            set
            {
                svg = value;
            }
        }

        public TimeSpan Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        public List<string> SelectedChannels
        {
            get
            {
                return selected_channels;
            }
            set
            {
                value = selected_channels;
            }
        }
    }
}
