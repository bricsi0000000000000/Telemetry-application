using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Events;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void Build(Grid diagram_grid, List<Lap> laps, InputFile input_file, bool time, Filter filter)
        {
            diagram_grid.Children.Clear();
            diagram_grid.RowDefinitions.Clear();

            int index = 0;
            for (int lap_index = 0; lap_index < laps.Count; lap_index++)
            {
                double max_value_x = double.MinValue;
                double min_value_x = double.MaxValue;
                double max_value_y = double.MinValue;
                double min_value_y = double.MaxValue;

                Chart chart = new Chart();
                /*CartesianChart chart = new CartesianChart();
                chart.DataTooltip = null;
                chart.DisableAnimations = true;
                chart.Hoverable = false;
                chart.MinHeight = chart_min_height;
                chart.Zoom = ZoomingOptions.Xy;*/

                if (laps[lap_index].SelectedChannels.Count > 0)
                {
                    Axis axis = new Axis();
                    string title = "";
                    foreach (var item in laps[lap_index].SelectedChannels)
                    {
                        title += item + ", ";
                    }
                    title = title.Substring(0, title.Length - 2);
                    axis.Title = title;
                    /*axis.Separator = new LiveCharts.Wpf.Separator
                    {
                        IsEnabled = false,
                        Step = 100
                    };*/

                    chart.AddAxisY(axis);

                    axis = new Axis();
                    axis.Title = time ? "Time" : "Distance";
                    chart.AddAxisX(axis);

                    LiveCharts.Wpf.Separator sep = new LiveCharts.Wpf.Separator();
                    sep.Step = 50;

                    chart.AxisX.Separator = sep;
                }

                RowDefinition row_up = new RowDefinition();
                RowDefinition row_down = new RowDefinition();
                row_down.Height = new GridLength(5);

                foreach (Data data in input_file.Datas)
                {
                    if (laps[lap_index].SelectedChannels.Contains(data.Attribute))
                    {
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

                        chart.MaxValueY = serie_values.Last().X;

                        ChartValues<double> values = ConvertLap(data, laps[lap_index], input_file, time);
                      
                        if (values.Max() > max_value_x)
                        {
                            max_value_x = values.Max();
                        }
                        if (values.Min() < min_value_x)
                        {
                            min_value_x = values.Min();
                        }

                        if (values[0] < min_value_y)
                        {
                            min_value_y = values[0];
                        }
                    }
                }

                chart.MaxValueX = max_value_x;
                chart.MinValueX = min_value_x;
                chart.MinValueY = min_value_y;
                chart.UpdateAxisValues();

                Console.WriteLine("max_value_x: {0}\tmin_value_x: {1}\tmax_value_y: {2}\tmin_value_y: {3}", max_value_x, min_value_x, max_value_y, min_value_y);

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

            ChartValues<double> values = new ChartValues<double>();
            for (int i = from; i < to; i++)
            {
                values.Add(data.Datas[i]);
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
            return ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).GetLapListElement(lap_index + 1).KalmanSensitivity;
        }
    }
}
