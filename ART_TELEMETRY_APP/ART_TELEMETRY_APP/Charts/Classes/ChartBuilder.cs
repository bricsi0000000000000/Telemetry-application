using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts.Geared;

namespace ART_TELEMETRY_APP
{
    public static class ChartBuilder
    {
        public static void Build(ref Grid diagramGrid,
                                 List<Lap> activeLaps,
                                 List<string> selectedChannels,
                                 InputFile inputFile,
                                 bool time,
                                 Filter filter,
                                 string groupName
                                 )
        { }
        // private static Stopwatch stopWatch;

        /* private static Lap longestLap;

         private static OrderBy orderBy;

         /// <summary>
         /// Builds all charts on the selected Grid.
         /// </summary>
         /// <param name="diagramGrid">Grid that will contain all the charts.</param>
         /// <param name="activeLaps">Only these laps will be shown.</param>
         /// <param name="selectedChannels">Only these channels will be shown.</param>
         /// <param name="inputFile"></param>
         /// <param name="time">If this is set to true, the X axis will be order by time, otherwise by distance.</param>
         /// <param name="filter"></param>
         /// <param name="name_group"></param>
         public static void Build(ref Grid diagramGrid,
                                  List<Lap> activeLaps,
                                  List<string> selectedChannels,
                                  InputFile inputFile,
                                  bool time,
                                  Filter filter,
                                  string groupName
                                  )
         {
             if (selectedChannels.Count <= 0)
                 return;

             //  stopWatch = new Stopwatch();
             // stopWatch.Start();

             diagramGrid.Children.Clear();
             diagramGrid.RowDefinitions.Clear();

             longestLap = CalculateLongestLap(ref inputFile);

             ushort gridRowIndex = 0;
             foreach (Data data in inputFile.AllData)
             {
                 if (selectedChannels.Contains(data.Attribute))
                 {
                     var chart = new Chart();

                     for (ushort lapIndex = 0; lapIndex < activeLaps.Count; lapIndex++)
                     {
                         switch (filter)
                         {
                             case Filter.kalman:
                                 chart.AddSerie(CalculateSerieValues(true, data, ref activeLaps, lapIndex, ref inputFile, groupName, ref chart, time));
                                 break;
                             case Filter.nothing:
                                 chart.AddSerie(CalculateSerieValues(false, data, ref activeLaps, lapIndex, ref inputFile, groupName, ref chart, time));
                                 break;
                             case Filter.both:
                                 chart.AddSerie(CalculateSerieValues(true, data, ref activeLaps, lapIndex, ref inputFile, groupName, ref chart, time));
                                 chart.AddSerie(CalculateSerieValues(false, data, ref activeLaps, lapIndex, ref inputFile, groupName, ref chart, time));
                                 break;
                         }
                     }

                     chart.UpdateAxisValues();

                     chart.AddAxisY(YAxis(data, ref chart));
                     chart.AddAxisX(XAxis(time));

                     chart.AxisX.Separator = XAxisSeparator;

                     Grid.SetRow(chart, gridRowIndex++);
                     diagramGrid.Children.Add(chart);

                     Grid.SetRow(GridSplitter, gridRowIndex++);
                     diagramGrid.Children.Add(GridSplitter);

                     RowDefinition row_above = new RowDefinition();
                     RowDefinition row_down = new RowDefinition();
                     row_down.Height = new GridLength(5);
                     diagramGrid.RowDefinitions.Add(row_above);
                     diagramGrid.RowDefinitions.Add(row_down);
                 }
             }

             //stopWatch.Stop();
             // Console.WriteLine(stopWatch.ElapsedMilliseconds);
             // SettingsManager.GGdiagram_UC.InitScatterPlot(group);
         }

         private static GridSplitter GridSplitter => new GridSplitter
         {
             ResizeDirection = GridResizeDirection.Rows,
             HorizontalAlignment = HorizontalAlignment.Stretch
         };

         private static LiveCharts.Wpf.Separator XAxisSeparator => new LiveCharts.Wpf.Separator
         {
             Step = 10,
             IsEnabled = false
         };

         private static Axis XAxis(bool time) => new Axis
         {
             Title = time ? "Time (s)" : "Distance (m)",
             Labels = XAxisLabels
         };

         private static Axis YAxis(Data data, ref Chart chart)
         {
             string yAxisTitle = data.Attribute;
             yAxisTitle = yAxisTitle.Substring(0, yAxisTitle.Length - 2);

             Axis axis = new Axis
             {
                 Title = yAxisTitle,
                 Labels = YAxisLabels(ref chart)
             };

             return axis;
         }

         private static List<string> XAxisLabels => (from point in Enumerable.Range(0, longestLap.LapLength) select point.ToString()).ToList();
         private static List<string> YAxisLabels(ref Chart chart) => (from i in Enumerable.Range(chart.MinValueX, chart.MaxValueX) select i.ToString()).ToList();

         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private static LineSeries CalculateSerieValues(bool useFilter,
                                                        Data data,
                                                        ref List<Lap> activeLaps,
                                                        int lapIndex,
                                                        ref InputFile inputFile,
                                                        string groupName,
                                                        ref Chart chart,
                                                        bool time)
         {
             ChartValues<ObservablePoint> serieValues = useFilter ?
                                          ConvertLapToObservablePoints(FilteredData(ConvertLap(data, activeLaps[lapIndex], inputFile, time),
                                                                    GetKalmanSensitivity(lapIndex, data.DriverName, groupName)), activeLaps[lapIndex], data) :
                                          ConvertLapToObservablePoints(ConvertLap(data, activeLaps[lapIndex], inputFile, time), activeLaps[lapIndex], data);

             ChartValues<double> lapValues = ConvertLap(data, activeLaps[lapIndex], inputFile, time);

             CalculateChartMinimumYValue(ref chart, ref serieValues);
             CalculateChartMaximumYValue(ref chart, ref serieValues);
             CalculateChartMinimumXValue(ref chart, ref lapValues);
             CalculateChartMaximumXValue(ref chart, ref lapValues);

             return new LineSeries
             {
                 Title = data.Attribute,
                 Values = serieValues,
                 PointGeometry = null,
                 LineSmoothness = 0,
                 StrokeThickness = .8f,
                 Fill = Brushes.Transparent,
                 Stroke = ChartLineColors.GetColor
             };
         }

         private static void CalculateChartMinimumYValue(ref Chart chart, ref ChartValues<ObservablePoint> serieValues)
         {
             for (short index = (short)(serieValues.Count - 1); index >= 0 && !double.IsNaN(serieValues[index].X); index--)
             {
                 if (!double.IsNaN(serieValues[index].X))
                 {
                     chart.MinValueY = (short)serieValues[index].X;
                 }
             }
         }

         private static void CalculateChartMaximumYValue(ref Chart chart, ref ChartValues<ObservablePoint> serieValues)
         {
             for (ushort index = 0; index < serieValues.Count && !double.IsNaN(serieValues[index].X); index++)
             {
                 if (!double.IsNaN(serieValues[index].X))
                 {
                     chart.MaxValueY = (short)serieValues[index].X;
                 }
             }
         }

         private static void CalculateChartMaximumXValue(ref Chart chart, ref ChartValues<double> lapValues)
         {
             for (ushort index = 0; index < lapValues.Count; index++)
             {
                 if (!double.IsNaN(lapValues[index]))
                 {
                     if (lapValues[index] > chart.MaxValueX)
                     {
                         chart.MaxValueX = (short)lapValues[index];
                     }
                 }
             }
         }

         private static void CalculateChartMinimumXValue(ref Chart chart, ref ChartValues<double> lapValues)
         {
             for (ushort index = 0; index < lapValues.Count; index++)
             {
                 if (!double.IsNaN(lapValues[index]))
                 {
                     if (lapValues[index] < chart.MinValueX)
                     {
                         chart.MinValueX = (short)lapValues[index];
                     }
                 }
             }
         }

         private static Lap CalculateLongestLap(ref InputFile inputFile)
         {
             ushort longestLapLength = (ushort)inputFile.Laps.First().Points.Count;
             Lap lap = inputFile.Laps.First();
             for (ushort index = 2; index < inputFile.Laps.Count - 1; index++)
             {
                 if (inputFile.Laps[index].Points.Count > longestLapLength)
                 {
                     longestLapLength = (ushort)inputFile.Laps[index].Points.Count;
                     lap = inputFile.Laps[index];
                 }
             }

             return lap;
         }


         /// <summary>
         /// Gets one lap from all the laps.
         /// </summary>
         /// <param name="data"></param>
         /// <param name="lap">One of the active laps.</param>
         /// <param name="inputFile"></param>
         /// <param name="time">If this is set to true, the X axis will be order by time, otherwise by distance.</param>
         /// <returns>One lap</returns>
         private static ChartValues<double> ConvertLap(Data data, Lap lap, InputFile inputFile, bool time)
         {
             /*  xAxisOrderByData = time ?
                           DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Times :
                           DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Distances;*/

