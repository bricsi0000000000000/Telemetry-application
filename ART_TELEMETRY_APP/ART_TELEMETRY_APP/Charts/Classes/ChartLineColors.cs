using MaterialDesignThemes.Wpf.Converters.CircularProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ART_TELEMETRY_APP
{
    public static class ChartLineColors
    {
        private static BrushConverter brushConverter = new BrushConverter();
        private static Brush[] Colors => new Brush[]{
            (Brush)brushConverter.ConvertFromString("#fc0505"),
            (Brush)brushConverter.ConvertFromString("#fc7c05"),
            (Brush)brushConverter.ConvertFromString("#fce705"),
            (Brush)brushConverter.ConvertFromString("#80fc05"),
            (Brush)brushConverter.ConvertFromString("#05fcf4"),
            (Brush)brushConverter.ConvertFromString("#8005fc"),
            (Brush)brushConverter.ConvertFromString("#e305fc"),
        };

        private static int colorIndex = 0;
        private static int ColorIndex
        {
            get
            {
                return colorIndex;
            }
            set
            {
                if (value >= Colors.Length - 1)
                {
                    value = 0;
                }
                colorIndex = value;
            }
        }

         public static Brush GetColor => Colors[ColorIndex++];

     /*   private static List<SolidColorBrush> usedColors = new List<SolidColorBrush>();
        public static Brush GetColor
        {
            get
            {
                Random random = new Random();
                SolidColorBrush randomColor = new SolidColorBrush(HsvToRgb(random.Next(100, 255), .95f, .95f));
                do
                {
                    randomColor = new SolidColorBrush(HsvToRgb(random.Next(100, 255), .95f, .95f));
                }
                while (usedColors.Contains(randomColor));
                usedColors.Add(randomColor);

                return randomColor;
            }
        }

        private static Color HsvToRgb(double h, double S, double V)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }

            return Color.FromRgb(Clamp((int)(R * 255.0)), Clamp((int)(G * 255.0)), Clamp((int)(B * 255.0)));
        }

        private static byte Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return (byte)i;
        }*/

        /* private static readonly List<Brush> AllColors = new List<Brush>();
         public static Brush RandomColor
         {
             get
             {
                 if (AllColors.Count <= 0)
                 {
                     foreach (Brush color in Colors)
                     {
                         AllColors.Add(color);
                     }
                 }
                 Random random = new Random();
                 Brush randomColor = AllColors[random.Next(0, AllColors.Count - 1)];
                 AllColors.Remove(randomColor);

                 return randomColor;
             }
         }*/
    }
}
