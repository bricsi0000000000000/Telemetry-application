using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Datas.Classes;
using System;

namespace ART_TELEMETRY_APP
{
    public static class ChartBuilder
    {
        // private static Stopwatch stopWatch;

        private static Lap longestLap;

        private static OrderBy orderBy;

        public static void Build(Filter filter)
        {
            InitCharts();

            foreach (var lapsContent in ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).Tabs)
            {
                if (!(lapsContent.Content is LapsContent))
                    continue;

                var selectedChannels = ((LapsContent)lapsContent.Content).Group.Attributes;

                for (int i = 0; i < InputFileManager.InputFiles.Count; i++)
                {
                    if (!InputFileManager.InputFiles[i].IsSelected)
                        continue;

                    var selectedLaps = new List<Lap>();

                    foreach (var lap in InputFileManager.InputFiles[i].Laps)
                    {
                        if (ChartsSelectedData.SelectedLaps.Contains(lap.Index))
                        {
                            selectedLaps.Add(lap);
                        }
                    }

                    foreach (var channel in InputFileManager.InputFiles[i].Channels)
                    {
                        if (selectedChannels.Contains(channel.ChannelName))
                        {
                            var chart = ((LapsContent)lapsContent.Content).GetChart(((LapsContent)lapsContent.Content).Group.Name + channel.ChannelName);
                            ((LapsContent)lapsContent.Content).TrackSVG.Data = Geometry.Parse(InputFileManager.InputFiles[i].AllLapsSVG);
                            for (ushort lapIndex = 0; lapIndex < selectedLaps.Count; lapIndex++)
                            {
                                var data = CalculateSerieValues(true, channel, ref selectedLaps, lapIndex, InputFileManager.InputFiles[i]);
                                chart.InitPlot(data.Item1, data.Item2, selectedLaps[lapIndex].Index, InputFileManager.InputFiles[i].DriverName, InputFileManager.InputFiles[i].FileName, channel.ChannelName);

                                /* switch (filter)
                                 {
                                     case Filter.kalman:
                                         chart.chart.plt.PlotScatter();
                                         break;
                                     case Filter.nothing:
                                         chart.AddSerie(CalculateSerieValues(false, channel, ref selectedLaps, lapIndex, InputFileManager.InputFiles[i], ref chart));
                                         break;
                                     case Filter.both:
                                         chart.AddSerie(CalculateSerieValues(true, channel, ref selectedLaps, lapIndex, InputFileManager.InputFiles[i], ref chart));
                                         chart.AddSerie(CalculateSerieValues(false, channel, ref selectedLaps, lapIndex, InputFileManager.InputFiles[i], ref chart));
                                         break;
                                 }*/
                            }
                        }
                    }
                }
            }
        }

