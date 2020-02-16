using LiveCharts;
using LiveCharts.Defaults;
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

namespace ART_TELEMETRY_APP
{
    class ChartBuilder
    {
        BrushConverter brush_converter = new BrushConverter();
        List<Tuple<Label, string>> act_lap_labels = new List<Tuple<Label, string>>();

        public ChartBuilder(Grid diagram_grid)
        {
            act_lap_labels.Clear();
            diagram_grid.Children.Clear();
            diagram_grid.RowDefinitions.Clear();

            int index = 0;
            for (int i = 0; i < Groups.Instance.GroupsCount; i++)
            {
                CartesianChart chart = new CartesianChart();
                chart.DataTooltip = null;
                chart.Zoom = Groups.Instance.GetGroup().Zooming;
                chart.DisableAnimations = true;
                chart.Hoverable = false;

                RowDefinition row_up = new RowDefinition();
                RowDefinition row_down = new RowDefinition();
                row_down.Height = new GridLength(5);

                Grid grid = new Grid();

                foreach (string attribute in Groups.Instance.GetGroups[i].Attributes)
                {
                    LineSeries serie = new LineSeries();
                    serie.Title = Groups.Instance.GetGroups[i].Name;
                    serie.Values = Datas.Instance.GetData().GetChartValues(attribute);
                    serie.LineSmoothness = Datas.Instance.GetData().GetSingleData(attribute).Option.line_smoothness ? 1 : 0;
                    serie.PointGeometry = null;
                    serie.StrokeThickness = Datas.Instance.GetData().GetSingleData(attribute).Option.stroke_thickness;
                    serie.Fill = Brushes.Transparent;
                    serie.Stroke = Datas.Instance.GetData().GetSingleData(attribute).Option.stroke_color;
                    chart.Series.Add(serie);
                }

                Label label = new Label();
                label.Content = string.Format("{0}/{1}", Groups.Instance.GetGroup().ActLap, Datas.Instance.GetData().Laps.Count);
                label.Name = string.Format("label{0}", i.ToString());
                label.Margin = new Thickness(0, 10, 0, 0);
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                act_lap_labels.Add(new Tuple<Label, string>(label, Groups.Instance.GetGroups[i].Name));

                RowDefinition row_title = new RowDefinition();
                row_title.Height = new GridLength(45);
                RowDefinition row_chart = new RowDefinition();
                grid.RowDefinitions.Add(row_title);
                grid.RowDefinitions.Add(row_chart);

                Grid.SetRow(label, 0);
                Grid.SetRow(chart, 1);

                Button next_lap_btn = new Button();
                next_lap_btn.Height = double.NaN;
                PackIcon icon_next = new PackIcon();
                icon_next.Kind = PackIconKind.NavigateNext;
                next_lap_btn.Content = icon_next;
                next_lap_btn.Background = (Brush)brush_converter.ConvertFrom("#FF303030");
                next_lap_btn.BorderBrush = (Brush)brush_converter.ConvertFrom("#FF303030");
                next_lap_btn.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(next_btn_click);
                next_lap_btn.Name = string.Format("next_lap_btn{0}", i.ToString());

                Button prev_lap_btn = new Button();
                prev_lap_btn.Height = double.NaN;
                PackIcon icon_prev = new PackIcon();
                icon_prev.Kind = PackIconKind.NavigateBefore;
                prev_lap_btn.Content = icon_prev;
                prev_lap_btn.Background = (Brush)brush_converter.ConvertFrom("#FF303030");
                prev_lap_btn.BorderBrush = (Brush)brush_converter.ConvertFrom("#FF303030");
                prev_lap_btn.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(prev_btn_click);
                prev_lap_btn.Name = string.Format("prev_lap_btn{0}", i.ToString());

                ColumnDefinition col_1 = new ColumnDefinition();
                ColumnDefinition col_2 = new ColumnDefinition();
                ColumnDefinition col_3 = new ColumnDefinition();
                col_1.Width = new GridLength(43);
                col_3.Width = new GridLength(43);

                Grid.SetColumn(label, 1);
                Grid.SetColumn(prev_lap_btn, 0);
                Grid.SetColumn(next_lap_btn, 2);
                Grid.SetColumn(chart, 1);

                Grid.SetRowSpan(prev_lap_btn, 2);
                Grid.SetRowSpan(next_lap_btn, 2);

                grid.Children.Add(label);
                grid.Children.Add(chart);
                grid.Children.Add(prev_lap_btn);
                grid.Children.Add(next_lap_btn);

                grid.ColumnDefinitions.Add(col_1);
                grid.ColumnDefinitions.Add(col_2);
                grid.ColumnDefinitions.Add(col_3);

                Grid.SetRow(grid, index++);
                diagram_grid.Children.Add(grid);

                GridSplitter splitter = new GridSplitter();
                splitter.ResizeDirection = GridResizeDirection.Rows;
                splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                Grid.SetRow(splitter, index++);

                diagram_grid.Children.Add(splitter);

                diagram_grid.RowDefinitions.Add(row_up);
                diagram_grid.RowDefinitions.Add(row_down);
            }
        }

        private void prev_btn_click(object sender, MouseButtonEventArgs e)
        {
            Tuple<Label, string> label = act_lap_labels.Find(n => n.Item1.Name.Contains(((Button)sender).Name.Last()));
            Console.WriteLine(label.Item2);
            Groups.Instance.GetGroup(label.Item2).ActLap--;
            label.Item1.Content = Groups.Instance.GetGroup(label.Item2).ActLap;
        }

        private void next_btn_click(object sender, MouseButtonEventArgs e)
        {
            Tuple<Label, string> label = act_lap_labels.Find(n => n.Item1.Name.Contains(((Button)sender).Name.Last()));
            Console.WriteLine(label.Item2);
            Groups.Instance.GetGroup(label.Item2).ActLap++;
            label.Item1.Content = Groups.Instance.GetGroup(label.Item2).ActLap;
        }
    }
}
