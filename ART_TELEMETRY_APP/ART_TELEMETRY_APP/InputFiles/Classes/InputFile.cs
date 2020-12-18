using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    /// <summary>
    /// This class represents one input files data.
    /// </summary>
    public class InputFile
    {
        public InputFile(string fileName, string driverName, List<Channel> channels, ref Snackbar errorSnackbar, Feedback feedback)
        {
            FileName = fileName;
            DriverName = driverName;
            Channels = channels;

            GetImportantChannelNames(ref errorSnackbar, feedback);

            TrackPoints = new List<Point>();
            Laps = new List<Lap>();
            CalculateAllLapsSVG();

            IsSelected = false;
        }

        public Tuple<double[], double[]> PlotTrackPoints => new Tuple<double[], double[]>(Latitude.ToArray(), Longitude.ToArray());

        public List<Channel> Channels { get; } = new List<Channel>();


        public delegate void Feedback(bool foundAll);

        private void GetImportantChannelNames(ref Snackbar errorSnackbar, Feedback feedback)
        {
            var cantFindChannelNames = new List<string>();

            if (CanFindChannelName(TextManager.DefaultLatitudeChannelName))
                latitudeChannelName = TextManager.DefaultLatitudeChannelName;
            else
                cantFindChannelNames.Add(TextManager.DefaultLatitudeChannelName);


            if (CanFindChannelName(TextManager.DefaultLongitudeChannelName))
                longitudeChannelName = TextManager.DefaultLongitudeChannelName;
            else
                cantFindChannelNames.Add(TextManager.DefaultLongitudeChannelName);


            if (CanFindChannelName(TextManager.DefaultSpeedChannelName))
                speedChannelName = TextManager.DefaultSpeedChannelName;
            else
                cantFindChannelNames.Add(TextManager.DefaultSpeedChannelName);


            if (CanFindChannelName(TextManager.DefaultTimeChannelName))
                timeChannelName = TextManager.DefaultTimeChannelName;
            else
                cantFindChannelNames.Add(TextManager.DefaultTimeChannelName);

            if (cantFindChannelNames.Count > 0)
            {
                feedback(false);
                string errorMessage = "";
                foreach (var channelName in cantFindChannelNames)
                {
                    errorMessage += channelName + ", ";
                }
                errorMessage = errorMessage.Substring(0, errorMessage.Length - 2);
                ShowError.ShowErrorMessage(ref errorSnackbar, string.Format("'{0}' not found! You can change it in settings", errorMessage), 3);
            }
        }

        private bool CanFindChannelName(string defaultChannelName)
        {
            return Channels.Find(x => x.Name.Equals(defaultChannelName)) != null;
        }

        private string latitudeChannelName;
        private string longitudeChannelName;
        private string timeChannelName;
        private string speedChannelName;

        public bool IsSelected { get; set; }
        private int ChannelDataCount => Channels.First().Data.Count;
        public string FileName { get; private set; }
        public string DriverName { get; private set; }
        public Track ActiveTrack { get; set; }
        public string AllLapsSVG { get; private set; }
        public string AllLapSVGWithStartPoint { get; private set; }
        public List<double> Latitude => Channels.Find(x => x.Name.Equals(TextManager.DefaultLatitudeChannelName)).Data.ToList();
        public List<double> Longitude => Channels.Find(x => x.Name.Equals(TextManager.DefaultLongitudeChannelName)).Data.ToList();
        public List<double> Times => Channels.Find(x => x.Name.Equals(TextManager.DefaultTimeChannelName)).Data;
        public List<double> Speeds => Channels.Find(x => x.Name.Equals(TextManager.DefaultSpeedChannelName)).Data;
        public Channel GetChannel(string name) => Channels.Find(x => x.Name.Equals(name));
        public List<Point> TrackPoints { get; set; }
        public List<Lap> Laps { get; private set; }
        public double AverageLapLength => Distances.Average(x => x.DistanceSum);
        public int PointsSum { get; set; }
        public TimeSpan OneLapAverageTime
        {
            get
            {
                var times = new List<TimeSpan>();
                for (int i = 1; i < Laps.Count - 1; i++)
                {
                    times.Add(Laps[i].Time);
                }
                return new TimeSpan(Convert.ToInt64(times.Average(x => x.Ticks)));
            }
        }
        public TimeSpan BestLapTime
        {
            get
            {
                var bestTime = Laps[1].Time;
                for (int i = 2; i < Laps.Count - 1; i++)
                {
                    if (Laps[i].Time < bestTime)
                    {
                        bestTime = Laps[i].Time;
                    }
                }
                return bestTime;
            }
        }
        public TimeSpan WorstLapTime
        {
            get
            {
                var worstTime = Laps[1].Time;
                for (int i = 2; i < Laps.Count - 1; i++)
                {
                    if (Laps[i].Time > worstTime)
                    {
                        worstTime = Laps[i].Time;
                    }
                }
                return worstTime;
            }
        }

        /// <summary>
        /// Distances for all laps
        /// </summary>
        public List<OneLapDistance> Distances { get; private set; } = new List<OneLapDistance>();
        public List<double> AllDistances { get; private set; } = new List<double>();
        public double OneLapAverageDistance
        {
            get
            {
                double distance = 0;
                for (int i = 1; i < Distances.Count - 1; i++)
                {
                    distance += Distances[i].DistanceSum;
                }
                return distance / (double)(Distances.Count - 2);
            }
        }
        private void CalculateAllLapsSVG()
        {
            List<double> latitude = Latitude;
            List<double> longitude = Longitude;

            if (latitude.Count > 0 && longitude.Count > 0)
            {
                double minimumXValue = double.MaxValue;
                double minimumYValue = double.MaxValue;

                foreach (double latitudeValue in latitude)
                {
                    if (latitudeValue != 0 && !double.IsNaN(latitudeValue))
                    {
                        if (latitudeValue < minimumXValue)
                        {
                            minimumXValue = latitudeValue;
                        }
                    }
                }
                foreach (double longitudeValue in longitude)
                {
                    if (longitudeValue != 0 && !double.IsNaN(longitudeValue))
                    {
                        if (longitudeValue < minimumYValue)
                        {
                            minimumYValue = longitudeValue;
                        }
                    }
                }

                double scale = Math.Pow(10, 5);
                List<double> scaledLatitude = new List<double>();
                List<double> scaledLongitude = new List<double>();
                for (int i = 0; i < latitude.Count; i++)
                {
                    if (latitude[i] != 0)
                    {
                        if (!double.IsNaN(latitude[i]))
                        {
                            scaledLatitude.Add(Math.Round((latitude[i] - minimumXValue) * scale));
                        }
                        if (longitude[i] != 0)
                        {
                            if (!double.IsNaN(longitude[i]))
                            {
                                scaledLongitude.Add(Math.Round((longitude[i] - minimumYValue) * scale));
                            }
                        }
                    }
                }

                for (int i = 0; i < scaledLatitude.Count; i++)
                {
                    TrackPoints.Add(new Point(scaledLatitude[i], scaledLongitude[i]));
                }

                AllLapsSVG = string.Format("M{0} {1}", TrackPoints[0].X, TrackPoints[0].Y);
                for (int i = 0; i < TrackPoints.Count; i++)
                {
                    AllLapsSVG += string.Format(" L{0} {1}", TrackPoints[i].X, TrackPoints[i].Y);
                }
            }
        }

        /// <summary>
        /// Calculates <seealso cref="AllDistances"/> based on speed and time
        /// </summary>
        public void CalculateAllDistances()
        {
            AllDistances.Clear();
            List<double> speed = Speeds;
            List<double> time = Times;
            AllDistances.Add(0);
            for (int i = 1; i < time.Count; i++)
            {
                AllDistances.Add(AllDistances[i - 1] + Distance(time[i - 1], time[i], speed[i] / 3.6f));
            }

            CalculateDistances();
        }
        private double Distance(double time1, double time2, double speed) => speed * (time2 - time1);
        public void CalculateDistances()
        {
            Distances.Clear();
            for (int index = 0; index < Laps.Count; index++)
            {
                var distance = new OneLapDistance
                {
                    FromIndex = ChannelDataCount * Laps[index].FromIndex / PointsSum,
                    ToIndex = ChannelDataCount * Laps[index].ToIndex / PointsSum
                };

                distance.DistanceValues.Clear();

                for (int i = distance.FromIndex; i < distance.ToIndex; i++)
                {
                    distance.DistanceValues.Add(AllDistances[i]);
                }

                distance.DistanceSum = index > 0
                                     ? distance.DistanceValues.Last() - Distances[index - 1].DistanceValues.Last()
                                     : distance.DistanceValues.Last();

                Distances.Add(distance);
            }

            for (int index = 1; index < Distances.Count - 1; index++)
            {
                for (int i = 0; i < Distances[index].DistanceValues.Count; i++)
                {
                    Distances[index].DistanceValues[i] = Math.Abs(Distances[index].DistanceValues[i] - index * Distances[index].DistanceSum);
                }
            }


           /* for (int index = 0; index < Distances.Count; index++)
            {
                using var writer = new StreamWriter(string.Format("distance{0}.txt", index));

                for (int i = 0; i < Distances[index].DistanceValues.Count; i++)
                {
                    writer.WriteLine(Distances[index].DistanceValues[i]);
                }
            }*/

            /* using var writer = new StreamWriter("times.txt");
             foreach (var item in Times)
             {
                 writer.WriteLine(item);
             }*/

            /*  using var writer = new StreamWriter("alldistances.txt");
              foreach (var item in AllDistances)
              {
                  writer.WriteLine(item);
              }*/

            /*   using var writer = new StreamWriter("distances.txt");
               foreach (var item in Distances)
               {
                   writer.WriteLine(item.DistanceSum);
               }*/
        }
        public void CalculateLapTimes()
        {
            var times = Times;
            // int index = 0;
            foreach (Lap lap in Laps)
            {
                int fromIndex = ChannelDataCount * lap.FromIndex / PointsSum;
                int toIndex = ChannelDataCount * lap.ToIndex / PointsSum;

                lap.Time = TimeSpan.FromSeconds(times[toIndex] - times[fromIndex]);

                /* using var writer = new StreamWriter(string.Format("time{0}.txt", index++));

                 for (int i = fromIndex; i < toIndex; i++)
                 {
                     writer.WriteLine(times[i]);
                 }*/
            }
        }
        /// <summary>
        /// Item1 is the Lap
        /// Item2 is is the lap is disabled
        /// </summary>
        public List<Tuple<Lap, bool>> SelectedLaps { get; set; } = new List<Tuple<Lap, bool>>();
        /* public InputFile(string input_data_name, List<Data> datas, string driver_name)
         {
             FileName = input_data_name;
             AllData = datas;
             DriverName = driver_name;

             makeAllLapsSVG();
             //InitDistances();
         }

         /// <summary>
         /// Use this constructor, if you want to create an empty <seealso cref="InputFile"/>
         /// </summary>
         public InputFile()
         {
             FileName = string.Empty;
         }

         public List<string> LapsSVGs { get; set; } = new List<string>();
         public List<bool> ActiveLaps { get; set; } = new List<bool>();
         public List<Lap> Laps { get; } = new List<Lap>();
         public List<Data> AllData { get; } = new List<Data>();
         public List<Point> AverageLap { get; private set; } = new List<Point>();
         public List<Point> MapPoints { get; set; } = new List<Point>();

         /// <summary>
         /// Distances lap by lap
         /// </summary>
         public List<DistanceData> Distances { get; private set; } = new List<DistanceData>();
         public ChartValues<double> AllDistances { get; private set; } = new ChartValues<double>();
         public Track ActiveMap { get; set; } = null;
         public string DriverName { get; }

         /// <summary>
         /// If it's empty, that means that you created an empty <seealso cref="InputFile"/>
         /// </summary>
         public string FileName { get; }
         public string AllLapsSVG { get; private set; }
         public ushort ActualLapIndex { get; set; } = 0;

         public void CalculateLapTimes()
         {
             List<double> times = AllData.Find(n => n.Attribute.Equals("Time")).AllData.ToList();
             foreach (Lap lap in Laps)
             {
                 int from = times.Count * lap.FromIndex / Laps.Sum(a => a.Points.Count);
                 int to = times.Count * lap.ToIndex / Laps.Sum(a => a.Points.Count);

                 lap.Time = TimeSpan.FromSeconds(times[to] - times[from]);
             }
         }

         public void InitActiveLaps()
         {
             ActiveLaps.Clear();
             for (ushort i = 0; i < Laps.Count; i++)
             {
                 ActiveLaps.Add(false);
             }
         }

         private void makeAllLapsSVG()
         {
             List<double> latitude = Latitude;
             List<double> longitude = Longitude;

             if (latitude.Count > 0 && longitude.Count > 0)
             {
                 double minimum_x_value = double.MaxValue;
                 double minimum_y_value = double.MaxValue;
                 foreach (double latitude_value in latitude)
                 {
                     if (latitude_value != 0 && !double.IsNaN(latitude_value))
                     {
                         if (latitude_value < minimum_x_value)
                         {
                             minimum_x_value = latitude_value;
                         }
                     }
                 }
                 foreach (double longitude_value in longitude)
                 {
                     if (longitude_value != 0 && !double.IsNaN(longitude_value))
                     {
                         if (longitude_value < minimum_y_value)
                         {
                             minimum_y_value = longitude_value;
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
                             scaled_latitude.Add(Math.Round((latitude[i] - minimum_x_value) * scale));
                         }
                         if (longitude[i] != 0)
                         {
                             if (!double.IsNaN(longitude[i]))
                             {
                                 scaled_longitude.Add(Math.Round((longitude[i] - minimum_y_value) * scale));
                             }
                         }
                     }
                 }

                 for (int i = 0; i < scaled_latitude.Count; i++)
                 {
                     MapPoints.Add(new Point(scaled_latitude[i], scaled_longitude[i]));
                 }

                 AllLapsSVG = string.Format("M{0} {1}", MapPoints[0].X, MapPoints[0].Y);
                 for (int i = 0; i < MapPoints.Count; i++)
                 {
                     AllLapsSVG += string.Format(" L{0} {1}", MapPoints[i].X, MapPoints[i].Y);
                 }
             }
         }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="filter_percent"></param>
         /// <returns>3. laps sampled SVG</returns>
         public string OneLapSVG(float filter_percent = .34f)
         {
             List<Point> input_data = Laps[3].Points;
             ushort total = (ushort)input_data.Count;
             Random random = new Random(DateTime.Now.Millisecond);
             while (input_data.Count / (double)total > filter_percent)
             {
                 try
                 {
                     input_data.RemoveAt(random.Next(1, input_data.Count - 1));
                 }
                 catch (Exception) { }
             }

             string svg_path = string.Format("M{0} {1}", input_data[0].X, input_data[0].Y);
             for (ushort index = 0; index < input_data.Count; index++)
             {
                 svg_path += string.Format(" L{0} {1}", input_data[index].X, input_data[index].Y);
             }

             return svg_path;
         }

         public void MakeAvgLap()
         {
             int shortest_lap_length = 0;
             int index = 0;
             for (int i = 1; i < Laps.Count - 1; i++)
             {
                 if (Laps[i].Points.Count > shortest_lap_length)
                 {
                     shortest_lap_length = Laps[i].Points.Count;
                     index = i;
                 }
             }

             AverageLap = Laps[index].Points;
             for (int i = 0; i < shortest_lap_length; i++)
             {
                 try
                 {
                     double avg_x = 0;
                     double avg_y = 0;
                     for (int j = 1; j < Laps.Count - 1; j++)
                     {
                         avg_x += Laps[j].Points[i].X;
                         avg_y += Laps[j].Points[i].Y;
                     }
                     avg_x /= Laps.Count - 2;
                     avg_y /= Laps.Count - 2;
                     AverageLap[i] = new Point(avg_x, avg_y);
                 }
                 catch (Exception) { }
             }
         }
        */


        /*  /// <summary>
          /// Distances of laps
          /// </summary>
          public void CalculateDistances()
          {
              for (int index = 0; index < Laps.Count; index++)
              {
                  DistanceData distance = new DistanceData();
                  int fromIndex = AllData[0].AllData.Count * Laps[index].FromIndex / Laps.Sum(x => x.Points.Count);
                  int toIndex = AllData[0].AllData.Count * Laps[index].ToIndex / Laps.Sum(x => x.Points.Count);

                  distance.DistanceValues.Clear();

                  for (int i = fromIndex; i < toIndex; i++)
                  {
                      distance.DistanceValues.Add(AllDistances[i]);
                  }

                  if (index > 0)
                  {
                      distance.DistanceSum = distance.DistanceValues.Last() - Distances[index - 1].DistanceValues.Last();
                  }
                  else
                  {
                      distance.DistanceSum = distance.DistanceValues.Last();
                  }

                  Distances.Add(distance);
              }

              for (int index = 0; index < Distances.Count; index++)
              {
                  StreamWriter wsw = new StreamWriter(string.Format("distances{0}.txt", index));
                  for (int i = 0; i < Distances[index].DistanceValues.Count; i++)
                  {
                      Distances[index].DistanceValues[i] -= index * Distances[index].DistanceSum;
                      wsw.WriteLine(Distances[index].DistanceValues[i]);
                      /* try
                       {

                       Distances[index].DistanceValues[i] -= Distances[index - 1].DistanceValues[i];
                       }
                       catch (Exception)
                       {
                       }*/
        /*  }
          wsw.Close();
      }

      StreamWriter sw = new StreamWriter("distances.txt");
      foreach (var item in Distances)
      {
          sw.WriteLine(item.DistanceSum);
      }
      sw.Close();
    }

    private double distance(double time1, double time2, double speed) => speed * (time2 - time1);

    public ChartValues<double> Times => AllData.Find(n => n.Attribute.Equals("Time")).AllData;

    public List<double> Latitude => AllData.Find(n => n.Attribute.Equals("Latitude")).AllData.ToList();

    public List<double> Longitude => AllData.Find(n => n.Attribute.Equals("Longitude")).AllData.ToList();

    public Data GetData(string name) => AllData.Find(n => n.Attribute.Equals(name));




    public string AvgLapSVG
    {
      get
      {
          int radius = 10;
          string svg_path = "M " + AverageLap[0].X + " " + AverageLap[0].Y + " m -" + radius + ", 0 a " + radius + "," + radius + " 0 1,0 " + (radius * 2) + ",0 a " + radius + "," + radius + ", 0 1,0 -" + (radius * 2) + ",0";
          radius = 5;
          svg_path += "L " + AverageLap[0].X + " " + AverageLap[0].Y + " m -" + radius + ", 0 a " + radius + "," + radius + " 0 1,0 " + (radius * 2) + ",0 a " + radius + "," + radius + ", 0 1,0 -" + (radius * 2) + ",0";
          svg_path += string.Format(" L{0} {1}", AverageLap[0].X, AverageLap[0].Y);
          for (int i = 0; i < AverageLap.Count; i++)
          {
              svg_path += string.Format(" L{0} {1}", AverageLap[i].X, AverageLap[i].Y);
          }
          svg_path += " Z";
          return svg_path;
      }
    }



    /*  public ChartValues<ObservablePoint> GetChartValues(string attribute, int lap = 0)
    {
        return convertToObservablePoints(filteredData(GetLapValues(datas.Find(attr => attr.Attribute == attribute).Datas, lap)));
    }

    ChartValues<double> GetLapValues(ChartValues<double> values, int lap = 0)
    {
        int get_lap = lap == 0 ? act_lap : lap;

        ChartValues<double> datas = new ChartValues<double>();

        for (int i = laps[get_lap].Item2; i < laps[get_lap].Item3; i++)
        {
            datas.Add(values[i]);
        }

        return datas;
    }

    ChartValues<ObservablePoint> convertToObservablePoints(ChartValues<double> datas)
    {
        ChartValues<ObservablePoint> return_datas = new ChartValues<ObservablePoint>();

        for (int i = 0; i < datas.Count; i++)
        {
            return_datas.Add(new ObservablePoint
            {
                X = distances[i],
                Y = datas[i]
            });
        }

        return return_datas;
    }*/

        /*  ChartValues<double> filteredData(ChartValues<double> datas)
          {
              ChartValues<double> input_datas = new ChartValues<double>(datas);
              int total = input_datas.Count;
              Random rand = new Random(DateTime.Now.Millisecond);
              while (input_datas.Count / (double)total > filter_percent)
              {
                  try
                  {
                      input_datas.RemoveAt(rand.Next(1, input_datas.Count - 1));
                  }
                  catch (Exception)
                  {
                  }
              }

              return input_datas;
          }*/
    }
}
