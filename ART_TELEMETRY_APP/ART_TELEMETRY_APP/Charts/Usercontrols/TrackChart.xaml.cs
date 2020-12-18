using ScottPlot;
using ScottPlot.Drawing;
using System.Drawing;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Charts.Usercontrols
{
    /// <summary>
    /// Interaction logic for TrackChart.xaml
    /// </summary>
    public partial class TrackChart : UserControl
    {
        private PlottableScatterHighlight plottableScatterHighlight;

        /// <summary>
        /// Car image on track.
        /// </summary>
        private readonly Bitmap carImage;

        public TrackChart(int chartHeight = 300)
        {
            InitializeComponent();

            ScottPlotChart.Height = chartHeight;
            carImage = new Bitmap("car_top_game.png");
        }

        /// <summary>
        /// Creates a <see cref="Tracks.Classes.DriverlessTrack"/>-s plot.
        /// </summary>
        /// <param name="xAxisValues">Values on <b>horizontal</b> axis.</param>
        /// <param name="yAxisValues">Values on <b>vertical</b> axis.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="xValue">Value on the horizontal axis.</param>
        /// <param name="yValue">Value on the vertical axis.</param>
        /// <param name="yAxisLabel">Label on <b>vertical</b> axis. Default is an empty string.</param>
        /// <param name="xAxisLabel">Label on <b>horizontal</b> axis. Default is an empty string.</param>
        /// <param name="lineWidth">Width of the line. Default is <c>3</c>.</param>
        /// <param name="lineStyle">Style of the line. Default is <see cref="LineStyle.Solid"/>.</param>
        /// <param name="enableLabel">If true, the label is enabled on the line.</param>
        public void InitPlot(double[] xAxisValues,
                             double[] yAxisValues,
                             Color color,
                             double xValue = 0,
                             double yValue = 0,
                             string yAxisLabel = "",
                             string xAxisLabel = "x",
                             double lineWidth = 3,
                             LineStyle lineStyle = LineStyle.Solid,
                             bool enableLabel = false)
        {
            if (enableLabel)
            {
                plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues,
                                                                                yAxisValues,
                                                                                markerShape: MarkerShape.none,
                                                                                color: color,
                                                                                lineWidth: lineWidth,
                                                                                lineStyle: lineStyle,
                                                                                label: $"{xAxisLabel}: {xValue:f3}\n{yAxisLabel}: {yValue:f3}");
            }
            else
            {
                plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues,
                                                                                yAxisValues,
                                                                                markerShape: MarkerShape.none,
                                                                                color: color,
                                                                                lineWidth: lineWidth,
                                                                                lineStyle: lineStyle);
            }
            

            ScottPlotChart.plt.Frame(false);
            ScottPlotChart.plt.Style(ScottPlot.Style.Gray1);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
            ScottPlotChart.plt.YLabel(yAxisLabel);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();
        }


        /// <summary>
        /// Plot a car image and rotates based on the <paramref name="rotation"/>.
        /// </summary>
        /// <param name="xValue">Images <b>x</b> value.</param>
        /// <param name="yValue">Images <b>y</b> value.</param>
        /// <param name="rotation">Images rotation.</param>
        public void PlotImage(double xValue, double yValue, double rotation)
        {
            ScottPlotChart.plt.PlotBitmap(carImage, xValue - .21f, yValue + 1.5f, rotation: rotation);
        }

        /// <summary>
        /// Sets the axis limits to automatic.
        /// </summary>
        public void SetAxisLimitsToAuto()
        {
            ScottPlotChart.plt.AxisAuto();
        }

        /// <summary>
        /// Sets the frame border visible or not.
        /// </summary>
        /// <param name="left">Left border.</param>
        /// <param name="bottom">Bottom border.</param>
        /// <param name="top">Top border.</param>
        /// <param name="right">Right</param>
        public void SetFrameBorder(bool left = true, bool bottom = true, bool top = true, bool right = true)
        {
            ScottPlotChart.plt.Frame(left: left, bottom: bottom, top: top, right: right);
        }
    }
}
