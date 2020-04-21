using ART_TELEMETRY_APP.InputFiles;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// Interaction logic for GGDiagram_UC.xaml
    /// </summary>
    public partial class GGDiagram_UC : UserControl
    {
        Group group;
        public GGDiagram_UC(Group group = null)
        {
            InitializeComponent();
            this.group = group;
        }

        public void InitScatterPlot(Group group)
        {
            this.group = group;
            if (group != null)
            {
                Data accx_data = group.Pilots[0].InputFiles[0].GetData("AccX");
                Data accy_data = group.Pilots[0].InputFiles[0].GetData("AccY");
                Console.Write(accx_data.Datas.Count + " -> ");
                ChartValues<double> filtered_accx = filteredData(accx_data.Datas);
                ChartValues<double> filtered_accy = filteredData(accy_data.Datas);
                Console.WriteLine(filtered_accx.Count);
                ChartValues<ObservablePoint> acc = new ChartValues<ObservablePoint>();
                for (int i = 0; i < filtered_accx.Count; i++)
                {
                    acc.Add(new ObservablePoint(filtered_accx[i], filtered_accy[i]));
                }

                ScatterSeries series = new ScatterSeries();
                series.Values = acc;
                gg_chart.Series.Add(series);
            }
        }

        ChartValues<double> filteredData(ChartValues<double> datas)
        {
            ChartValues<double> input_datas = new ChartValues<double>(datas);
            int total = input_datas.Count;
            Random rand = new Random(DateTime.Now.Millisecond);
            while (input_datas.Count / (double)total > .01f)
            {
                try
                {
                    input_datas.RemoveAt(rand.Next(1, input_datas.Count - 1));
                }
                catch (Exception)
                {
                }
            }

            return input_datas;
        }
    }
}
