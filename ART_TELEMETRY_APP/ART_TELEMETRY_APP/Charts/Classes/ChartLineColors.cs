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
                if(all_colors.Count <= 0)
                {
                    foreach (Brush item in Colors)
                    {
                        all_colors.Add(item);
                    }
                }
                Brush color = all_colors[rand.Next(0, all_colors.Count - 1)];
                all_colors.Remove(color);
                return color;
            }
        }

        static private List<Brush> all_colors = new List<Brush>();
    }
}
