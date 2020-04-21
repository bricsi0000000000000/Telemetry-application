using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Maps;
using ART_TELEMETRY_APP.Pilots;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Settings
{
    /// <summary>
    /// Interaction logic for MapSettings_UC.xaml
    /// </summary>
    public partial class MapSettings_UC : UserControl
    {
        public MapSettings_UC()
        {
            InitializeComponent();

            drawActLap();
        }

        double distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void AllLapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point cursor = Mouse.GetPosition(all_lap_canvas);

            double min_distance = double.MaxValue;
            Point min_point = new Point();
            foreach (Point point in LapBuilder.MapPoints)
            {
                if (distance(cursor, point) < min_distance)
                {
                    min_distance = distance(cursor, point);
                    min_point = point;
                }
            }

            Canvas.SetLeft(start_line_ellipse, min_point.X);
            Canvas.SetTop(start_line_ellipse, min_point.Y);
        }

        private void AllLapCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LapManager.Laps.Clear();
            LapBuilder.LapIndex = 0;
            LapBuilder.NearestMapPoints.Clear();
            short radius = 20;

            Point cursor = Mouse.GetPosition(all_lap_canvas);

            foreach (Point point in LapBuilder.MapPoints)
            {
                if (distance(cursor, point) <= radius)
                {
                    LapBuilder.NearestMapPoints.Add(point);
                }
            }

            if (LapBuilder.NearestMapPoints.Count >= 2)
            {
                Random rand = new Random();
                Point random_point = LapBuilder.NearestMapPoints[rand.Next(0, LapBuilder.NearestMapPoints.Count)];

                short after = 500;
                radius = 40;

                Lap act_lap = new Lap();

                int last_circle_index = 0;

                for (int i = 0; i < LapBuilder.MapPoints.Count; i++)
                {
                    bool can_add = false;

                    act_lap.AddPoint(LapBuilder.MapPoints[i]);

                    if (i + 1 >= LapBuilder.MapPoints.Count)
                    {
                        can_add = true;
                    }

                    if (LapManager.Laps.Count <= 0)
                    {
                        after = 0;
                    }
                    else
                    {
                        after = 500;
                    }

                    if (i >= last_circle_index + after)
                    {
                        if (Math.Sqrt(Math.Pow(LapBuilder.MapPoints[i].X - random_point.X, 2) + Math.Pow(LapBuilder.MapPoints[i].Y - random_point.Y, 2)) <= radius)
                        {
                            can_add = true;

                            act_lap.FromIndex = last_circle_index;
                            act_lap.ToIndex = i;

                            last_circle_index = i;

                            if (i + 1 < LapBuilder.MapPoints.Count)
                            {
                                act_lap.AddPoint(LapBuilder.MapPoints[i + 1]);
                                last_circle_index = i + 1;
                                act_lap.ToIndex = i + 1;
                            }
                        }
                    }

                    if (can_add)
                    {
                        LapManager.Laps.Add(act_lap);
                        act_lap = new Lap();
                    }
                }

                LapManager.Laps.Last().FromIndex = last_circle_index;
                //LapManager.Laps.Last().ToIndex = last_circle_index + LapManager.Laps.Last().Points.Count;
                LapManager.Laps.Last().ToIndex = LapBuilder.MapPoints.Count;

                foreach (var item in LapManager.Laps)
                {
                    Console.WriteLine(item.ToIndex - item.FromIndex);
                }

                laps_lbl.Content = string.Format("Laps: {0}", LapManager.Laps.Count - 2);

                LapManager.LapsSVG.Clear();
                for (int i = 0; i < LapManager.Laps.Count; i++)
                {
                    string svg_path = string.Format("M{0} {1}", LapManager.Laps[i].GetPoint(0).X, LapManager.Laps[i].GetPoint(0).Y);
                    for (int index = 0; index < LapManager.Laps[i].Points.Count; index++)
                    {
                        svg_path += string.Format(" L{0} {1}", LapManager.Laps[i].GetPoint(index).X, LapManager.Laps[i].GetPoint(index).Y);
                    }
                    LapManager.LapsSVG.Add(svg_path);
                }

                drawActLap();

                //TabManager.GetTab("Diagrams").DiagramsGroupTabsUI.InitGroupTabs();
                foreach (Group group in GroupManager.Groups)
                {
                    group.ClearSelectedPilotsAndLaps();
                   // ChartBuilder.Build(SettingsManager.GetDiagramsUc(group.Name).diagrams_grid, group);
                    MapBuilder.Build(SettingsManager.GetDiagramsUc(group.Name).one_lap_svg);
                }
            }
        }

        private void drawActLap()
        {
            if (LapManager.Laps.Count > 0)
            {
                one_lap_svg.Data = Geometry.Parse(LapManager.LapsSVG[LapBuilder.LapIndex]); //TODO: ha kevesebb lap lesz mint amennyin volt a LapBuilder.LapIndex nem jó

                string act_lap_txt = "";
                if (LapBuilder.LapIndex == 0)
                {
                    act_lap_txt = "In lap\n";
                }
                if (LapBuilder.LapIndex == LapManager.Laps.Count - 1)
                {
                    act_lap_txt = "Out lap\n";
                }
                act_lap_lbl.Content = string.Format("{0}{1}/{2}", act_lap_txt, LapBuilder.LapIndex + 1, LapManager.Laps.Count);
            }
        }

        private void PrevLap_Click(object sender, RoutedEventArgs e)
        {
            LapBuilder.LapIndex--;
            drawActLap();
        }

        private void NextLap_Click(object sender, RoutedEventArgs e)
        {
            LapBuilder.LapIndex++;
            drawActLap();
        }
    }
}
