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
using System.Diagnostics;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Menus;
using Telemetry_presentation_layer.Menus.Driverless;
using Telemetry_presentation_layer.Menus.Settings.Groups;
using Telemetry_presentation_layer.Menus.Settings;
using Telemetry_data_and_logic_layer.Colors;
using System.Windows.Input;
using Telemetry_presentation_layer.Defaults;

namespace Telemetry_presentation_layer.Charts
{
    /// <summary>
    /// Represents a <see cref="Chart"/> based on <see cref="ScottPlot"/>.
    /// </summary>
    public partial class Chart : UserControl
    {
        private PlottableScatterHighlight plottableScatterHighlight;
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

        private List<Tuple<double[], double[], Color, int>> values = new List<Tuple<double[], double[], Color, int>>();

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
                            int lineWidth,
                            string xAxisLabel = "x"
                            )
        {
            values.Add(new Tuple<double[], double[], Color, int>(xAxisValues, yAxisValues, lineColor, lineWidth));

            UpdatePlot(xAxisValues[0]);

            ScottPlotChart.plt.Style(chartStyle);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();
        }

        public void UpdatePlot(double xValue)
        {
            ScottPlotChart.plt.Clear();
            foreach (var item in values)
            {
                plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(item.Item1, item.Item2, markerShape: MarkerShape.none, color: item.Item3, lineWidth: item.Item4);
            }

            ScottPlotChart.plt.PlotVLine(xValue, lineStyle: LineStyle.Dash, color: DefaultsManager.DefaultChartHighlightColor);

            /*plottableScatterHighlight.HighlightPointNearestX(xValue);
            plottableScatterHighlight.highlightedColor = DefaultsManager.DefaultChartHighlightColor;
            plottableScatterHighlight.highlightedShape = MarkerShape.filledCircle;*/
            ScottPlotChart.Render();



            // plottableScatterHighlight.HighlightClear();
            //plottableScatterHighlight.HighlightPointNearestX(xValue);

            /*ScottPlotChart.plt.PlotVLine(xValue,
                                         lineStyle: LineStyle.Dash,
                                         color: vLineColor);

            plottableScatterHighlight.HighlightClear();

            ScottPlotChart.plt.PlotVLine(xValue + 2,
                                         lineStyle: LineStyle.DashDot,
                                         color: vLineColor);*/
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
                ValuesStackPanel.Children.Add(new ChartValue(yAxisLabel, /*liveChartValues.Last(),*/ unit == null ? "" : unit.UnitOfMeasure, "", "#4d4d4d", 0));
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

            ValuesStackPanel.Children.Add(new ChartValue(channelName, unit == null ? "" : unit.UnitOfMeasure, ChartName));
            SettingsStackPanel.Children.Add(new ChartValueSettings(channelName, ChartName));

            ChannelsGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        public void AddSideValue(int inputFileID, string channelName, ref int dataIndex, int lineWidth, bool isActive, string color = "")
        {
            var channel = InputFileManager.GetInputFile(inputFileID).GetChannel(channelName);

            var unit = UnitOfMeasureManager.GetUnitOfMeasure(channelName);
            if (unit == null)
            {
                unit = UnitOfMeasureManager.GetUnitOfMeasure(channelName.Replace("_", ""));
            }

            var chartValue = new ChartValue(channelName, unit == null ? "" : unit.UnitOfMeasure, ChartName, color, inputFileID);
            chartValue.SetUp(color, inputFileID);

            double value = dataIndex < channel.Data.Count ? channel.Data[dataIndex] : channel.Data.Last();
            chartValue.SetChannelValue(value);

            ValuesStackPanel.Children.Add(chartValue);

            SettingsStackPanel.Children.Add(new ChartValueSettings(channelName: channelName,
                                                                   groupName: ChartName,
                                                                   lineWidth: lineWidth,
                                                                   color: color,
                                                                   inputFileID: inputFileID,
                                                                   isActive: isActive));

            ChannelsGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            string channelName = e.Data.GetData(typeof(string)).ToString();

            var group = GroupManager.GetGroup(ChartName);

            if (group != null)
            {
                if (group.GetAttribute(channelName) == null)
                {
                    GroupManager.GetGroup(ChartName).AddAttribute(channelName, ColorManager.GetChartColor, 1);
                }
            }
            else
            {
                if (!channelNames.Contains(channelName))
                {
                    channelNames.Add(channelName);

                    string oldName = ChartName;

                    var temporaryGroup = GroupManager.GetGroup($"Temporary{GroupManager.TemporaryGroupIndex}");

                    while (temporaryGroup != null)
                    {
                        GroupManager.TemporaryGroupIndex++;
                        temporaryGroup = GroupManager.GetGroup($"Temporary{GroupManager.TemporaryGroupIndex}");
                    }

                    ChartName = $"Temporary{GroupManager.TemporaryGroupIndex}";
                    var newGroup = new Group(GroupManager.LastGroupID++, ChartName);
                    foreach (var name in channelNames)
                    {
                        newGroup.AddAttribute(name, ColorManager.GetChartColor, 1);
                    }
                    GroupManager.AddGroup(newGroup);
                    ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).ReplaceChannelWithTemporaryGroup(oldName, ChartName);
                }
            }

            GroupManager.SaveGroups();
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();
            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).BuildCharts();

            Mouse.OverrideCursor = null;
        }
    }
}
