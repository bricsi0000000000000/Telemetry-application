using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Experimental.xaml
    /// </summary>
    public partial class Experimental : UserControl
    {
        double[] xs;
        double[] ys;
        PlottableScatterHighlight sph;
        PlottableVLine a;

        public Experimental()
        {
            InitializeComponent();

          /*  var data = new List<double>();
            using var reader = new StreamReader("alldistances.txt");
            while (!reader.EndOfStream)
            {
                data.Add(double.Parse(reader.ReadLine()));
            }

            var data1 = new List<double>();
            using var reader1 = new StreamReader("times.txt");
            while (!reader1.EndOfStream)
            {
                data1.Add(double.Parse(reader1.ReadLine()));
            }

        
            //    chart.plt.PlotScatter(data2.ToArray(), data3.ToArray());

            chart.Render();*/

            /*  int pointCount = 100;
              Random rand = new Random(0);
              xs = DataGen.Consecutive(pointCount, 0.1);
              ys = DataGen.NoisySin(rand, pointCount);
              sph = chart.plt.PlotScatterHighlight(xs, ys);

              a = chart.plt.PlotVLine(0, lineStyle: LineStyle.Dash);*/


            // optional arguments customize highlighted point color, shape, and size

            // you can clear previously-highlighted points
            /* sph.HighlightPoint(4);
             sph.HighlightClear();*/

            // highlight the point nearest an X (or Y) position
            // chart.plt.PlotVLine(8.123, lineStyle: LineStyle.Dash);
            //sph.HighlightPointNearestX(8.123);

            // or highlight the point nearest another point in 2D space
            /* chart.plt.PlotPoint(4.43, 1.48);
             sph.HighlightPointNearest(4.43, 1.48);*/

        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            /* double mouseXPosition = chart.GetMouseCoordinates().x;
             sph.HighlightClear();
             chart.plt.Clear(a);
             sph.HighlightPointNearestX(mouseXPosition);
             a = chart.plt.PlotVLine(mouseXPosition, lineStyle: LineStyle.Dash);
             Console.WriteLine(sph.GetPointNearestX(mouseXPosition));
             chart.Render();*/
        }
    }
}
