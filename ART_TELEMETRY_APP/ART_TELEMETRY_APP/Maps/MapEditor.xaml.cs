using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
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
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Maps
{
    /// <summary>
    /// Interaction logic for MapEditor.xaml
    /// </summary>
    public partial class MapEditor : Window
    {
        InputFile input_file;
        int lap_index;

        public MapEditor(InputFile input_file)
        {
            InitializeComponent();

            this.input_file = input_file;

            all_lap_svg.Data = Geometry.Parse(input_file.AllLapsSVG);
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
            foreach (Point point in input_file.MapPoints)
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
            input_file.Laps.Clear();
            lapIndex = 0;
            List<Point> nearest_map_points = new List<Point>();
            short radius = 20;

            Point cursor = Mouse.GetPosition(all_lap_canvas);

            foreach (Point point in input_file.MapPoints)
            {
                if (distance(cursor, point) <= radius)
                {
                    nearest_map_points.Add(point);
                }
            }

            if (nearest_map_points.Count >= 2)
            {
                Random rand = new Random();
                Point random_point = nearest_map_points[rand.Next(0, nearest_map_points.Count)];

                short after = 500;
                radius = 40;

                Lap act_lap = new Lap();

                int lap_index = 0;
                int last_circle_index = 0;

                for (int i = 0; i < input_file.MapPoints.Count; i++)
                {
                    bool can_add = false;

                    act_lap.AddPoint(input_file.MapPoints[i]);

                    if (i + 1 >= input_file.MapPoints.Count)
                    {
                        can_add = true;
                    }

                    if (input_file.Laps.Count <= 0)
                    {
                        after = 0;
                    }
                    else
                    {
                        after = 500;
                    }

                    if (i >= last_circle_index + after)
                    {
                        if (Math.Sqrt(Math.Pow(input_file.MapPoints[i].X - random_point.X, 2) + Math.Pow(input_file.MapPoints[i].Y - random_point.Y, 2)) <= radius)
                        {
                            can_add = true;

                            act_lap.FromIndex = last_circle_index;
                            act_lap.ToIndex = i;
                            act_lap.Index = lap_index++;

                            last_circle_index = i;

                            if (i + 1 < input_file.MapPoints.Count)
                            {
                                act_lap.AddPoint(input_file.MapPoints[i + 1]);
                                last_circle_index = i + 1;
                                act_lap.ToIndex = i + 1;
                            }
                        }
                    }

                    if (can_add)
                    {
                        input_file.Laps.Add(act_lap);
                        act_lap = new Lap();
                    }
                }

                input_file.Laps.Last().FromIndex = last_circle_index;
                input_file.Laps.Last().ToIndex = input_file.MapPoints.Count;

                laps_lbl.Content = string.Format("Laps: {0}", input_file.Laps.Count - 2);

                input_file.LapsSVGs.Clear();
                for (int i = 0; i < input_file.Laps.Count; i++)
                {
                    string svg_path = string.Format("M{0} {1}", input_file.Laps[i].GetPoint(0).X, input_file.Laps[i].GetPoint(0).Y);
                    for (int index = 0; index < input_file.Laps[i].Points.Count; index++)
                    {
                        svg_path += string.Format(" L{0} {1}", input_file.Laps[i].GetPoint(index).X, input_file.Laps[i].GetPoint(index).Y);
                    }
                    input_file.LapsSVGs.Add(svg_path);
                }

                input_file.MakeAvgLap();
                input_file.MakeLapTimes();
                input_file.InitActiveLaps();

                drawActLap();

               // ChartBuilder.Build(((PilotContent)((DatasMenuContent)TabManager.GetTab("Datas").Content).GetTab(PilotManager.GetPilot(input_file.PilotName).Name).Content).charts_grid)

                //TabManager.GetTab("Diagrams").DiagramsGroupTabsUI.InitGroupTabs();
                /* foreach (Group group in GroupManager.Groups)
                 {
                     group.ClearSelectedPilotsAndLaps();
                     ChartBuilder.Build(SettingsManager.GetDiagramsUc(group.Name).diagrams_grid, group);
                     MapBuilder.Build(SettingsManager.GetDiagramsUc(group.Name).one_lap_svg);
                 }*/
            }
        }

        private void drawActLap()
        {
            if (input_file.Laps.Count > 0)
            {
                one_lap_svg.Data = Geometry.Parse(input_file.LapsSVGs[lapIndex]); //TODO: ha kevesebb lap lesz mint amennyin volt a LapBuilder.LapIndex nem jó

                string act_lap_txt = "";
                if (lapIndex == 0)
                {
                    act_lap_txt = "In lap\n";
                }
                if (lapIndex == input_file.Laps.Count - 1)
                {
                    act_lap_txt = "Out lap\n";
                }
                act_lap_lbl.Content = string.Format("{0}{1}/{2}", act_lap_txt, lapIndex + 1, input_file.Laps.Count);
            }
        }

        private void PrevLap_Click(object sender, RoutedEventArgs e)
        {
            lapIndex--;
            drawActLap();
        }

        private void NextLap_Click(object sender, RoutedEventArgs e)
        {
            lapIndex++;
            drawActLap();
        }

        public int lapIndex
        {
            get
            {
                return lap_index;
            }
            set
            {
                if (value >= 0 && value < input_file.Laps.Count)
                {
                    lap_index = value;
                }
            }
        }
    }
}
