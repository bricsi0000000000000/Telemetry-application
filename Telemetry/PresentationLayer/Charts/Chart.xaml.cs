using ScottPlot;
using ScottPlot.Drawing;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using LogicLayer.Menus;
using LogicLayer.Menus.Settings.Groups;
using LogicLayer.Menus.Settings;
using System.Windows.Input;
using PresentationLayer.Units;
using PresentationLayer.Groups;
using LogicLayer.Colors;
using PresentationLayer.Texts;
using PresentationLayer.Menus.Driverless;
using PresentationLayer.Defaults;

namespace PresentationLayer.Charts
{
    /// <summary>
    /// Represents a <see cref="Chart"/> based on <see cref="ScottPlot"/>.
    /// </summary>
    public partial class Chart : UserControl
    {
        private readonly PlottableScatterHighlight plottableScatterHighlight;
        private PlottableVLine plottableVLine;
        private readonly Style chartStyle = ScottPlot.Style.Light1;

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

        private List<Tuple<string, double[]>> values = new List<Tuple<string, double[]>>();


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
            ScottPlotChart.plt.Style(chartStyle);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
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
                            string xAxisLabel)
        {
            ScottPlotChart.plt.PlotScatter(xs: xAxisValues, ys: yAxisValues, markerShape: MarkerShape.none, color: lineColor, lineWidth: lineWidth);

            ScottPlotChart.plt.XLabel(xAxisLabel, bold: true);
            ScottPlotChart.plt.Legend();
            ScottPlotChart.Render();
        }

        public void AddPlot(double[] xAxisValues,
                            Color lineColor,
                            int lineWidth,
                            string xAxisLabel)
        {
            if (xAxisValues.Any())
            {
                ScottPlotChart.plt.PlotSignal(xAxisValues, color: lineColor, lineWidth: lineWidth, markerSize: 1);
                ScottPlotChart.plt.XLabel(xAxisLabel, bold: true);
                ScottPlotChart.plt.Legend();
                ScottPlotChart.Render();
            }
        }

        public void AddLivePlot(double[] xAxisValues,
                                Color lineColor,
                                int lineWidth,
                                string xAxisLabel,
                                string channelName,
                                int minRenderIndex,
                                int maxRenderIndex)
        {
            if (xAxisValues.Any())
            {
                // save the incoming values because based on liveChartValues can the side value be updated
                int index = values.FindIndex(x => x.Item1.Equals(xAxisLabel));
                if (index == -1)
                {
                    values.Add(new Tuple<string, double[]>(channelName, xAxisValues));
                }
                else
                {
                    var newValues = new List<double>(values[index].Item2);
                    newValues.AddRange(xAxisValues);
                    values[index] = new Tuple<string, double[]>(channelName, newValues.ToArray());
                }

                ScottPlotChart.plt.PlotSignal(xAxisValues, color: lineColor, lineWidth: lineWidth, markerSize: 1, minRenderIndex: minRenderIndex, maxRenderIndex: maxRenderIndex);
                ScottPlotChart.plt.XLabel(xAxisLabel, bold: true);
                ScottPlotChart.plt.Legend();
                ScottPlotChart.Render();
            }
        }

        public void UpdateHighlight(double xValue)
        {
            if (plottableScatterHighlight != null)
            {
                plottableScatterHighlight.HighlightClear();
            }

            if (plottableVLine != null)
            {
                ScottPlotChart.plt.Clear(plottableVLine);
            }

            plottableVLine = ScottPlotChart.plt.PlotVLine(xValue, lineStyle: LineStyle.Dash, color: DefaultsManager.DefaultChartHighlightColor);

            ScottPlotChart.Render();
        }

