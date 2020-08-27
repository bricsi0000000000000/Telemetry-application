using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings.Classes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ART_TELEMETRY_APP.Pilots.LapsContent;

namespace ART_TELEMETRY_APP
{
    public static class ChartBuilder
    {
        private static ushort longest_lap_length = 0;
        private static Lap longest_lap = new Lap();

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
            diagram_grid.Children.Clear();
            diagram_grid.RowDefinitions.Clear();

            #region Calculate the longest lap
            longest_lap_length = (ushort)input_file.Laps[1].Points.Count;
            longest_lap = input_file.Laps[1];
            for (ushort i = 2; i < input_file.Laps.Count - 1; i++)
            {
                if (input_file.Laps[i].Points.Count > longest_lap_length)
                {
                    longest_lap_length = (ushort)input_file.Laps[i].Points.Count;
                    longest_lap = input_file.Laps[i];
                }
            }
            #endregion

            List<string> labels_y_axis = new List<string>();
            List<string> labels_x_axis = new List<string>();
            for (ushort i = 0; i < longest_lap_length; i++)
            {
                labels_x_axis.Add(i.ToString());
            }

            int grid_row_index = 0;
            foreach (Data data in input_file.AllData)
            {
                if (selected_channels.Contains(data.Attribute))
                {
                    short max_value_x_axis = short.MinValue;
                    short min_value_x_axis = short.MaxValue;
                    short max_value_y_axis = short.MinValue;
                    short min_value_y_axis = short.MaxValue;

                    Chart chart = new Chart();

                    RowDefinition row_above = new RowDefinition();
                    RowDefinition row_down = new RowDefinition();
                    row_down.Height = new GridLength(5);

                    for (ushort lap_index = 0; lap_index < active_laps.Count; lap_index++)
                    {
                        float stroke_thickness = .7f;
                        ChartValues<ObservablePoint> serie_values = new ChartValues<ObservablePoint>();

                        switch (filter)
                        {
                            case Filter.kalman:
                                serie_values = convertToObservablePoints(kalmanFilter(ConvertLap(data, active_laps[lap_index], input_file, time), getKalmanSensitivity(lap_index, data.DriverName, group_name)));
                                LineSeries serie_kalman = new LineSeries
                                {
                                    Title = data.Attribute,
                                    Values = serie_values,
                                    PointGeometry = null,
                                    LineSmoothness = 0,
                                    StrokeThickness = stroke_thickness,
                                    Fill = Brushes.Transparent,
                                    Stroke = ChartLineColors.RandomColor
                                };
                                chart.AddSerie(serie_kalman);
                                break;
                            case Filter.nothing:
                                serie_values = convertToObservablePoints(ConvertLap(data, active_laps[lap_index], input_file, time));
                                LineSeries serie_nothing = new LineSeries
                                {
                                    Title = data.Attribute,
                                    Values = serie_values,
                                    PointGeometry = null,
                                    LineSmoothness = 0,
                                    StrokeThickness = stroke_thickness,
                                    Fill = Brushes.Transparent,
                                    Stroke = ChartLineColors.RandomColor
                                };
                                chart.AddSerie(serie_nothing);
                                break;
                            case Filter.both:
                                serie_values = convertToObservablePoints(kalmanFilter(ConvertLap(data, active_laps[lap_index], input_file, time), getKalmanSensitivity(lap_index, data.DriverName, group_name)));
                                LineSeries serie_kalman_both = new LineSeries
                                {
                                    Title = data.Attribute,
                                    Values = serie_values,
                                    PointGeometry = null,
                                    LineSmoothness = 0,
                                    StrokeThickness = stroke_thickness,
                                    Fill = Brushes.Transparent,
                                    Stroke = ChartLineColors.RandomColor
                                };
                                chart.AddSerie(serie_kalman_both);

                                serie_values = convertToObservablePoints(ConvertLap(data, active_laps[lap_index], input_file, time));
                                LineSeries serie_nothing_both = new LineSeries
                                {
                                    Title = data.Attribute,
                                    Values = serie_values,
                                    PointGeometry = null,
                                    LineSmoothness = 0,
                                    StrokeThickness = stroke_thickness,
                                    Fill = Brushes.Transparent,
                                    Stroke = ChartLineColors.RandomColor
                                };
                                chart.AddSerie(serie_nothing_both);
                                break;
                            default:
                                break;
                        }

                        #region Calculating minimum and maximum values on X and Y axes
                        for (short i = (short)(serie_values.Count - 1); i >= 0 && !double.IsNaN(serie_values[i].X); i--)
                        {
                            if (!double.IsNaN(serie_values[i].X))
                            {
                                min_value_y_axis = (short)serie_values[i].X;
                            }
                        }

                        ChartValues<double> lap_values = ConvertLap(data, active_laps[lap_index], input_file, time);

                        for (ushort i = 0; i < lap_values.Count; i++)
                        {
                            if (!double.IsNaN(lap_values[i]))
                            {
                                if (lap_values[i] > max_value_x_axis)
                                {
                                    max_value_x_axis = (short)lap_values[i];
                                }
                            }
                        }

                        for (ushort i = 0; i < lap_values.Count; i++)
                        {
                            if (!double.IsNaN(lap_values[i]))
                            {
                                if (lap_values[i] < min_value_x_axis)
                                {
                                    min_value_x_axis = (short)lap_values[i];
                                }
                            }
                        }

                        for (ushort i = 0; i < serie_values.Count && !double.IsNaN(serie_values[i].X); i++)
                        {
                            if (!double.IsNaN(serie_values[i].X))
                            {
                                max_value_y_axis = (short)serie_values[i].X;
                            }
                        }
                        #endregion

                        for (short i = min_value_x_axis; i < max_value_x_axis; i++)
                        {
                            labels_y_axis.Add(i.ToString());
                        }
                    }

                    chart.MaxValueX = max_value_x_axis;
                    chart.MinValueX = min_value_x_axis;
                    chart.MinValueY = min_value_y_axis;
                    chart.MaxValueY = max_value_y_axis;
                    chart.UpdateAxisValues();

                    if (selected_channels.Count > 0)
                    {
                        Axis axis_y = new Axis();
                        string title_axis_y = data.Attribute;

                        title_axis_y = title_axis_y.Substring(0, title_axis_y.Length - 2);
                        axis_y.Title = title_axis_y;

                        axis_y.Labels = labels_y_axis;

                        chart.AddAxisY(axis_y);

                        Axis axis_x = new Axis();
                        axis_x.Title = time ? "Time (s)" : "Distance (m)";

                        axis_x.Labels = labels_x_axis;

                        chart.AddAxisX(axis_x);

                        LiveCharts.Wpf.Separator x_axis_separator = new LiveCharts.Wpf.Separator();
                        x_axis_separator.Step = 10;
                        x_axis_separator.IsEnabled = false;

                        chart.AxisX.Separator = x_axis_separator;
                    }

                    Grid.SetRow(chart, grid_row_index++);
                    diagram_grid.Children.Add(chart);

                    GridSplitter splitter = new GridSplitter();
                    splitter.ResizeDirection = GridResizeDirection.Rows;
                    splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    Grid.SetRow(splitter, grid_row_index++);

                    diagram_grid.Children.Add(splitter);
                    diagram_grid.RowDefinitions.Add(row_above);
                    diagram_grid.RowDefinitions.Add(row_down);
                }
            }
            // SettingsManager.GGdiagram_UC.InitScatterPlot(group);
        }

