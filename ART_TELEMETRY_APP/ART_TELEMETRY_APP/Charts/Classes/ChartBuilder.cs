using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings.Classes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ART_TELEMETRY_APP.Pilots.LapsContent;

namespace ART_TELEMETRY_APP
{
    public static class ChartBuilder
    {
        // private static Stopwatch stopWatch;

        private static Lap longest_lap;

        /// <summary>
        /// This list contains either distances or times. Later the X axis will be order based on this lists content.
        /// </summary>
        private static ChartValues<double> xAxisOrderByData = new ChartValues<double>();
        private static ushort xAxisOrderByFromIndex = 0;
        private static ushort xAxisOrderByToIndex = 0;

        /// <summary>
        /// Builds all charts on the selected Grid.
        /// </summary>
        /// <param name="diagram_grid">Grid that will contain all the charts.</param>
        /// <param name="active_laps">Only these laps will be shown.</param>
        /// <param name="selected_channels">Only these channels will be shown.</param>
        /// <param name="input_file"></param>
        /// <param name="time">If this is set to true, the X axis will be order by time, otherwise by distance.</param>
        /// <param name="filter"></param>
        /// <param name="name_group"></param>
        public static void Build(ref Grid diagram_grid,
                                 List<Lap> active_laps,
                                 List<string> selected_channels,
                                 InputFile input_file,
                                 bool time,
                                 Filter filter,
                                 string group_name
                                 )
        {
            if (selected_channels.Count <= 0) return;

            //  stopWatch = new Stopwatch();
            // stopWatch.Start();

            diagram_grid.Children.Clear();
            diagram_grid.RowDefinitions.Clear();

            longest_lap = calculateLongestLap(ref input_file);

            ushort grid_row_index = 0;
            foreach (Data data in input_file.AllData)
            {
                if (selected_channels.Contains(data.Attribute))
                {
                    Chart chart = new Chart();

                    for (ushort lap_index = 0; lap_index < active_laps.Count; lap_index++)
                    {
                        switch (filter)
                        {
                            case Filter.kalman:
                                chart.AddSerie(calculateSerieValues(true, data, ref active_laps, lap_index, ref input_file, group_name, ref chart, time));
                                break;
                            case Filter.nothing:
                                chart.AddSerie(calculateSerieValues(false, data, ref active_laps, lap_index, ref input_file, group_name, ref chart, time));
                                break;
                            case Filter.both:
                                chart.AddSerie(calculateSerieValues(true, data, ref active_laps, lap_index, ref input_file, group_name, ref chart, time));
                                chart.AddSerie(calculateSerieValues(false, data, ref active_laps, lap_index, ref input_file, group_name, ref chart, time));
                                break;
                        }
                    }

                    chart.UpdateAxisValues();

                    chart.AddAxisY(axisY(data, ref chart));
                    chart.AddAxisX(axisX(time));

                    chart.AxisX.Separator = xAxisSeparator;

                    Grid.SetRow(chart, grid_row_index++);
                    diagram_grid.Children.Add(chart);

                    Grid.SetRow(gridSplitter, grid_row_index++);
                    diagram_grid.Children.Add(gridSplitter);

                    RowDefinition row_above = new RowDefinition();
                    RowDefinition row_down = new RowDefinition();
                    row_down.Height = new GridLength(5);
                    diagram_grid.RowDefinitions.Add(row_above);
                    diagram_grid.RowDefinitions.Add(row_down);
                }
            }

            //stopWatch.Stop();
            // Console.WriteLine(stopWatch.ElapsedMilliseconds);
            // SettingsManager.GGdiagram_UC.InitScatterPlot(group);
        }

        private static GridSplitter gridSplitter
        {
            get
            {
                GridSplitter splitter = new GridSplitter();
                splitter.ResizeDirection = GridResizeDirection.Rows;
                splitter.HorizontalAlignment = HorizontalAlignment.Stretch;

                return splitter;
            }
        }

