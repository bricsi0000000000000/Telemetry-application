using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Events;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static ART_TELEMETRY_APP.Pilots.LapsContent;

namespace ART_TELEMETRY_APP
{
    public static class ChartBuilder
    {
        static int longest_lap_length = 0;
        static Lap longest_lap = new Lap();
        static string group_name;
        static List<string> all_selected_channels;

        public static void Build(Grid diagram_grid, List<Lap> active_laps, List<string> selected_channels, InputFile input_file, bool time, Filter filter, string name_group)
        {
            all_selected_channels = selected_channels;
            group_name = name_group;
            diagram_grid.Children.Clear();
            diagram_grid.RowDefinitions.Clear();

            longest_lap_length = input_file.Laps[1].Points.Count;
            longest_lap = input_file.Laps[1];
            for (int i = 2; i < input_file.Laps.Count - 1; i++)
            {
                if (input_file.Laps[i].Points.Count > longest_lap_length)
                {
                    longest_lap_length = input_file.Laps[i].Points.Count;
                    longest_lap = input_file.Laps[i];
                }
            }

            List<string> labels_y = new List<string>();
            List<string> labels_x = new List<string>();
            for (int i = 0; i < longest_lap_length; i++)
            {
                labels_x.Add(i.ToString());
            }

            int index = 0;
            foreach (Data data in input_file.Datas)
            {
                if (all_selected_channels.Contains(data.Attribute))
                {
                    int max_value_x = int.MinValue;
                    int min_value_x = int.MaxValue;
                    int max_value_y = int.MinValue;
                    int min_value_y = int.MaxValue;

                    Chart chart = new Chart();

                    RowDefinition row_up = new RowDefinition();
                    RowDefinition row_down = new RowDefinition();
                    row_down.Height = new GridLength(5);

                    for (int lap_index = 0; lap_index < active_laps.Count; lap_index++)
                    {
                        float stroke_thickness = .7f;
                        ChartValues<ObservablePoint> serie_values = new ChartValues<ObservablePoint>();

                        switch (filter)
                        {
                            case Filter.kalman:
                                serie_values = convertToObservablePoints(kalmanFilter(ConvertLap(data, active_laps[lap_index], input_file, time), getKalmanSensitivity(lap_index, data.PilotsName)));
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
                                serie_values = convertToObservablePoints(kalmanFilter(ConvertLap(data, active_laps[lap_index], input_file, time), getKalmanSensitivity(lap_index, data.PilotsName)));
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

                        for (int i = serie_values.Count - 1; i >= 0 && !double.IsNaN(serie_values[i].X); i--)
                        {
                            if (!double.IsNaN(serie_values[i].X))
                            {
                                min_value_y = (int)serie_values[i].X;
                            }
                        }

                        ChartValues<double> values = ConvertLap(data, active_laps[lap_index], input_file, time);

                        for (int i = 0; i < values.Count; i++)
                        {
                            if (!double.IsNaN(values[i]))
                            {
                                if (values[i] > max_value_x)
                                {
                                    max_value_x = (int)values[i];
                                }
                            }
                        }

                        for (int i = 0; i < values.Count; i++)
                        {
                            if (!double.IsNaN(values[i]))
                            {
                                if (values[i] < min_value_x)
                                {
                                    min_value_x = (int)values[i];
                                }
                            }
                        }

                        for (int i = 0; i < serie_values.Count && !double.IsNaN(serie_values[i].X); i++)
                        {
                            if (!double.IsNaN(serie_values[i].X))
                            {
                                max_value_y = (int)serie_values[i].X;
                            }
                        }

                        for (int i = min_value_x; i < max_value_x; i++)
                        {
                            labels_y.Add(i.ToString());
                        }
                    }
                    chart.MaxValueX = max_value_x;
                    chart.MinValueX = min_value_x;
                    chart.MinValueY = min_value_y;
                    chart.MaxValueY = max_value_y;
                    chart.UpdateAxisValues();

                    if (all_selected_channels.Count > 0)
                    {
                        Axis axis_y = new Axis();
                        string title = data.Attribute;

                        title = title.Substring(0, title.Length - 2);
                        axis_y.Title = title;

                        axis_y.Labels = labels_y;

                        chart.AddAxisY(axis_y);

                        Axis axis_x = new Axis();
                        axis_x.Title = time ? "Time (s)" : "Distance (m)";

                        axis_x.Labels = labels_x;

                        chart.AddAxisX(axis_x);

                        LiveCharts.Wpf.Separator sep = new LiveCharts.Wpf.Separator();
                        sep.Step = 10;
                        sep.IsEnabled = false;

                        chart.AxisX.Separator = sep;
                    }

                    Grid.SetRow(chart, index++);
                    diagram_grid.Children.Add(chart);

                    GridSplitter splitter = new GridSplitter();
                    splitter.ResizeDirection = GridResizeDirection.Rows;
                    splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    Grid.SetRow(splitter, index++);

                    diagram_grid.Children.Add(splitter);
                    diagram_grid.RowDefinitions.Add(row_up);
                    diagram_grid.RowDefinitions.Add(row_down);
                }
            }

          /*  int index = 0;
            for (int lap_index = 0; lap_index < laps.Count; lap_index++)
            {
                foreach (Data data in input_file.Datas)
                {
                    if (all_selected_channels.Contains(data.Attribute))
                    {
                        int max_value_x = int.MinValue;
                        int min_value_x = int.MaxValue;
                        int max_value_y = int.MinValue;
                        int min_value_y = int.MaxValue;

                        Chart chart = new Chart();

                        RowDefinition row_up = new RowDefinition();
                        RowDefinition row_down = new RowDefinition();
                        row_down.Height = new GridLength(5);

                        float stroke_thickness = .7f;
                        ChartValues<ObservablePoint> serie_values = new ChartValues<ObservablePoint>();

                        switch (filter)
                        {
                            case Filter.kalman:
                                serie_values = convertToObservablePoints(kalmanFilter(ConvertLap(data, laps[lap_index], input_file, time), getKalmanSensitivity(lap_index, data.PilotsName)));
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
                                serie_values = convertToObservablePoints(ConvertLap(data, laps[lap_index], input_file, time));
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
                                serie_values = convertToObservablePoints(kalmanFilter(ConvertLap(data, laps[lap_index], input_file, time), getKalmanSensitivity(lap_index, data.PilotsName)));
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

                                serie_values = convertToObservablePoints(ConvertLap(data, laps[lap_index], input_file, time));
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

                        for (int i = serie_values.Count - 1; i >= 0 && !double.IsNaN(serie_values[i].X); i--)
                        {
                            if (!double.IsNaN(serie_values[i].X))
                            {
                                min_value_y = (int)serie_values[i].X;
                            }
                        }

                        ChartValues<double> values = ConvertLap(data, laps[lap_index], input_file, time);

                        for (int i = 0; i < values.Count; i++)
                        {
                            if (!double.IsNaN(values[i]))
                            {
                                if (values[i] > max_value_x)
                                {
                                    max_value_x = (int)values[i];
                                }
                            }
                        }

                        for (int i = 0; i < values.Count; i++)
                        {
                            if (!double.IsNaN(values[i]))
                            {
                                if (values[i] < min_value_x)
                                {
                                    min_value_x = (int)values[i];
                                }
                            }
                        }

                        for (int i = 0; i < serie_values.Count && !double.IsNaN(serie_values[i].X); i++)
                        {
                            if (!double.IsNaN(serie_values[i].X))
                            {
                                max_value_y = (int)serie_values[i].X;
                            }
                        }

                        for (int i = min_value_x; i < max_value_x; i++)
                        {
                            labels_y.Add(i.ToString());
                        }

                        chart.MaxValueX = max_value_x;
                        chart.MinValueX = min_value_x;
                        chart.MinValueY = min_value_y;
                        chart.MaxValueY = max_value_y;
                        chart.UpdateAxisValues();

                        if (all_selected_channels.Count > 0)
                        {
                            Axis axis_y = new Axis();
                            string title = "";
                            foreach (var item in all_selected_channels)
                            {
                                title += item + ", ";
                            }
                            title = title.Substring(0, title.Length - 2);
                            axis_y.Title = title;

                            axis_y.Labels = labels_y;

                            chart.AddAxisY(axis_y);

                            Axis axis_x = new Axis();
                            axis_x.Title = time ? "Time (s)" : "Distance (m)";

                            axis_x.Labels = labels_x;

                            chart.AddAxisX(axis_x);

                            LiveCharts.Wpf.Separator sep = new LiveCharts.Wpf.Separator();
                            sep.Step = 10;
                            sep.IsEnabled = false;

                            chart.AxisX.Separator = sep;
                        }

                        Grid.SetRow(chart, index++);
                        diagram_grid.Children.Add(chart);

                        GridSplitter splitter = new GridSplitter();
                        splitter.ResizeDirection = GridResizeDirection.Rows;
                        splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                        Grid.SetRow(splitter, index++);

                        diagram_grid.Children.Add(splitter);
                        diagram_grid.RowDefinitions.Add(row_up);
                        diagram_grid.RowDefinitions.Add(row_down);
                    }
                }
            }*/

            // SettingsManager.GGdiagram_UC.InitScatterPlot(group);
        }

        static ChartValues<double> distances = new ChartValues<double>();
        private static ChartValues<double> ConvertLap(Data data, Lap lap, InputFile input_file, bool time)
        {
            if (time)
            {
                distances = PilotManager.GetPilot(data.PilotsName).GetInputFile(data.InputFileName).Times;
            }
            else
            {
                distances = PilotManager.GetPilot(data.PilotsName).GetInputFile(data.InputFileName).Distances;
            }

            int from = (data.Datas.Count * lap.FromIndex) / input_file.Laps.Sum(a => a.Points.Count);
            int to = (data.Datas.Count * lap.ToIndex) / input_file.Laps.Sum(a => a.Points.Count);

            int longest_lap_from = (data.Datas.Count * longest_lap.FromIndex) / input_file.Laps.Sum(a => a.Points.Count);
            int longest_lap_to = (data.Datas.Count * longest_lap.ToIndex) / input_file.Laps.Sum(a => a.Points.Count);

            ChartValues<double> values = new ChartValues<double>();
            for (int i = from; i < to; i++)
            {
                values.Add(data.Datas[i]);
            }

            for (int i = (to - from); i < (longest_lap_to - longest_lap_from); i++)
            {
                values.Add(double.NaN);
            }

            return values;
        }

        private static ChartValues<ObservablePoint> convertToObservablePoints(ChartValues<double> datas)
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
        }

        private static ChartValues<double> kalmanFilter(ChartValues<double> datas, double Q)
        {
            ChartValues<double> clean = new ChartValues<double>();
            KalmanFilter filter = new KalmanFilter(1, 1, Q, 1, 0.1, datas[0]);
            for (int i = 0; i < datas.Count; i++)
            {
                clean.Add(filter.Output(datas[i]));
            }

            return clean;
        }

        private static float getKalmanSensitivity(int lap_index, string pilots_name)
        {
            return ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(group_name).Content).GetLapListElement(lap_index + 1).KalmanSensitivity;
        }
    }
}
