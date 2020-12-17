using ScottPlot;
using ScottPlot.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Charts.Usercontrols
{
    /// <summary>
    /// Represents a chart based on <see cref="ScottPlot"/>.
    /// </summary>
    public partial class Chart : UserControl
    {
        private PlottableScatterHighlight plottableScatterHighlight;
        private PlottableVLine plottableVLine;

        /// <summary>
        /// Name of the <see cref="Chart"/>.
        /// </summary>
        public string ChartName { get; set; }

        /// <summary>
        /// Name of the channel whose data is represented.
        /// </summary>
        private readonly string channelName;

        /// <summary>
        /// Car image on track.
        /// </summary>
        private readonly Bitmap carImage;

        /// <summary>
        /// Constructor for <see cref="Chart"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="Chart"/>.</param>
        /// <param name="channelName">Name of the channel whose data is represented.</param>
        /// <param name="chartHeight">Height of the <see cref="Chart"/>. Default value is <c>300</c></param>
        public Chart(string name, string channelName, int chartHeight = 300)
        {
            InitializeComponent();
            ScottPlotChart.Height = chartHeight;
            ChartName = name;
            this.channelName = channelName;
            carImage = new Bitmap("car_top_game.png");
        }

        /// <summary>
        /// Initializes and renders a plot for <see cref="Laps"/>.
        /// </summary>
        /// <param name="xAxisValues">Values on <b>horizontal</b> axis.</param>
        /// <param name="yAxisValues">Values on <b>vertical</b> axis.</param>
        /// <param name="lapIndex">Index of selected <see cref="Lap"/>.</param>
        /// <param name="driverName">Name of the selected <see cref="Driver"/>.</param>
        /// <param name="fileName">Name of the selected file.</param>
        /// <param name="yAxisLabel">Label on <b>vertical</b> axis.</param>
        /// <param name="xAxisLabel">Label on <b>horizontal</b> axis. Default is <c>Distance (m)</c></param>
        public void InitPlot(double[] xAxisValues, double[] yAxisValues, int lapIndex, string driverName, string fileName, string yAxisLabel, string xAxisLabel = "Distance (m)")
        {
            plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues, yAxisValues, markerShape: MarkerShape.none, label: $"lap {lapIndex} - {driverName} - {fileName}");
            plottableVLine = ScottPlotChart.plt.PlotVLine(0, lineStyle: LineStyle.Dash);

            ScottPlotChart.plt.Style(ScottPlot.Style.Gray1);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
            ScottPlotChart.plt.YLabel(yAxisLabel);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();
        }

        /// <summary>
        /// Initializes and renders a plot for <see cref="Driverless"/>.
        /// </summary>
        /// <param name="xValue">Selected points <b>x</b> value</param>
        /// <param name="yValue">Selected points <b>y</b> value</param>
        /// <param name="xAxisValues">Values on <b>horizontal</b> axis.</param>
        /// <param name="yAxisValues">Values on <b>vertical</b> axis.</param>
        /// <param name="vLineColor"><see cref="Color"/> of the VLine.</param>
        /// <param name="yAxisLabel">Label on <b>vertical</b> axis.</param>
        /// <param name="xAxisLabel">Label on <b>horizontal</b> axis. Default is <c>x</c></param>
        /// <param name="plotVLine">If true, a VLine will be plotted.</param>
        /// <param name="plotHighlightPoint">If true, a highlighted point will be plotted at <paramref name="xValue"/> and <paramref name="yValue"/>.</param>
        /// <param name="labelEnabled">If true, the label on the plot is enabled.</param>
        public void InitPlot(double xValue,
                             double yValue,
                             double[] xAxisValues,
                             double[] yAxisValues,
                             Color vLineColor,
                             string yAxisLabel = "",
                             string xAxisLabel = "x",
                             bool plotVLine = true,
                             bool plotHighlightPoint = false,
                             bool labelEnabled = false
                             )
        {
            if (labelEnabled)
            {
                plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues, yAxisValues, markerShape: MarkerShape.none, label: $"{yAxisLabel}: {yValue:f3}");
            }
            else
            {
                plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues, yAxisValues, markerShape: MarkerShape.none);
            }

            if (plotVLine)
            {
                plottableVLine = ScottPlotChart.plt.PlotVLine(xValue, lineStyle: LineStyle.Dash, color: vLineColor, label: $"{yAxisLabel}: {yValue:f3}");
            }

            if (plotHighlightPoint)
            {
                ScottPlotChart.plt.PlotPoint(xValue, yValue, color: Color.Red, markerSize: 10);
            }

            ScottPlotChart.plt.Style(ScottPlot.Style.Gray1);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
            ScottPlotChart.plt.YLabel(yAxisLabel);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();
        }

        /// <summary>
        /// Creates a <see cref="Tracks.Classes.DriverlessTrack"/>-s plot.
        /// </summary>
        /// <param name="xAxisValues">Values on <b>horizontal</b> axis.</param>
        /// <param name="yAxisValues">Values on <b>vertical</b> axis.</param>
        /// <param name="yAxisLabel">Label on <b>vertical</b> axis. Default is an empty string.</param>
        /// <param name="xAxisLabel">Label on <b>horizontal</b> axis. Default is an empty string.</param>
        public void InitPlot(double[] xAxisValues, double[] yAxisValues, Color color, string yAxisLabel = "", string xAxisLabel = "", double lineWidth = 3, LineStyle lineStyle = LineStyle.Solid)
        {
            plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues,
                                                                                yAxisValues,
                                                                                markerShape: MarkerShape.none,
                                                                                color: color,
                                                                                lineWidth: lineWidth,
                                                                                lineStyle: lineStyle
                                                                                );

            ScottPlotChart.plt.Frame(false);
            ScottPlotChart.plt.Style(ScottPlot.Style.Gray1);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
            ScottPlotChart.plt.YLabel(yAxisLabel);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();
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

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /*double mouseXPosition = ScottPlotChart.GetMouseCoordinates().x;
            RenderPlot(mouseXPosition);

            Console.WriteLine(plottableScatterHighlight.GetPointNearestX(mouseXPosition));*/
        }

        /// <summary>
        /// Updates the VLine on the chart.
        /// </summary>
        /// <param name="xValue">VLines <b>x</b> coordinate.</param>
        /// <param name="yValue"><b>Vertical</b> value of the vertical axis.</param>
        /// <param name="yAxisLabel"><b>Y</b> axis label.</param>
        /// <param name="vLineColor"><see cref="Color"/> of the VLine.</param>
        public void RenderPlot(double xValue, double yValue, string yAxisLabel, Color vLineColor)
        {
            plottableScatterHighlight.HighlightClear();
            ScottPlotChart.plt.Clear(plottableVLine);

            plottableVLine = ScottPlotChart.plt.PlotVLine(xValue, lineStyle: LineStyle.Dash, color: vLineColor, label: $"{yAxisLabel}: {yValue:f3}");

            ScottPlotChart.Render();
        }
    }


    /*  public string Name { get; set; }
      private const ushort chart_minimum_height = 100;
      private const ushort chart_height = 350;
      private int max_step_x
      {
          get
          {
              return MaxValueX - MinValueX;
          }
      }
      private const int step_size = 1;
      private int chart_step_size = 0;

      public Chart(string name)
      {
          InitializeComponent();
          Name = name;
          chart.DataTooltip = null;
          chart.DisableAnimations = true;
          chart.Hoverable = false;
         // chart.MinHeight = chart_minimum_height;
          chart.Height = chart_height;
          chart.Zoom = ZoomingOptions.Xy;

          UpdateAxisValues();
      }

      public void UpdateAxisValues()
      {
          try
          {
              chart.AxisX[0].MinValue = chart_step_size;
              chart.AxisX[0].MaxValue = chart_step_size + MaxValueY;
              chart.AxisY[0].MinValue = MinValueX;
              chart.AxisY[0].MaxValue = MaxValueX;
          }
          catch (Exception) { }
      }

      private void pushLeft_Click(object sender, RoutedEventArgs e)
      {
          chart_step_size += step_size;
          UpdateAxisValues();
      }

      private void pushRight_Click(object sender, RoutedEventArgs e)
      {
          chart_step_size -= step_size;
          UpdateAxisValues();
      }

      public void AddAxisX(Axis axis) => chart.AxisX.Add(axis);

      public void AddAxisY(Axis axis) => chart.AxisY.Add(axis);

      public Axis AxisY => chart.AxisY[0];

      public Axis AxisX => chart.AxisX[0];

      public void AddSerie(LineSeries serie) => chart.Series.Add(serie);

      public void ResetZoom()
      {
          chart.AxisX[0].MinValue = double.NaN;
          chart.AxisX[0].MaxValue = double.NaN;
          chart.AxisY[0].MinValue = double.NaN;
          chart.AxisY[0].MaxValue = double.NaN;
      }

      public int MaxValueX { get; set; } = 0;
      public int MinValueX { get; set; } = 0;
      public int MaxValueY { get; set; } = 0;
      public int MinValueY { get; set; } = 0;*/
}
