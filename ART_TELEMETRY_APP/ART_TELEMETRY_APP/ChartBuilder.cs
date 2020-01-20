using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    class ChartBuilder : Window
    {
        // List<Tuple<string, List<Chart>>> groups = new List<Tuple<string, List<Chart>>>();
        List<CartesianChart> groups = new List<CartesianChart>();
        List<Chart> charts = new List<Chart>();

        public void AddGroup(string group_name)
        {
            if (getGroup(group_name) == null)
            {
                //groups.Add(new Tuple<string, List<Chart>>(group_title, new List<Chart>()));
                CartesianChart new_chart = new CartesianChart();
                new_chart.Name = group_name;
                groups.Add(new_chart);
            }
            else
            {
                //TODO: error message: group already exists
            }
        }

        public void AddChartToGroup(string group_name, string chart_title, ChartValues<float> datas)
        {
            CartesianChart group = getGroup(group_name);
            if (group != null)
            {
                if (!isChartExists(group_name, chart_title))
                {
                    //group.Item2.Add(new Chart(chart_title, datas));
                    charts.Add(new Chart(group_name, chart_title, datas));
                }
                else
                {
                    //TODO: error message: chart already exists
                }
            }
            else
            {
                //TODO: error message: group doesn't exists
            }
        }

        bool isChartExists(string group_name, string chart_title)
        {
            foreach (Chart chart in charts)
            {
                if (chart.GroupName == group_name)
                {
                    if (chart.Title == chart_title)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        CartesianChart getGroup(string group_name)
        {
            foreach (CartesianChart group in groups)
            {
                if (group.Name == group_name)
                {
                    return group;
                }
            }

            return null;
        }

        public void BuildGroup(string group_name, Grid grid)
        {
            grid.Children.Add(groups.Find(group => group.Name == group_name));
        }

        public void BuildChart(string group_name, string chart_name)
        {
            groups.Find(group => group.Name == group_name).Series = new SeriesCollection
            {
               new LineSeries
               {
                   Values = charts.Find(chart => chart.Title == chart_name).Datas
               }
            };
        }
    }
}
