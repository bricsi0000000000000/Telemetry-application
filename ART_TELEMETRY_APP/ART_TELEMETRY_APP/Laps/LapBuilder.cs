using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings;
using LiveCharts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    public static class LapBuilder
    {
        static List<Point> map_points = new List<Point>();
        static List<Point> nearest_map_points = new List<Point>();
        static int lap_index = 0;

        public static List<Point> MapPoints
        {
            get
            {
                return map_points;
            }
            set
            {
                map_points = value;
            }
        }
        public static List<Point> NearestMapPoints
        {
            get
            {
                return nearest_map_points;
            }
            set
            {
                nearest_map_points = value;
            }
        }
        public static int LapIndex
        {
            get
            {
                return lap_index;
            }
            set
            {
                if (value >= 0 && value < LapManager.Laps.Count)
                {
                    lap_index = value;
                }
            }
        }

        public static void MakeLaps()
        {
            if (PilotManager.Pilots.Count > 0 && LapManager.AllLapSVG.Equals(""))
            {
                InputFile first_input_file = PilotManager.Pilots.First().InputFiles.First();
                ChartValues<double> latitude = new ChartValues<double>();
                ChartValues<double> longitude = new ChartValues<double>();

                try
                {
                    latitude = first_input_file.GetData("Latitude").Datas;
                }
                catch (Exception)
                {
                    MessageBox.Show("No latitude data found", "Map error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                try
                {
                    longitude = first_input_file.GetData("Longitude").Datas;
                }
                catch (Exception)
                {
                    MessageBox.Show("No longitude data found", "Map error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (latitude.Count > 0 && longitude.Count > 0)
                {
                    double x_min = double.MaxValue;
                    double y_min = double.MaxValue;
                    foreach (double item in latitude)
                    {
                        if (item != 0 && !double.IsNaN(item))
                        {
                            if (item < x_min)
                            {
                                x_min = item;
                            }
                        }
                    }
                    foreach (double item in longitude)
                    {
                        if (item != 0 && !double.IsNaN(item))
                        {
                            if (item < y_min)
                            {
                                y_min = item;
                            }
                        }
                    }

                    double scale = Math.Pow(10, 5);
                    List<double> scaled_latitude = new List<double>();
                    List<double> scaled_longitude = new List<double>();
                    for (int i = 0; i < latitude.Count; i++)
                    {
                        if (latitude[i] != 0)
                        {
                            if (!double.IsNaN(latitude[i]))
                            {
                                scaled_latitude.Add(Math.Round((latitude[i] - x_min) * scale));
                            }
                            if (longitude[i] != 0)
                            {
                                if (!double.IsNaN(longitude[i]))
                                {
                                    scaled_longitude.Add(Math.Round((longitude[i] - y_min) * scale));
                                }
                            }
                        }
                    }

                    for (int i = 0; i < scaled_latitude.Count; i++)
                    {
                        map_points.Add(new Point(scaled_latitude[i], scaled_longitude[i]));
                    }

                    string svg_path = string.Format("M{0} {1}", map_points[0].X, map_points[0].Y);
                    for (int i = 0; i < map_points.Count; i++)
                    {
                        svg_path += string.Format(" L{0} {1}", map_points[i].X, map_points[i].Y);
                    }
                    LapManager.AllLapSVG = svg_path;
                }
            }
        }

        /*
        #region instance
        private static LapBuilder instance = null;
        private LapBuilder() { }

        public static LapBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LapBuilder();
                }
                return instance;
            }
        }
        #endregion

        BackgroundWorker worker;
        ColorZone diagram_calculate_laps;
        Grid charts_grid;
        ColorZone diagram_nothing;
        Label act_lap_lbl;

        public void Build(ColorZone diagram_nothing, ColorZone diagram_calculate_laps, Grid charts_grid, Label act_lap_lbl)
        {
            this.diagram_calculate_laps = diagram_calculate_laps;
            this.charts_grid = charts_grid;
            this.diagram_nothing = diagram_nothing;
            this.act_lap_lbl = act_lap_lbl;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += calculateLaps;
            worker.ProgressChanged += workerProgressChanged;
            worker.RunWorkerCompleted += workerCompleted;
            worker.RunWorkerAsync(10000);

            diagram_nothing.Visibility = Visibility.Hidden;
            diagram_calculate_laps.Visibility = Visibility.Visible;
           
        }

        private void calculateLaps(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < GroupManager.GroupsCount; i++)
            {
               /* foreach (SelectedChannelSettings_UC attribute in GroupManager.Groups[i].SelectedChannelSettingsUCs)
                {
                    DataManager.GetData().MakeDatasInLaps(attribute.Attribute);
                }*/
        /* }
     }

     private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
     {

     }

     private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
     {
         diagram_calculate_laps.Visibility = Visibility.Hidden;

        // ChartBuilder.Instance.Build(charts_grid, diagram_nothing);
         act_lap_lbl.Content = string.Format("{0}/{1}", 1, DataManager.GetData().Laps.Count);
     }*/
    }
}
