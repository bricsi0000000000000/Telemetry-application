using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP
{
    class ChartBuilder
    {
        public ChartBuilder(Grid diagram_grid)
        {
            diagram_grid.Children.Clear();
            diagram_grid.RowDefinitions.Clear();

            int index = 0;
            for (int i = 0; i < Groups.Instance.GroupsCount; i++)
            {
                Groups.Instance.GetGroups[i].Chart.Series.Clear();

                RowDefinition row = new RowDefinition();

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
                    Groups.Instance.GetGroups[i].Chart.Series.Add(serie);
                }

                Grid.SetRow(Groups.Instance.GetGroups[i].Chart, index++);
                diagram_grid.Children.Add(Groups.Instance.GetGroups[i].Chart);

                //diagram_grid.Children.Add(Groups.Instance.GetGroup().Chart);

                diagram_grid.RowDefinitions.Add(row);
            }
        }
    }
}
