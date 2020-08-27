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
        public bool Init = false;
        public GGDiagram_UC()
        {
            InitializeComponent();
            gg_chart.DataTooltip = null;
            gg_chart.DisableAnimations = true;
            gg_chart.Hoverable = false;
        }

        public void InitScatterPlot(InputFile input_file)
        {
            if (!Init)
            {
                Data accx_data = input_file.GetData("AccX");
                Data accy_data = input_file.GetData("AccY");
                Console.Write(accx_data.AllData.Count + " -> ");
                ChartValues<double> filtered_accx = filteredData(accx_data.AllData);
                ChartValues<double> filtered_accy = filteredData(accy_data.AllData);
              //  Console.WriteLine(filtered_accx.Count);
                ChartValues<ScatterPoint> acc = new ChartValues<ScatterPoint>();
                for (int i = 0; i < filtered_accx.Count; i++)
                {
                    acc.Add(new ScatterPoint(filtered_accx[i], filtered_accy[i]));
                }

                gg_chart.Series.Add(new ScatterSeries
                {
                    Values = acc,
                    MinPointShapeDiameter = 4,
                });


                Axis axis = new Axis();
                axis.Title = "AccX";
                gg_chart.AxisX.Add(axis);

                axis = new Axis();
                axis.Title = "AccY";
                gg_chart.AxisY.Add(axis);

                Init = true;
            }
        }

        ChartValues<double> filteredData(ChartValues<double> datas)
        {
            ChartValues<double> input_datas = new ChartValues<double>(datas);
            int total = input_datas.Count;
            Random rand = new Random(DateTime.Now.Millisecond);
            while (input_datas.Count / (double)total > .04f)
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