        /// <summary>
        /// This list contains either distances or times. Later the X axis will be order based on this lists content.
        /// </summary>
        private static ChartValues<double> xAxisOrderByData = new ChartValues<double>();

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
            xAxisOrderByData = time ?
                        DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Times :
                        DriverManager.GetDriver(data.DriverName).GetInputFile(data.InputFileName).Distances;

            ushort from_index = (ushort)(data.AllData.Count * lap.FromIndex / input_file.Laps.Sum(a => a.Points.Count));
            ushort to_index = (ushort)(data.AllData.Count * lap.ToIndex / input_file.Laps.Sum(a => a.Points.Count));

            ushort longest_lap_from_index = (ushort)(data.AllData.Count * longest_lap.FromIndex / input_file.Laps.Sum(a => a.Points.Count));
            ushort longest_lap_to_index = (ushort)(data.AllData.Count * longest_lap.ToIndex / input_file.Laps.Sum(a => a.Points.Count));

            ChartValues<double> converted_lap_values = new ChartValues<double>();
            for (ushort i = from_index; i < to_index; i++)
            {
                converted_lap_values.Add(data.AllData[i]);
            }

            for (ushort i = (ushort)(to_index - from_index); i < (longest_lap_to_index - longest_lap_from_index); i++)
            {
                converted_lap_values.Add(double.NaN);
            }

            return converted_lap_values;
        }

        /// <summary>
        /// Converts a laps data to ObservablePoints based on <seealso cref="xAxisOrderByData"/>
        /// </summary>
        /// <param name="lap_data"></param>
        /// <returns></returns>
        private static ChartValues<ObservablePoint> convertToObservablePoints(ChartValues<double> lap_data)
        {
            ChartValues<ObservablePoint> return_datas = new ChartValues<ObservablePoint>();

            for (ushort i = 0; i < lap_data.Count; i++)
            {
                return_datas.Add(new ObservablePoint
                {
                    X = xAxisOrderByData[i],
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
        /// <returns>Filtered data</returns>
        private static ChartValues<double> kalmanFilter(ChartValues<double> data, double Q)
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
