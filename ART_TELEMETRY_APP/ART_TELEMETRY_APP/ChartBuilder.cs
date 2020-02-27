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

namespace ART_TELEMETRY_APP
{
    class ChartBuilder
    {
        #region instance
        private static ChartBuilder instance = null;
        private ChartBuilder() { }

        public static ChartBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChartBuilder();
                }
                return instance;
            }
        }
        #endregion
        public void Build(Grid diagram_grid, ColorZone diagram_nothing)
        {
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

                /*Axis axis = new Axis();
                axis.Title = Groups.Instance.GetGroups[i].Name;
                axis.Position = AxisPosition.LeftBottom;

                chart.AxisX.Add(axis);*/

                RowDefinition row_up = new RowDefinition();
                RowDefinition row_down = new RowDefinition();
                row_down.Height = new GridLength(5);

                foreach (string attribute in Groups.Instance.GetGroups[i].Attributes)
                {
                    LineSeries serie = new LineSeries
                    {
                        Title = Groups.Instance.GetGroups[i].Name,
                        Values = Datas.Instance.GetData().GetSingleData(attribute).DatasInLaps[Datas.Instance.GetData().ActLap - 1],
                        LineSmoothness = Datas.Instance.GetData().GetSingleData(attribute).Option.line_smoothness ? 1 : 0,
                        PointGeometry = null,
                        StrokeThickness = Datas.Instance.GetData().GetSingleData(attribute).Option.stroke_thickness,
                        Fill = Brushes.Transparent,
                        Stroke = Datas.Instance.GetData().GetSingleData(attribute).Option.stroke_color
                    };
                    chart.Series.Add(serie);
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

                diagram_nothing.Visibility = Visibility.Hidden;
            }
        }
    }
}
