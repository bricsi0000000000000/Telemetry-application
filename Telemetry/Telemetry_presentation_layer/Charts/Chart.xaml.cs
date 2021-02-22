using ScottPlot;
using ScottPlot.Drawing;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Controls;
using Telemetry_data_and_logic_layer.Groups;
using System.Linq;
using Telemetry_presentation_layer.Errors;
using Telemetry_data_and_logic_layer.Units;
using Telemetry_data_and_logic_layer.InputFiles;

namespace Telemetry_presentation_layer.Charts
{
    /// <summary>
    /// Represents a <see cref="Chart"/> based on <see cref="ScottPlot"/>.
    /// </summary>
    public partial class Chart : UserControl
    {
        private PlottableScatterHighlight plottableScatterHighlight;
        private PlottableVLine plottableVLine;
        private readonly Style chartStyle = ScottPlot.Style.Light1;
        private List<double> liveChartValues = new List<double>();

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
        /// Represents a chart.
        /// </summary>
        /// <param name="name">Name of the <see cref="Chart"/>.</param>
        /// <param name="channelNames">List of channel names whose are in this <see cref="Chart"/>.</param>
        public Chart(string name)
        {
            InitializeComponent();
            ChartName = name;
            ScottPlotChart.Configure(enableScrollWheelZoom: false);
            ScottPlotChart.plt.YLabel(name);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();
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
        /// Adds and renders a plot for <see cref="Driverless"/>.
        /// </summary>
        /// <param name="xAxisValues">Values on <b>horizontal</b> axis.</param>
        /// <param name="yAxisValues">Values on <b>vertical</b> axis.</param>
        /// <param name="lineColor"><see cref="Color"/> of the line.</param>
        /// <param name="xAxisLabel">Label on <b>horizontal</b> axis. Default is <c>x</c></param>
        public void AddPlot(double[] xAxisValues,
                            double[] yAxisValues,
                            Color lineColor,
                            string xAxisLabel = "x"
                            )
        {
            plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues, yAxisValues, markerShape: MarkerShape.none, color: lineColor);

            ScottPlotChart.plt.Style(chartStyle);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();
        }

        public void UpdateVLine(double xValue, Color vLineColor)
        {
            plottableVLine = ScottPlotChart.plt.PlotVLine(xValue,
                                                          lineStyle: LineStyle.Dash,
                                                          color: vLineColor);
        }

        /// <summary>
        /// Updates the plot with new data.
        /// </summary>
        /// <param name="data"></param>
        public void Update(double[] data, string xAxisLabel, string yAxisLabel)
        {
            liveChartValues.AddRange(data);

            ScottPlotChart.plt.Clear();
            ScottPlotChart.plt.PlotSignal(liveChartValues.ToArray(), markerSize: 0);

            ScottPlotChart.plt.Style(chartStyle);
            ScottPlotChart.plt.Colorset(Colorset.Category10);
            ScottPlotChart.plt.YLabel(yAxisLabel);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();

            SetAxisLimitsToAuto();

            if (ValuesStackPanel.Children.Count == 0)
            {
                var unit = UnitOfMeasureManager.GetUnitOfMeasure(yAxisLabel);
                ValuesStackPanel.Children.Add(new ChartValue("#4d4d4d", yAxisLabel, /*liveChartValues.Last(),*/ unit == null ? "" : unit.UnitOfMeasure, 0));
            }
            else
            {
                foreach (ChartValue item in ValuesStackPanel.Children)
                {
                    item.SetChannelValue(liveChartValues.Last());
                }
            }
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

        public void AddSideValue(string channelName)
        {
            var unit = UnitOfMeasureManager.GetUnitOfMeasure(channelName);
            if (unit == null)
            {
                unit = UnitOfMeasureManager.GetUnitOfMeasure(channelName.Replace("_", ""));
            }

            ValuesStackPanel.Children.Add(new ChartValue("#ffffff", channelName, unit == null ? "" : unit.UnitOfMeasure, -1));
        }

        public void UpdateSideValues(int inputFileID, string channelName, ref int dataIndex)
        {
            var channel = InputFileManager.GetInputFile(inputFileID).GetChannel(channelName);

            foreach (ChartValue item in ValuesStackPanel.Children)
            {
                if (item.ChannelName.Equals(channelName))
                {
                    double value = dataIndex < channel.Data.Count ? channel.Data[dataIndex] : channel.Data.Last();
                    item.SetChannelValue(value);
                    item.Set(channel.Color, inputFileID);
                }
            }
        }
    }
}