        private static void InitCharts()
        {
            foreach (var lapsContent in ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).Tabs)
            {
                if (!(lapsContent.Content is LapsContent))
                {
                    continue;
                }

                ((LapsContent)lapsContent.Content).ChartsGrid.Children.Clear();
                ((LapsContent)lapsContent.Content).ChartsGrid.RowDefinitions.Clear();
                ((LapsContent)lapsContent.Content).Charts.Clear();

                ushort gridRowIndex = 0;
                foreach (var attribute in ((LapsContent)lapsContent.Content).Group.Attributes)
                {
                    var chart = new Chart(((LapsContent)lapsContent.Content).Group.Name + attribute, attribute);

                    ChartManager.CursorChannelNames.Add(attribute);
                    ChartManager.CursorChannelData.Add(0);

                    Grid.SetRow(chart, gridRowIndex++);
                    ((LapsContent)lapsContent.Content).ChartsGrid.Children.Add(chart);

                    GridSplitter gridSplitter = new GridSplitter
                    {
                        ResizeDirection = GridResizeDirection.Rows,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch
                    };

                    Grid.SetRow(gridSplitter, gridRowIndex++);
                    ((LapsContent)lapsContent.Content).ChartsGrid.Children.Add(gridSplitter);

                    RowDefinition chartRow = new RowDefinition();
                    RowDefinition gridSplitterRow = new RowDefinition
                    {
                        Height = new GridLength(5)
                    };

                    ((LapsContent)lapsContent.Content).ChartsGrid.RowDefinitions.Add(chartRow);
                    ((LapsContent)lapsContent.Content).ChartsGrid.RowDefinitions.Add(gridSplitterRow);
                    ((LapsContent)lapsContent.Content).AddChart(ref chart, ((LapsContent)lapsContent.Content).Group.Name + attribute);
                }
            }

        }
        /*  private static LiveCharts.Wpf.Separator XAxisSeparator => new LiveCharts.Wpf.Separator
          {
              Step = 10,
              IsEnabled = false
          };*/

        /*private static Axis XAxis(bool time) => new Axis
         {
             Title = time ? "Time (s)" : "Distance (m)"
         };*/

        /*  private static Axis YAxis(string channelName, ref Chart chart)
          {
              Axis axis = new Axis
              {
                  Title = channelName,
                  Labels = YAxisLabels(ref chart)
              };

              return axis;
          }*/

        //private static List<string> XAxisLabels => (from point in Enumerable.Range(0, longestLap.Points.Count) select point.ToString()).ToList();
        //   private static List<string> YAxisLabels(ref Chart chart) => (from i in Enumerable.Range(chart.MinValueX, chart.MaxValueX) select i.ToString()).ToList();

        //   [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Tuple<double[], double[]> CalculateSerieValues(bool useFilter,
                                                       Channel channel,
                                                       ref List<Lap> activeLaps,
                                                       int lapIndex,
                                                       InputFile inputFile)
        {

            return ConvertLapToObservablePoints(ConvertLap(channel, activeLaps[lapIndex], inputFile), activeLaps[lapIndex], channel, inputFile);

            //  ChartValues<double> lapValues = ConvertLap(channel, activeLaps[lapIndex], inputFile);

            /*   CalculateChartMinimumYValue(ref chart, ref serieValues);
               CalculateChartMaximumYValue(ref chart, ref serieValues);
               CalculateChartMinimumXValue(ref chart, ref lapValues);
               CalculateChartMaximumXValue(ref chart, ref lapValues);*/

            //   return seri

            /*return new LineSeries
            {
                Title = channel.ChannelName,
                Values = serieValues.WithQuality(Quality.High),
                PointGeometry = null,
                StrokeThickness = .8f,
                Fill = Brushes.Transparent
            };*/
        }

        /*   private static void CalculateChartMinimumYValue(ref Chart chart, ref ChartValues<ObservablePoint> serieValues)
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

           private static Lap CalculateLongestLap(InputFile inputFile)
           {
               ushort longestLapLength = (ushort)inputFile.Laps.First().Points.Count;
               var lap = inputFile.Laps.First();
               for (ushort index = 2; index < inputFile.Laps.Count - 1; index++)
               {
                   if (inputFile.Laps[index].Points.Count > longestLapLength)
                   {
                       longestLapLength = (ushort)inputFile.Laps[index].Points.Count;
                       lap = inputFile.Laps[index];
                   }
               }

               return lap;
           }*/


        private static double[] ConvertLap(Channel channelData, Lap lap, InputFile inputFile)
        {
            /*  xAxisOrderByData = time ?
                          DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Times :
                          DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Distances;*/

            orderBy.FromIndex = channelData.ChannelData.Count * lap.FromIndex / inputFile.Laps.Sum(x => x.Points.Count);
            orderBy.ToIndex = channelData.ChannelData.Count * lap.ToIndex / inputFile.Laps.Sum(x => x.Points.Count);
            //ushort longestLapFromIndex = (ushort)(data.AllData.Count * longestLap.FromIndex / inputFile.Laps.Sum(x => x.Points.Count));
            // ushort longestLapToIndex = (ushort)(data.AllData.Count * longestLap.ToIndex / inputFile.Laps.Sum(x => x.Points.Count));

            var convertedLapValues = new List<double>();
            for (int i = inputFile.Distances[lap.Index].FromIndex; i < inputFile.Distances[lap.Index].ToIndex; i++)
            {
                convertedLapValues.Add(channelData.ChannelData[i]);
                //orderBy.Data.Add(allDistances[i]);
            }

            // System.Console.WriteLine(orderBy.Data.Last() + "\t" + orderBy.Data.Count);


            /* for (ushort i = (ushort)(to_index - from_index); i < (longest_lap_to_index - longest_lap_from_index); i++)
             {
                 converted_lap_values.Add(double.NaN);
             }*/

            return convertedLapValues.ToArray();
        }

        /// <summary>
        /// Converts a lap data to ObservablePoints based on <seealso cref="xAxisOrderByData"/>
        /// </summary>
        /// <param name="lapData"></param>
        /// <returns></returns>
        private static Tuple<double[], double[]> ConvertLapToObservablePoints(double[] lapData, Lap lap, Channel channelData, InputFile inputFile)
        {
            var orderByData = new List<double>();
            for (int i = 0; i < inputFile.Distances[lap.Index].DistanceSum; i++)
            {
                orderByData.Add(i);
            }
            orderBy.Data = inputFile.Distances[lap.Index].DistanceValues;

            List<double> x = new List<double>();
            List<double> y = new List<double>();

            for (ushort i = 0; i < lapData.Length; i++)
            {
                x.Add(orderBy.Data[i]);
                y.Add(lapData[i]);
            }

            /* for (ushort i = 0; i < lapData.Length; i++)
             {
                 returnData.Add(new ObservablePoint
                 {
                     X = orderBy.Data[i],
                     Y = lapData[i]
                 });
             }*/

            /*  StreamWriter sw = new StreamWriter(string.Format("actdist{0}.txt", lap.Index));
              foreach (var item in orderBy.Data)
              {
                  sw.WriteLine(item);
              }
              sw.WriteLine(orderBy.Data.Sum());
              sw.Close();*/

            return new Tuple<double[], double[]>(x.ToArray(), y.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Q">Sensitivity of the filter. The smaller this value is, more "smoother" will be the data.</param>
        /// <returns></returns>
      /*  private static ChartValues<double> FilteredData(ChartValues<double> data, double Q)
        {
            var filteredData = new ChartValues<double>();
            var filter = new KalmanFilter(1, 1, Q, 1, 0.1, data[0]);
            for (ushort i = 0; i < data.Count; i++)
            {
                filteredData.Add(filter.Output(data[i]));
            }

            return filteredData;
        }*/

        /* private static float GetKalmanSensitivity(int lapIndex, string driverName, string groupName)
                     => ((LapsContent)((DriverContentTab)((DiagramsMenu)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(driverName).Content).GetTab(groupName).Content).GetLapListElement(lapIndex + 1).KalmanSensitivity;
         */
    }
}
