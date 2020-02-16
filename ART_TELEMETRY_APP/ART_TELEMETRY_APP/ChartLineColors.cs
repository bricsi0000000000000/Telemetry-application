using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ART_TELEMETRY_APP
{
    class ChartLineColors
    {
        private static ChartLineColors instance = null;
        private ChartLineColors() { }
        public static ChartLineColors Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChartLineColors();
                }
                return instance;
            }
        }

        public Brush[] Colors = new Brush[] {
            Brushes.Aqua,
            Brushes.Black,
            Brushes.Blue,
            Brushes.Brown,
            Brushes.Chartreuse,
            Brushes.Crimson,
            Brushes.DeepPink,
            Brushes.Gray,
            Brushes.Gold,
            Brushes.Green,
            Brushes.OrangeRed,
            Brushes.Red,
        };

        public List<Brush> UsedColors = new List<Brush>();
    }
}
