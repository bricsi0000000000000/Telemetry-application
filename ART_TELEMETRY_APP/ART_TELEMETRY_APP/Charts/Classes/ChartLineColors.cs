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
        static Random random = new Random();

        private static Brush[] colors => new Brush[]{ Brushes.Aqua,
                                                      Brushes.Chartreuse,
                                                      Brushes.Crimson,
                                                      Brushes.Gold,
                                                      Brushes.Orange,
                                                      Brushes.Tomato };

        public static Brush RandomColor
        {
            get
            {
                if (all_colors.Count <= 0)
                {
                    foreach (Brush item in colors)
                    {
                        all_colors.Add(item);
                    }
                }
                Brush color = all_colors[random.Next(0, all_colors.Count - 1)];
                all_colors.Remove(color);
                return color;
            }
        }

        static private List<Brush> all_colors = new List<Brush>();
    }
}