        /* orderBy.FromIndex = data.AllData.Count * lap.FromIndex / inputFile.Laps.Sum(x => x.Points.Count);
         orderBy.ToIndex = data.AllData.Count * lap.ToIndex / inputFile.Laps.Sum(x => x.Points.Count);
         //ushort longestLapFromIndex = (ushort)(data.AllData.Count * longestLap.FromIndex / inputFile.Laps.Sum(x => x.Points.Count));
         // ushort longestLapToIndex = (ushort)(data.AllData.Count * longestLap.ToIndex / inputFile.Laps.Sum(x => x.Points.Count));

         var allDistances = DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).AllDistances;
         var convertedLapValues = new ChartValues<double>();
         for (int i = orderBy.FromIndex; i < orderBy.ToIndex; i++)
         {
             convertedLapValues.Add(data.AllData[i]);
             //orderBy.Data.Add(allDistances[i]);
         }

         // System.Console.WriteLine(orderBy.Data.Last() + "\t" + orderBy.Data.Count);


         /* for (ushort i = (ushort)(to_index - from_index); i < (longest_lap_to_index - longest_lap_from_index); i++)
          {
              converted_lap_values.Add(double.NaN);
          }*/

        /* return convertedLapValues;
     }

     /// <summary>
     /// Converts a lap data to ObservablePoints based on <seealso cref="xAxisOrderByData"/>
     /// </summary>
     /// <param name="lapData"></param>
     /// <returns></returns>
     private static ChartValues<ObservablePoint> ConvertLapToObservablePoints(ChartValues<double> lapData, Lap lap, Data data)
     {
         var distances = DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Distances;

         orderBy.Data = distances[lap.Index].DistanceValues;

         var returnData = new ChartValues<ObservablePoint>();

         for (ushort i = 0; i < lapData.Count; i++)
         {
             returnData.Add(new ObservablePoint
             {
                 X = orderBy.Data[i],
                 Y = lapData[i]
             });
         }

       /*  StreamWriter sw = new StreamWriter(string.Format("actdist{0}.txt", lap.Index));
         foreach (var item in orderBy.Data)
         {
             sw.WriteLine(item);
         }
         sw.WriteLine(orderBy.Data.Sum());
         sw.Close();*/

        /* return returnData;
     }

     /// <summary>
     /// 
     /// </summary>
     /// <param name="data"></param>
     /// <param name="Q">Sensitivity of the filter. The smaller this value is, more "smoother" will be the data.</param>
     /// <returns></returns>
     private static ChartValues<double> FilteredData(ChartValues<double> data, double Q)
     {
         var filteredData = new ChartValues<double>();
         var filter = new KalmanFilter(1, 1, Q, 1, 0.1, data[0]);
         for (ushort i = 0; i < data.Count; i++)
         {
             filteredData.Add(filter.Output(data[i]));
         }

         return filteredData;
     }

     private static float GetKalmanSensitivity(int lapIndex, string driverName, string groupName)
                 => ((LapsContent)((DriverContentTab)((DiagramsMenu)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(driverName).Content).GetTab(groupName).Content).GetLapListElement(lapIndex + 1).KalmanSensitivity;
*/
      }
}
