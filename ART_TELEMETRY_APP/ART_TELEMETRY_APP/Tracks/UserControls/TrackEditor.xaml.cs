using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Tracks.UserControls
{
    /// <summary>
    /// Interaction logic for <seealso cref="TrackEditor"/>.xaml
    /// </summary>
    public partial class TrackEditor : UserControl
    {
        private readonly InputFile inputFile;
        private readonly Track track;
        private readonly Grid driverProgressBarGrid;
        private BackgroundWorker worker;
        private Point cursor;

        public TrackEditor(InputFile inputFile, Track track, Grid driverProgressBarGrid = null)
        {
            InitializeComponent();

            this.inputFile = inputFile;
            this.track = track;
            this.driverProgressBarGrid = driverProgressBarGrid;

            AllLapsSVG.Data = Geometry.Parse(inputFile.AllLapsSVG);

            if (TrackManager.GetTrack(track).StarPoint != new Point(-1, -1))
            {
                cursor = TrackManager.GetTrack(track).StarPoint;
            }

            if (!TrackManager.GetTrack(track).Processed)
            {
                StartWorker();
                TrackManager.GetTrack(track).Processed = true;
            }
            else
            {
                inputFile.CalculateAllDistances();
            }
        }

        double Distance(Point startPoint, Point endPoint) => Math.Sqrt(Math.Pow(startPoint.X - endPoint.X, 2) + Math.Pow(startPoint.Y - endPoint.Y, 2));

        private void AllLapsCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            /* Point cursor = Mouse.GetPosition(all_lap_canvas);
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
             Canvas.SetTop(start_line_ellipse, min_point.Y);*/
        }

        private void StartWorker()
        {
            if (driverProgressBarGrid != null)
            {
                driverProgressBarGrid.Visibility = Visibility.Visible;
            }
            driverProgressBarGrid.Visibility = Visibility.Visible;
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += WorkerDoWork;
            worker.ProgressChanged += WorkerProgressChanged;
            worker.RunWorkerCompleted += WorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void AllLapsCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cursor = Mouse.GetPosition(AllLapsCanvas);

            TrackManager.ChangeMapsStartPoint(track, cursor);

            StartWorker();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            inputFile.Laps.Clear();
            var nearestPoints = new List<Point>();
            short radius = 20;

            foreach (Point point in inputFile.TrackPoints)
            {
                if (Distance(cursor, point) <= radius)
                {
                    nearestPoints.Add(point);
                }
            }

            if (nearestPoints.Count >= 2)
            {
                var random = new Random();
                Point randomPoint = nearestPoints[random.Next(0, nearestPoints.Count)];

                short after = 500;
                radius = 40;

                Lap actualLap = new Lap();

                int newLapIndex = 0;
                int lastCircleIndex = 0;

                for (int trackPointIndex = 0; trackPointIndex < inputFile.TrackPoints.Count; trackPointIndex++)
                {
                    bool canAdd = false;

                    actualLap.AddPoint(inputFile.TrackPoints[trackPointIndex]);

                    canAdd = trackPointIndex + 1 >= inputFile.TrackPoints.Count;

                    after = inputFile.Laps.Count <= 0 ? (short)0 : (short)500;

                    if (trackPointIndex >= lastCircleIndex + after)
                    {
                        if (Math.Sqrt(Math.Pow(inputFile.TrackPoints[trackPointIndex].X - randomPoint.X, 2) + Math.Pow(inputFile.TrackPoints[trackPointIndex].Y - randomPoint.Y, 2)) <= radius)
                        {
                            canAdd = true;

                            actualLap.FromIndex = lastCircleIndex;
                            actualLap.ToIndex = trackPointIndex;
                            actualLap.Index = newLapIndex++;

                            lastCircleIndex = trackPointIndex;

                            if (trackPointIndex + 1 < inputFile.TrackPoints.Count)
                            {
                                actualLap.AddPoint(inputFile.TrackPoints[trackPointIndex + 1]);
                                lastCircleIndex = trackPointIndex + 1;
                                actualLap.ToIndex = trackPointIndex + 1;
                            }
                        }
                    }

                    if (canAdd)
                    {
                        inputFile.Laps.Add(actualLap);
                        actualLap = new Lap();
                    }
                }

                inputFile.Laps.Last().FromIndex = lastCircleIndex;
                inputFile.Laps.Last().ToIndex = inputFile.TrackPoints.Count;
                inputFile.Laps.Last().Index = newLapIndex;

                /*inputFile.LapsSVGs.Clear();
                for (int i = 0; i < input_file.Laps.Count; i++)
                {
                    string svg_path = string.Format("M{0} {1}", input_file.Laps[i].GetPoint(0).X, input_file.Laps[i].GetPoint(0).Y);
                    for (int index = 0; index < input_file.Laps[i].Points.Count; index++)
                    {
                        svg_path += string.Format(" L{0} {1}", input_file.Laps[i].GetPoint(index).X, input_file.Laps[i].GetPoint(index).Y);
                    }
                    input_file.LapsSVGs.Add(svg_path);
                }*/

               // inputFile.MakeAvgLap();
                inputFile.CalculateLapTimes();
                //inputFile.InitActiveLaps();
                inputFile.CalculateAllDistances();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    /*foreach (TabItem item in ((DriverContentTab)((DiagramsMenu)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(inputFile.DriverName).Content).Tabs)
                    {
                        var lapsContent = (LapsContent)item.Content;
                        if (lapsContent != null)
                        {
                            lapsContent.InitFirstInputFilesContent();
                            lapsContent.InputFilesCmbboxSelectionChange();
                        }
                    }*/
                    //((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(input_file.PilotName).Content).GetTab(TextManager.DiagramCustomTabName).Content).InitFirstInputFilesContent();
                    //((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(input_file.PilotName).Content).InitTabs();
                    //((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).InitPilotsTabs();
                });
            }
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (driverProgressBarGrid != null)
            {
                driverProgressBarGrid.Visibility = Visibility.Hidden;
            }
            ProgressBarGrid.Visibility = Visibility.Hidden;

            //  avg_lap_svg.Data = Geometry.Parse(input_file.OneLap());

            /*   if (input_file.Laps.Count > 0)
               {
                   makeLapData();
               }*/
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
