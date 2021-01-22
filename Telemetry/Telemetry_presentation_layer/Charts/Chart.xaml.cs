using ScottPlot;
using ScottPlot.Drawing;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Controls;
using Telemetry_data_and_logic_layer.Groups;
using System.Linq;

namespace Telemetry_presentation_layer.Charts
{
    /// <summary>
    /// Represents a <see cref="Chart"/> based on <see cref="ScottPlot"/>.
    /// </summary>
    public partial class Chart : UserControl
    {
        private PlottableScatterHighlight plottableScatterHighlight;
        private PlottableVLine plottableVLine;
        private readonly ScottPlot.Style chartStyle = ScottPlot.Style.Light1;

        /// <summary>
        /// Name of the <see cref="Chart"/>.
        /// </summary>
        public string ChartName { get; set; }

        /// <summary>
        /// List of channel names whose are in this <see cref="Chart"/>.
        /// </summary>
        private readonly List<string> channelNames = new List<string>();

        /// <summary>
        /// It decides that this chart has only one VLine.
        /// </summary>
        public bool HasVLine { get; set; } = false;

        /// <summary>
        /// Constructor for <see cref="Chart"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="Chart"/>.</param>
        /// <param name="channelNames">List of channel names whose are in this <see cref="Chart"/>.</param>
        /// <param name="chartHeight">Height of the <see cref="Chart"/>. Default value is <c>300</c></param>
        public Chart(string name, int chartHeight = 300)
        {
            InitializeComponent();
            ScottPlotChart.Height = chartHeight;
            ChartName = name;
        }

        /// <summary>
        /// Add a channel name to the <see cref="channelNames"/> list.
        /// </summary>
        /// <param name="channelName">Channels name you want to add.</param>
        public void AddChannelName(string channelName) => channelNames.Add(channelName);

        /// <summary>
        /// Checks if the <paramref name="channelName"/> is in the <see cref="channelNames"/> list.
        /// </summary>
        /// <param name="channelName">Channels name you want to check.</param>
        /// <returns></returns>
        public bool HasChannelName(string channelName) => channelNames.Contains(channelName);

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

            ScottPlotChart.plt.Style(chartStyle);
            ScottPlotChart.plt.Colorset(Colorset.Category10);
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
        /// <param name="lineColor"><see cref="Color"/> of the line.</param>
        /// <param name="channels"><see cref="Channel"/>s.</param>
        /// <param name="yAxisLabel">Label on <b>vertical</b> axis.</param>
        /// <param name="xAxisLabel">Label on <b>horizontal</b> axis. Default is <c>x</c></param>
        /// <param name="plotVLine">If true, a VLine will be plotted.</param>
        /// <param name="plotHighlightPoint">If true, a highlighted point will be plotted at <paramref name="xValue"/> and <paramref name="yValue"/>.</param>
        public void InitPlot(double xValue,
                             double yValue,
                             double[] xAxisValues,
                             double[] yAxisValues,
                             Color vLineColor,
                             Color lineColor,
                             List<Channel> channels,
                             int dataIndex,
                             string yAxisLabel = "",
                             string xAxisLabel = "x",
                             bool plotVLine = true,
                             bool plotHighlightPoint = false
                             )
        {
            plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues, yAxisValues, markerShape: MarkerShape.none, color: lineColor);

            if (plotVLine)
            {
                plottableVLine = ScottPlotChart.plt.PlotVLine(xValue,
                                                              lineStyle: LineStyle.Dash,
                                                              color: vLineColor);
            }

            if (plotHighlightPoint)
            {
                ScottPlotChart.plt.PlotPoint(xValue, yValue, color: Color.Red, markerSize: 10);
            }

            ScottPlotChart.plt.Style(chartStyle);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
            ScottPlotChart.plt.YLabel(yAxisLabel);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();

            UpdateSideValues(ref channels, ref dataIndex);
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

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /*double mouseXPosition = ScottPlotChart.GetMouseCoordinates().x;
            RenderPlot(mouseXPosition);

            Console.WriteLine(plottableScatterHighlight.GetPointNearestX(mouseXPosition));*/
        }

        /// <summary>
        /// Updates the VLine(s) on the chart.
        /// </summary>
        /// <param name="xValue">VLines <b>x</b> coordinate.</param>
        /// <param name="vLineColor"><see cref="Color"/> of the VLine.</param>
        /// <param name="channels"><b>All channels from the <see cref="Driverless.UserControls.DriverlessMenu"/>.</param>
        /// <param name="dataIndex"><b>Index of the channel data.</param>
        public void RenderPlot(double xValue, Color vLineColor, List<Channel> channels, int dataIndex)
        {
            plottableScatterHighlight.HighlightClear();
            ScottPlotChart.plt.Clear(plottableVLine);

            plottableVLine = ScottPlotChart.plt.PlotVLine(xValue,
                                                          lineStyle: LineStyle.Dash,
                                                          color: vLineColor);

            ScottPlotChart.Render();
            UpdateSideValues(ref channels, ref dataIndex);
        }

        /// <summary>
        /// Update the <see cref="ChartValue"/>s next to the <see cref="Chart"/>.
        /// </summary>
        /// <param name="channels">List, where the <see cref="Channel"/>s are.</param>
        /// <param name="dataIndex">Index where the actual data is.</param>
        private void UpdateSideValues(ref List<Channel> channels, ref int dataIndex)
        {
            if (ValuesStackPanel.Children.Count == 0)
            {
                foreach (var channel in channels)
                {
                    if (HasChannelName(channel.Name))
                    {
                        ValuesStackPanel.Children.Add(new ChartValue(channel.Color, channel.Name, dataIndex < channel.Data.Count ? channel.Data[dataIndex] : channel.Data.Last()));
                    }
                }
            }
            else
            {
                foreach (ChartValue item in ValuesStackPanel.Children)
                {
                    var channel = channels.Find(x => x.Name.Equals(item.ChannelName));
                    double value = dataIndex < channel.Data.Count ? channel.Data[dataIndex] : channel.Data.Last();
                    item.SetChannelValue(value);
                }
            }
        }
    }
}
