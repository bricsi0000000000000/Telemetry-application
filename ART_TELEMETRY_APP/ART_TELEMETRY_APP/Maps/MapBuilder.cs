using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using LiveCharts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    public static class MapBuilder
    {
        public static void Build(Path map_svg_path)
        {
            //string svg_path = "";

            /* svg_path += string.Format("M{0} {1}", LapManager.Laps[lap_index].GetPoint(0).X, LapManager.Laps[lap_index].GetPoint(0).Y);
             for (int i = 0; i < LapManager.Laps[lap_index].Points.Count; i++)
             {
                 svg_path += string.Format(" L{0} {1}", LapManager.Laps[lap_index].GetPoint(i).X, LapManager.Laps[lap_index].GetPoint(i).Y);
             }*/

            map_svg_path.Data = Geometry.Parse(AverageMap);
            // Console.WriteLine(maps[0].SvgPathes[0].Item1);
            // map_svg_path.Data = Geometry.Parse(maps[0].SvgPathes[0].Item1);

            /*if (map_nothing != null)
            {
                map_nothing.Visibility = System.Windows.Visibility.Hidden;
            }*/
        }

        private static string AverageMap
        {
            get
            {
                int shortest_lap_length = 0;
                int index = 0;
                for (int i = 1; i < LapManager.Laps.Count - 1; i++)
                {
                    if (LapManager.Laps[i].Points.Count > shortest_lap_length)
                    {
                        shortest_lap_length = LapManager.Laps[i].Points.Count;
                        index = i;
                    }
                }

                List<Point> average_lap = LapManager.Laps[index].Points;
                for (int i = 0; i < shortest_lap_length; i++)
                {
                    try
                    {
                        double avg_x = 0;
                        double avg_y = 0;
                        for (int j = 1; j < LapManager.Laps.Count - 1; j++)
                        {
                            avg_x += LapManager.Laps[j].Points[i].X;
                            avg_y += LapManager.Laps[j].Points[i].Y;
                        }
                        avg_x /= LapManager.Laps.Count - 2;
                        avg_y /= LapManager.Laps.Count - 2;
                        average_lap[i] = new Point(avg_x, avg_y);
                    }
                    catch (Exception)
                    {
                    }
                }

                string svg_path = "";

                svg_path += string.Format("M{0} {1}", average_lap[0].X, average_lap[0].Y);
                for (int i = 0; i < average_lap.Count; i++)
                {
                    svg_path += string.Format(" L{0} {1}", average_lap[i].X, average_lap[i].Y);
                }

                return svg_path;
            }
        }

        public static void Make(string mape_name,
                         Path map_svg/*,
                         ProgressBar map_progressbar,
                         ColorZone map_progressbar_colorzone,
                         ColorZone map_nothing*/
                        )
        {
            Map map = new Map(mape_name,
                              map_svg/*,
                              map_progressbar,
                              map_progressbar_colorzone,
                              map_nothing*/
                              );
            // maps.Add(map);
        }


    }
}
