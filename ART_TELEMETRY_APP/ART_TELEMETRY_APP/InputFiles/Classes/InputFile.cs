﻿using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.Classes;
using LiveCharts;
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
        public InputFile(string fileName, string driverName, List<Channel> channels)
        {
            FileName = fileName;
            DriverName = driverName;
            this.channels = channels;
            TrackPoints = new List<Point>();
            Laps = new List<Lap>();
            CalculateAllLapsSVG();
        }

        private readonly List<Channel> channels = new List<Channel>();

        private int ChannelDataCount => channels.First().ChannelData.Count;
        public string FileName { get; private set; }
        public string DriverName { get; private set; }
        public Track ActiveTrack { get; set; }
        public string AllLapsSVG { get; private set; }
        public List<double> Latitude => channels.Find(x => x.ChannelName.Equals("Latitude")).ChannelData.ToList();
        public List<double> Longitude => channels.Find(x => x.ChannelName.Equals("Longitude")).ChannelData.ToList();
        public ChartValues<double> Times => channels.Find(x => x.ChannelName.Equals("Time")).ChannelData;
        public ChartValues<double> Speeds => channels.Find(x => x.ChannelName.Equals("speed")).ChannelData;
        public Channel GetChannel(string name) => channels.Find(x => x.ChannelName.Equals(name));
        public List<Point> TrackPoints { get; set; }
        public List<Lap> Laps { get; private set; }


        /// <summary>
        /// Distances for all laps
        /// </summary>
        public List<OneLapDistance> Distances { get; private set; } = new List<OneLapDistance>();
        public ChartValues<double> AllDistances { get; private set; } = new ChartValues<double>();
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
            ChartValues<double> speed = Speeds;
            ChartValues<double> time = Times;
            AllDistances.Add(0);
            for (int i = 1; i < time.Count; i++)
            {
                AllDistances.Add(AllDistances[i - 1] + Distance(time[i - 1], time[i], speed[i] / 3.6));
            }

            CalculateDistances();
        }

        private double Distance(double time1, double time2, double speed) => speed * (time2 - time1);

        public void CalculateDistances()
        {
            for (int index = 0; index < Laps.Count; index++)
            {
                var distance = new OneLapDistance();
                int pointsSum = Laps.Sum(x => x.Points.Count);
                int fromIndex = ChannelDataCount * Laps[index].FromIndex / pointsSum;
                int toIndex = ChannelDataCount * Laps[index].ToIndex / pointsSum;

                distance.DistanceValues.Clear();

                for (int i = fromIndex; i < toIndex; i++)
                {
                    distance.DistanceValues.Add(AllDistances[i]);
                }

                distance.DistanceSum = index > 0 
                                     ? distance.DistanceValues.Last() - Distances[index - 1].DistanceValues.Last() 
                                     : distance.DistanceValues.Last();

                Distances.Add(distance);
            }

            for (int index = 0; index < Distances.Count; index++)
            {
                for (int i = 0; i < Distances[index].DistanceValues.Count; i++)
                {
                    Distances[index].DistanceValues[i] -= index * Distances[index].DistanceSum;
                }
            }

           /* StreamWriter sw = new StreamWriter("distances.txt");
            foreach (var item in Distances)
            {
                sw.WriteLine(item.DistanceSum);
            }
            sw.Close();*/
        }

        public void CalculateLapTimes()
        {
            var times = Times;
            foreach (Lap lap in Laps)
            {
                int from = times.Count * lap.FromIndex / Laps.Sum(a => a.Points.Count);
                int to = times.Count * lap.ToIndex / Laps.Sum(a => a.Points.Count);

                lap.Time = TimeSpan.FromSeconds(times[to] - times[from]);
            }
        }

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
