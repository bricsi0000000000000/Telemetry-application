using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Maps.UserControls
{
    /// <summary>
    /// Interaction logic for MapEditor_UC.xaml
    /// </summary>
    public partial class MapEditor_UC : UserControl
    {
        private readonly InputFile input_file;
        private readonly Map map;
        private readonly Grid pilot_progressbar_grid;
        private readonly List<OnlyLapListElement> lap_list_elements = new List<OnlyLapListElement>();
        private BackgroundWorker worker;
        private Point cursor;

        public MapEditor_UC(InputFile input_file, Map map, Grid pilot_progressbar_grid = null)
        {
            InitializeComponent();

            this.input_file = input_file;
            this.map = map;
            this.pilot_progressbar_grid = pilot_progressbar_grid;

            all_lap_svg.Data = Geometry.Parse(input_file.AllLapsSVG);

            if (MapManager.GetMap(map).StarPoint != new Point(-1, -1))
            {
                cursor = MapManager.GetMap(map).StarPoint;
            }

            if (!MapManager.GetMap(map).Processed)
            {
                startWorker();
                MapManager.GetMap(map).Processed = true;
            }
            else
            {
                //avg_lap_svg.Data = Geometry.Parse(input_file.OneLap());
                makeLapData();
            }
        }

        double distance(Point start_point, Point end_point) => Math.Sqrt(Math.Pow(start_point.X - end_point.X, 2) + Math.Pow(start_point.Y - end_point.Y, 2));

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

        private void startWorker()
        {
            if (pilot_progressbar_grid != null)
            {
                pilot_progressbar_grid.Visibility = Visibility.Visible;
            }
            progressbar_grid.Visibility = Visibility.Visible;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += workerDoWork;
            worker.ProgressChanged += workerProgressChanged;
            worker.RunWorkerCompleted += workerCompleted;
            worker.RunWorkerAsync(10000);
        }

        private void AllLapCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cursor = Mouse.GetPosition(all_lap_canvas);

            MapManager.ChangeMapsStartPoint(map, cursor);

            startWorker();
        }

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            input_file.Laps.Clear();
            List<Point> nearest_map_points = new List<Point>();
            short radius = 20;

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

                int new_lap_index = 0;
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
                            act_lap.Index = new_lap_index++;

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
                input_file.Laps.Last().Index = new_lap_index;

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
                input_file.CalculateLapTimes();
                input_file.InitActiveLaps();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (TabItem item in ((DriverContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(input_file.DriverName).Content).Tabs)
                    {
                        if (item is LapsContent)
                        {
                            ((LapsContent)item.Content).InitFirstInputFilesContent();
                            ((LapsContent)item.Content).InputFilesCmbboxSelectionChange();
                        }
                    }
                    //((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(input_file.PilotName).Content).GetTab(TextManager.DiagramCustomTabName).Content).InitFirstInputFilesContent();
                    //((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(input_file.PilotName).Content).InitTabs();
                    //((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).InitPilotsTabs();
                });
            }
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (pilot_progressbar_grid != null)
            {
                pilot_progressbar_grid.Visibility = Visibility.Hidden;
            }
            progressbar_grid.Visibility = Visibility.Hidden;

            //  avg_lap_svg.Data = Geometry.Parse(input_file.OneLap());

            if (input_file.Laps.Count > 0)
            {
                makeLapData();
            }
        }

        private void makeLapData()
        {
            lap_list_elements.Clear();
            laps_stackpanel.Children.Clear();

            TimeSpan all_time = new TimeSpan();

            ushort worst_index = 1;
            ushort best_index = 1;
            TimeSpan worst_time = input_file.Laps[1].Time;
            TimeSpan best_time = input_file.Laps[1].Time;
            for (ushort i = 2; i < input_file.Laps.Count - 1; i++)
            {
                if (input_file.Laps[i].Time > worst_time)
                {
                    worst_time = input_file.Laps[i].Time;
                    worst_index = i;
                }
            }

            for (ushort i = 2; i < input_file.Laps.Count - 1; i++)
            {
                if (input_file.Laps[i].Time < best_time)
                {
                    best_time = input_file.Laps[i].Time;
                    best_index = i;
                }
            }

            for (ushort i = 0; i < input_file.Laps.Count; i++)
            {
                OnlyLapListElement lap_list_element;
                if (i + 1 >= input_file.Laps.Count)
                {
                    lap_list_element = new OnlyLapListElement(input_file.Laps[i],
                                                              i > 0 && i < input_file.Laps.Count - 1 ? i == best_index ? 1 : i == worst_index ? 0 : 2 : 2,
                                                              true
                                                              );
                }
                else
                {
                    lap_list_element = new OnlyLapListElement(input_file.Laps[i],
                                                              i > 0 && i < input_file.Laps.Count - 1 ? i == best_index ? 1 : i == worst_index ? 0 : 2 : 2
                                                              );
                }

                lap_list_elements.Add(lap_list_element);
                laps_stackpanel.Children.Add(lap_list_element);

                all_time += input_file.Laps[i].Time;
            }

            OnlyLapListElement all_lap_list_element = new OnlyLapListElement(all_time);
            laps_stackpanel.Children.Insert(0, all_lap_list_element);
        }

        //private void

        /*private void drawActLap()
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
        }*/

        /* public int lapIndex
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
         }*/
    }
}
