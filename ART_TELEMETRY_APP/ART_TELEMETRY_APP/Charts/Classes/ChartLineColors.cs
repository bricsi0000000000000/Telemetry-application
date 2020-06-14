using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ART_TELEMETRY_APP
{
    public static class ChartLineColors
    {
        static Random rand = new Random();

        static public Brush[] Colors = new Brush[] {
            Brushes.Aqua,
            Brushes.Chartreuse,
            Brushes.Crimson,
            Brushes.Gold,
            Brushes.Orange,
            Brushes.Tomato,
        };

        static public Brush RandomColor
        {
            get
            {
                return Colors[rand.Next(0, Colors.Length - 1)];
            }
        }

       // public List<Brush> UsedColors = new List<Brush>();
    }
}