        public void UpdateLiveHighlight(ref int dataIndex)
        {
            if (plottableScatterHighlight != null)
            {
                plottableScatterHighlight.HighlightClear();
            }

            if (plottableVLine != null)
            {
                ScottPlotChart.plt.Clear(plottableVLine);
            }

            plottableVLine = ScottPlotChart.plt.PlotVLine(dataIndex, lineStyle: LineStyle.Dash, color: DefaultsManager.DefaultChartHighlightColor);

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

        private ChartValue AddEmptySideValue(string channelName)
        {

            var unit = UnitOfMeasureManager.GetUnitOfMeasure(channelName);
            if (unit == null)
            {
                unit = UnitOfMeasureManager.GetUnitOfMeasure(channelName.Replace("_", ""));
            }

            var chartValue = new ChartValue(channelName: channelName,
                                            unitOfMeasure: unit == null ? "" : unit.UnitOfMeasure,
                                            groupName: ChartName);

            return chartValue;
        }

        public void AddSideValue(string channelName, double[] xAxisValues, bool isActive = false, int inputFileID = -1, string color = "", int lineWidth = 1)
        {
            values.Add(new Tuple<string, double[]>(channelName, xAxisValues));

            var chartValue = AddEmptySideValue(channelName);

            if (!color.Equals(string.Empty))
            {
                if (isActive)
                {
                    chartValue.SetUp(colorText: color, inputFileID: inputFileID);
                }
                SettingsStackPanel.Children.Add(new ChartValueSettings(channelName: channelName,
                                                                       groupName: ChartName,
                                                                       colorText: color,
                                                                       lineWidth: lineWidth,
                                                                       inputFileID: inputFileID,
                                                                       isActive: isActive));
            }
            else
            {
                SettingsStackPanel.Children.Add(new ChartValueSettings(channelName, ChartName, inputFileID: inputFileID, isActive: isActive));
            }

            ValuesStackPanel.Children.Add(chartValue);


            ChannelsGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        public void UpdateSideValues(ref int dataIndex)
        {
            //  var channel = InputFileManager.GetInputFile(inputFileID).GetChannel(channelName);

            /*var unit = UnitOfMeasureManager.GetUnitOfMeasure(channelName);
            if (unit == null)
            {
                unit = UnitOfMeasureManager.GetUnitOfMeasure(channelName.Replace("_", ""));
            }*/

            //var chartValue = new ChartValue(channelName, unit == null ? "" : unit.UnitOfMeasure, ChartName, color, inputFileID);
            //chartValue.SetUp(color, inputFileID);

            // double value = dataIndex < channel.Data.Count ? channel.Data[dataIndex] : channel.Data.Last();

            foreach (var item in ValuesStackPanel.Children)
            {
                if (item is ChartValue chartValue)
                {
                    var currentValues = values.Find(x => x.Item1.Equals(chartValue.ChannelName));
                    if (currentValues != null)
                    {
                        if (currentValues.Item2.Length > 0)
                        {
                            double currentValue = dataIndex < currentValues.Item2.Length ? currentValues.Item2[dataIndex] : currentValues.Item2.Last();
                            chartValue.SetChannelValue(currentValue);
                        }
                    }
                }
            }
            // chartValue.SetChannelValue(value);

            // ValuesStackPanel.Children.Add(chartValue);

            /*SettingsStackPanel.Children.Add(new ChartValueSettings(channelName: channelName,
                                                                   groupName: ChartName,
                                                                   lineWidth: lineWidth,
                                                                   color: color,
                                                                   inputFileID: inputFileID,
                                                                   isActive: isActive));*/

            ChannelsGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        public void UpdateLiveSideValue(ref int dataIndex)
        {
            if (values.Any())
            {
                foreach (var item in ValuesStackPanel.Children)
                {
                    if (item is ChartValue chartValue)
                    {
                        var channelValues = values.Find(x => x.Item1.Equals(chartValue.ChannelName)).Item2;
                        if (channelValues.Any())
                        {
                            double value = dataIndex < channelValues.Length ? channelValues[dataIndex] : channelValues.Last();
                            chartValue.SetLiveChannelValue(value);
                        }
                    }
                }
            }
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
                    GroupManager.GetGroup(ChartName).AddAttribute(channelName, ColorManager.GetChartColor.ToString(), 1);
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

                    GroupManager.AddGroup(GroupManager.MakeGroupWithAttributes(ChartName, channelNames));
                    ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).ReplaceChannelWithTemporaryGroup(oldName, ChartName);
                    MenuManager.LiveTelemetry.ReplaceChannelWithTemporaryGroup(oldName, ChartName);
                }
            }

            GroupManager.SaveGroups();
            ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();
            ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).BuildCharts();
            MenuManager.LiveTelemetry.BuildCharts();

            Mouse.OverrideCursor = null;
        }
    }
}