        private static LiveCharts.Wpf.Separator xAxisSeparator
        {
            get
            {
                LiveCharts.Wpf.Separator separator = new LiveCharts.Wpf.Separator();
                separator.Step = 10;
                separator.IsEnabled = false;

                return separator;
            }
        }

        private static Axis axisX(bool time)
        {
            Axis axis = new Axis();
            axis.Title = time ? "Time (s)" : "Distance (m)";
            axis.Labels = xAxisLabels;

            return axis;
        }

        private static Axis axisY(Data data, ref Chart chart)
        {
            string title_axis_y = data.Attribute;
            title_axis_y = title_axis_y.Substring(0, title_axis_y.Length - 2);

            Axis axis = new Axis();
            axis.Title = title_axis_y;
            axis.Labels = yAxisLabels(ref chart);

            return axis;
        }

        private static List<string> xAxisLabels => (from i in Enumerable.Range(0, longest_lap.LapLength) select i.ToString()).ToList();
        private static List<string> yAxisLabels(ref Chart chart) => (from i in Enumerable.Range(chart.MinValueX, chart.MaxValueX) select i.ToString()).ToList();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static LineSeries calculateSerieValues(bool use_filter,
                                                           Data data,
                                                           ref List<Lap> active_laps,
                                                           int lap_index,
                                                           ref InputFile input_file,
                                                           string group_name,
                                                           ref Chart chart,
                                                           bool time)
        {
            ChartValues<ObservablePoint> serie_values = use_filter ?
                                         convertToObservablePoints(filteredData(ConvertLap(data, active_laps[lap_index], input_file, time),
                                                                   getKalmanSensitivity(lap_index, data.DriverName, group_name)), active_laps[lap_index], data) :
                                         convertToObservablePoints(ConvertLap(data, active_laps[lap_index], input_file, time), active_laps[lap_index], data);

            ChartValues<double> lap_values = ConvertLap(data, active_laps[lap_index], input_file, time);

            calculateChartMinimumYValue(ref chart, ref serie_values);
            calculateChartMaximumYValue(ref chart, ref serie_values);
            calculateChartMinimumXValue(ref chart, ref lap_values);
            calculateChartMaximumXValue(ref chart, ref lap_values);

            return new LineSeries
            {
                Title = data.Attribute,
                Values = serie_values,
                PointGeometry = null,
                LineSmoothness = 0,
                StrokeThickness = .7f,
                Fill = Brushes.Transparent,
                Stroke = ChartLineColors.RandomColor
            };
        }

        private static void calculateChartMinimumYValue(ref Chart chart, ref ChartValues<ObservablePoint> serie_values)
        {
            for (short index = (short)(serie_values.Count - 1); index >= 0 && !double.IsNaN(serie_values[index].X); index--)
            {
                if (!double.IsNaN(serie_values[index].X))
                {
                    chart.MinValueY = (short)serie_values[index].X;
                }
            }
        }

        private static void calculateChartMaximumYValue(ref Chart chart, ref ChartValues<ObservablePoint> serie_values)
        {
            for (ushort index = 0; index < serie_values.Count && !double.IsNaN(serie_values[index].X); index++)
            {
                if (!double.IsNaN(serie_values[index].X))
                {
                    chart.MaxValueY = (short)serie_values[index].X;
                }
            }
        }

        private static void calculateChartMaximumXValue(ref Chart chart, ref ChartValues<double> lap_values)
        {
            for (ushort index = 0; index < lap_values.Count; index++)
            {
                if (!double.IsNaN(lap_values[index]))
                {
                    if (lap_values[index] > chart.MaxValueX)
                    {
                        chart.MaxValueX = (short)lap_values[index];
                    }
                }
            }
        }

        private static void calculateChartMinimumXValue(ref Chart chart, ref ChartValues<double> lap_values)
        {
            for (ushort index = 0; index < lap_values.Count; index++)
            {
                if (!double.IsNaN(lap_values[index]))
                {
                    if (lap_values[index] < chart.MinValueX)
                    {
                        chart.MinValueX = (short)lap_values[index];
                    }
                }
            }
        }

        private static Lap calculateLongestLap(ref InputFile input_file)
        {
            ushort longest_lap_length = (ushort)input_file.Laps.First().Points.Count;
            Lap lap = input_file.Laps.First();
            for (ushort index = 2; index < input_file.Laps.Count - 1; index++)
            {
                if (input_file.Laps[index].Points.Count > longest_lap_length)
                {
                    longest_lap_length = (ushort)input_file.Laps[index].Points.Count;
                    lap = input_file.Laps[index];
                }
            }

            return lap;
        }


        /// <summary>
        /// Gets one lap from all the laps.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lap">One of the active laps.</param>
        /// <param name="input_file"></param>
        /// <param name="time">If this is set to true, the X axis will be order by time, otherwise by distance.</param>
        /// <returns>One lap</returns>
        private static ChartValues<double> ConvertLap(Data data, Lap lap, InputFile input_file, bool time)
        {
            /*  xAxisOrderByData = time ?
                          DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Times :
                          DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Distances;*/

            ushort from_index = (ushort)(data.AllData.Count * lap.FromIndex / input_file.Laps.Sum(a => a.Points.Count));
            ushort to_index = (ushort)(data.AllData.Count * lap.ToIndex / input_file.Laps.Sum(a => a.Points.Count));

            xAxisOrderByFromIndex = from_index;
            xAxisOrderByToIndex = to_index;

            ushort longest_lap_from_index = (ushort)(data.AllData.Count * longest_lap.FromIndex / input_file.Laps.Sum(a => a.Points.Count));
            ushort longest_lap_to_index = (ushort)(data.AllData.Count * longest_lap.ToIndex / input_file.Laps.Sum(a => a.Points.Count));

            ChartValues<double> converted_lap_values = new ChartValues<double>();
            for (ushort i = from_index; i < to_index; i++)
            {
                converted_lap_values.Add(data.AllData[i]);
            }

           /* for (ushort i = (ushort)(to_index - from_index); i < (longest_lap_to_index - longest_lap_from_index); i++)
            {
                converted_lap_values.Add(double.NaN);
            }*/

            return converted_lap_values;
        }

        /// <summary>
        /// Converts a laps data to ObservablePoints based on <seealso cref="xAxisOrderByData"/>
        /// </summary>
        /// <param name="lap_data"></param>
        /// <returns></returns>
        private static ChartValues<ObservablePoint> convertToObservablePoints(ChartValues<double> lap_data, Lap lap, Data data)
        {
            List<ChartValues<double>> a = DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Distances;
            ChartValues<ObservablePoint> return_datas = new ChartValues<ObservablePoint>();

            for (ushort i = 0; i < lap_data.Count; i++)
            {
                return_datas.Add(new ObservablePoint
                {
                    X = a[lap.Index][i],
                    Y = lap_data[i]
                });
            }

            return return_datas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Q">Sensitivity of the filter. The smaller this value is, more "smoother" will be the data.</param>
        /// <returns></returns>
        private static ChartValues<double> filteredData(ChartValues<double> data, double Q)
        {
            ChartValues<double> filtered_data = new ChartValues<double>();
            KalmanFilter filter = new KalmanFilter(1, 1, Q, 1, 0.1, data[0]);
            for (ushort i = 0; i < data.Count; i++)
            {
                filtered_data.Add(filter.Output(data[i]));
            }

            return filtered_data;
        }

        private static float getKalmanSensitivity(int lap_index, string driver_name, string group_name)
                    => ((LapsContent)((DriverContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(driver_name).Content).GetTab(group_name).Content).GetLapListElement(lap_index + 1).KalmanSensitivity;
    }
}
