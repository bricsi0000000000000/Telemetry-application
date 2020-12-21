﻿using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Groups.UserControls;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Tracks.Classes;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Point = System.Windows.Point;

namespace ART_TELEMETRY_APP.Driverless.UserControls
{
    /// <summary>
    /// This class contains the **main menu** to the driverless section.
    /// </summary>
    public partial class DriverlessMenu : UserControl
    {
        /// <summary>
        /// A list of the channels from the input file.
        /// </summary>
        public List<Channel> Channels { get; } = InputFileManager.FirstDriverlessInputFile.Channels;

        /// <summary>
        /// List of selected groups.
        /// </summary>
        private readonly List<string> selectedGroups = new List<string>();

        /// <summary>
        /// Yaw rate integrate.
        /// </summary>
        private readonly List<double> integratedYawangle = new List<double>();

        /// <summary>
        /// Delta time for <see cref="integratedYawangle"/>.
        /// 50 ms -> 0.05 sec
        /// </summary>
        private readonly float dt = .05f;

        /// <summary>
        /// Constructor of the <see cref="DriverlessMenu"/> class.
        /// </summary>
        public DriverlessMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the channels based on <see cref="Channels"/>.
        /// </summary>
        private void InitChannelCheckBoxes()
        {
            ChannelsStackPanel.Children.Clear();

            foreach (var channel in Channels)
            {
                AddChannelCheckBox(channel);
            }
        }

        /// <summary>
        /// Initialize the groups based on <see cref="GroupManager.Groups"/>.
        /// </summary>
        private void InitGroups()
        {
            foreach (var group in GroupManager.Groups)
            {
                if (group.Driverless)
                {
                    var checkBox = new CheckBox()
                    {
                        Content = group.Name
                    };
                    checkBox.Click += GroupCheckBox_Click;

                    GroupsStackPanel.Children.Add(checkBox);
                }
            }
        }

        /// <summary>
        /// Updates groups after the <see cref="CheckBox"/>-es state is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string content = checkBox.Content.ToString();
            if ((bool)checkBox.IsChecked)
            {
                selectedGroups.Add(content);
            }
            else
            {
                selectedGroups.Remove(content);
            }

            UpdateCharts();
        }

        /// <summary>
        /// Adds a single <see cref="CheckBox"/> to the <see cref="ChannelsStackPanel"/> based on the <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel">The <see cref="CheckBox"/> will be made based on this <see cref="Channel"/>.</param>
        private void AddChannelCheckBox(Channel channel)
        {
            var checkBox = new CheckBox()
            {
                Content = channel.Name,
                IsChecked = channel.IsActive
            };

            checkBox.Click += ChannelCheckBox_Click;

            ChannelsStackPanel.Children.Add(checkBox);
        }

        /// <summary>
        /// Updates charts after the <see cref="CheckBox"/>-es state is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            GetChannel(checkBox.Content.ToString()).IsActive = (bool)checkBox.IsChecked;

            UpdateCharts();
        }

        /// <summary>
        /// Clears all the children of <see cref="ChannelsStackPanel"/> and fill it with the newly created Charts.
        /// </summary>
        public void UpdateCharts()
        {
            ChartsStackPanel.Children.Clear();
            foreach (var channel in Channels)
            {
                if (channel.IsActive)
                {
                    ChartsStackPanel.Children.Add(BuildChart(channel));
                }
            }

            foreach (var group in GroupManager.Groups)
            {
                if (selectedGroups.Contains(group.Name))
                {
                    if (group.Driverless)
                    {
                        if (((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).GetGroupSettingsContent(group.Name) != null)
                        {
                            ChartsStackPanel.Children.Add(BuildGroupChart(group));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="Chart"/> based on the <paramref name="channel"/>-s <see cref="Channel.Data"/>.
        /// </summary>
        /// <param name="channel">The <see cref="Chart"/> will be made based on this <see cref="Channel"/>.</param>
        /// <returns>A <see cref="Chart"/> with the highligthed point and the v-line</returns>
        private Chart BuildChart(Channel channel)
        {
            var chart = new Chart(channel.Name);
            chart.AddChannelName(channel.Name);
            var data = ConvertChannelDataToPlotData(channel.Data.ToArray(), HorizontalAxisData.Data);
            int dataIndex = (int)DataSlider.Value;

            var color = ColorManager.GetChartColor;
            channel.Color = color;
            double xValue = dataIndex < HorizontalAxisData.Data.Count ? HorizontalAxisData.Data[dataIndex] : HorizontalAxisData.Data.Last();
            double yValue = dataIndex < channel.Data.Count ? channel.Data[dataIndex] : channel.Data.Last();

            chart.InitPlot(xValue: xValue,
                           yValue: yValue,
                           xAxisValues: data.Item1,
                           yAxisValues: data.Item2,
                           vLineColor: Color.White,
                           lineColor: Color.FromArgb(channel.Color.A, channel.Color.R, channel.Color.G, channel.Color.B),
                           channels: Channels,
                           dataIndex: dataIndex,
                           yAxisLabel: channel.Name);

            return chart;
        }

        /// <summary>
        /// Builds the track.
        /// </summary>
        /// <returns>The <see cref="Chart"/> that contains the selected track width the cars path.</returns>
        private TrackChart BuildTrack()
        {
            var chart = new TrackChart();

            if ((bool)TrackStraightRadioButton.IsChecked)
            {
                var track = DriverlessTrackManager.GetTrack("Straight");
                if (track != null)
                {
                    var data = ConvertChannelDataToPlotData(track.LeftSide);
                    int dataIndex = (int)DataSlider.Value;

                    chart.InitPlot(xAxisValues: data.Item1,
                                   yAxisValues: data.Item2,
                                   color: Color.Black);

                    data = ConvertChannelDataToPlotData(track.RightSide);
                    chart.InitPlot(xAxisValues: data.Item1,
                                   yAxisValues: data.Item2,
                                   color: Color.Black);

                    data = ConvertChannelDataToPlotData(track.Center);
                    chart.InitPlot(xAxisValues: data.Item1,
                                   yAxisValues: data.Item2,
                                   color: Color.Black,
                                   lineStyle: ScottPlot.LineStyle.Dash);

                    var channelData = CreateOffset(GetChannel("y").Data, (float)GetChannel("c0ref").Data.First());
                    data = ConvertChannelDataToPlotData(HorizontalAxisData.Data.ToArray(), channelData);

                    double xValue = 0;
                    double yValue = 0;
                    if (dataIndex < HorizontalAxisData.Data.Count)
                    {
                        xValue = channelData[dataIndex];
                        yValue = HorizontalAxisData.Data[dataIndex];
                    }
                    else
                    {
                        xValue = channelData.Last();
                        yValue = HorizontalAxisData.Data.Last();
                    }

                    chart.InitPlot(xValue: xValue,
                                   yValue: yValue,
                                   xAxisValues: data.Item1,
                                   yAxisValues: data.Item2,
                                   color: Color.Orange,
                                   yAxisLabel: "y",
                                   lineWidth: 2,
                                   enableLabel: true
                                   );

                    chart.SetAxisLimitsToAuto();
                    chart.SetFrameBorder(left: false, bottom: false, top: false, right: false);

                    if (dataIndex < integratedYawangle.Count)
                    {
                        chart.PlotImage(xValue, yValue, CreateOffset(integratedYawangle, (float)GetChannel("c0ref").Data.First())[dataIndex]);
                    }
                    else
                    {
                        chart.PlotImage(xValue, yValue, CreateOffset(integratedYawangle, (float)GetChannel("c0ref").Data.First()).Last());
                    }
                }
                else
                {
                    ShowError.ShowErrorMessage(ref ErrorSnackbar, "Straight track can't be found!");
                }
            }

            return chart;
        }

        /// <summary>
        /// Builds one <see cref="Chart"/> with the <paramref name="group"/>s values.
        /// </summary>
        /// <param name="group"><see cref="Group"/> that will shown on the <see cref="Chart"/></param>
        /// <returns>A <see cref="Chart"/> with the <paramref name="group"/>s values.</returns>
        private Chart BuildGroupChart(Group group)
        {
            var chart = new Chart(group.Name);
            foreach (var channel in Channels)
            {
                if (group.GetAttribute(channel.Name) != null)
                {
                    chart.AddChannelName(channel.Name);
                    channel.Color = group.GetAttribute(channel.Name).Color;
                }
            }

            foreach (var channel in Channels)
            {
                if (chart.HasChannelName(channel.Name))
                {
                    var data = ConvertChannelDataToPlotData(channel.Data.ToArray(), HorizontalAxisData.Data);
                    int dataIndex = (int)DataSlider.Value;
                    double xValue = dataIndex < HorizontalAxisData.Data.Count ? HorizontalAxisData.Data[dataIndex] : HorizontalAxisData.Data.Last();
                    double yValue = dataIndex < channel.Data.Count ? channel.Data[dataIndex] : channel.Data.Last();
                   
                    chart.InitPlot(xValue: xValue,
                                    yValue: yValue,
                                    xAxisValues: data.Item1,
                                    yAxisValues: data.Item2,
                                    vLineColor: Color.White,
                                    lineColor: Color.FromArgb(channel.Color.A, channel.Color.R, channel.Color.G, channel.Color.B),
                                    channels: Channels,
                                    dataIndex: dataIndex,
                                    yAxisLabel: group.Name,
                                    plotVLine: !chart.HasVLine);

                    chart.HasVLine = true;
                }
            }

            chart.SetAxisLimitsToAuto();

            return chart;
        }

        /// <summary>
        /// Pushes the <paramref name="list"/> with <paramref name="offset"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private List<double> CreateOffset(List<double> list, float offset)
        {
            var newList = new List<double>();
            foreach (var number in list)
            {
                newList.Add(number - offset);
            }

            return newList;
        }

        /// <summary>
        /// Converts all data from a <see cref="Channel"/> to plot data.
        /// The horizontal <i>(x)</i> axis is based on <see cref="HorizontalAxisData"/>.
        /// </summary>
        /// <param name="lapData">Data from a single <see cref="Channel"/> converted to a double array.</param>
        /// <param name="horizontalAxisData">Horizontal axis data.</param>
        /// <returns>Horizontal and vertical axes data.</returns>
        private Tuple<double[], double[]> ConvertChannelDataToPlotData(double[] lapData, List<double> horizontalAxisData)
        {
            var x = new List<double>();
            var y = new List<double>();

            for (ushort i = 0; i < lapData.Length; i++)
            {
                x.Add(horizontalAxisData[i]);
                y.Add(lapData[i]);
            }

            return new Tuple<double[], double[]>(x.ToArray(), y.ToArray());
        }

        /// <summary>
        /// Converts a <see cref="DriverlessTrack"/>-s side points to plot data.
        /// </summary>
        /// <param name="points">Convertable list of <see cref="Point"/>-s.</param>
        /// <returns>Converted plot data.</returns>
        private Tuple<double[], double[]> ConvertChannelDataToPlotData(List<Point> points)
        {
            var x = new List<double>();
            var y = new List<double>();

            for (ushort i = 0; i < points.Count; i++)
            {
                x.Add(points[i].X);
                y.Add(points[i].Y);
            }

            return new Tuple<double[], double[]>(x.ToArray(), y.ToArray());
        }

        /// <summary>
        /// The horizontal axis will be based on the <i>x</i> <see cref="Channel"/>.
        /// </summary>
        private Channel HorizontalAxisData => GetChannel("x");

        /// <summary>
        /// Finds a <see cref="Channel"/> based on <paramref name="channelName"/>.
        /// </summary>
        /// <param name="channelName">The name of the <see cref="Channel"/> to find.</param>
        /// <returns>A single <see cref="Channel"/></returns>
        public Channel GetChannel(string channelName)
        {
            return Channels.Find(x => x.Name.Equals(channelName));
        }

        /// <summary>
        /// Reads a selected file from the computer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Read file",
                DefaultExt = ".csv",
                Multiselect = false,
                Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName.Split('\\').Last();
                ReadFileProgressBarLbl.Content = $"Reading \"{fileName}\"";
                DataReader.Instance.ReadData(openFileDialog.FileName,
                                             ReadFileProgressBarGrid,
                                             ReadFileProgressBar,
                                             ErrorSnackbar);
            }
        }

        /// <summary>
        /// Initializes <see cref="integratedYawangle"/> list based on <c>yawangle</c> and <c>yawrate</c>.
        /// Formula: <c>yawangle(x) = yawangle(x - 1) + dt * yawrate</c>,
        /// where <c>x</c> is the loop variable and <c>dt</c> ist the timestep in <b>ms</b>.
        /// </summary>
        public void InitIntegratedYawangle()
        {
            integratedYawangle.Clear();

            var yawrate = GetChannel(TextManager.DefaultYawrateChannelName);
            if (yawrate == null)
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, $"Can't find '{TextManager.DefaultYawrateChannelName}' channel");
                return;
            }

            integratedYawangle.Add(0);
            for (int i = 1; i < yawrate.Data.Count; i++)
            {
                integratedYawangle.Add(integratedYawangle[i - 1] + dt * yawrate.Data[i]);
            }

            for (int i = 0; i < integratedYawangle.Count; i++)
            {
                integratedYawangle[i] *= 180 / Math.PI;

            }
        }

        /// <summary>
        /// Creates the track.
        /// The <i>x</i> axis is the <see cref="HorizontalAxisData"/>.
        /// The <i>y</i> axis is the <b>y</b> Channels <see cref="Channel.Data"/>.
        /// </summary>
        public void UpdateTrack()
        {
            TrackStackPanel.Children.Clear();
            TrackStackPanel.Children.Add(BuildTrack());
        }

        /// <summary>
        /// Changes the newly readed channels, than updates everything:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="InitChannelCheckBoxes()"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="SetUpDataSlider()"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="UpdateCharts()"/></description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="channels">New channels.</param>
        public void UpdateAfterReadFile()
        {
            InitChannelCheckBoxes();
            InitGroups();
            SetUpDataSlider();
            InitIntegratedYawangle();
            // TODO: ha nem kell a highlight karika, akkor a kikommentelt sor kell, az UpdateCharts(); pedig nem
            UpdateCharts();
            //ChangeChartHighlight((int)DataSlider.Value);
            UpdateTrack();
        }

        /// <summary>
        /// Sets the <see cref="DataSlider"/>s maximum value based on the <see cref="Channels"/> first <see cref="Channel.Data"/>-s count.
        /// </summary>
        private void SetUpDataSlider()
        {
            DataSlider.Maximum = Channels.First().Data.Count;
        }

        /// <summary>
        /// Updates the Charts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // TODO: ha nem kell a highlight karika, akkor a kikommentelt sor kell, az UpdateCharts(); pedig nem
            //  UpdateCharts();
            UpdateTrack();
            ChangeChartHighlight((int)((Slider)sender).Value);
        }

        /// <summary>
        /// Changes the charts highlights, so the VLines.
        /// </summary>
        /// <param name="dataIndex">Index of the data on the horizontal axis.</param>
        private void ChangeChartHighlight(int dataIndex)
        {
            foreach (Chart chart in ChartsStackPanel.Children)
            {
                double value = dataIndex < HorizontalAxisData.Data.Count ? HorizontalAxisData.Data[dataIndex] : HorizontalAxisData.Data.Last();
                chart.RenderPlot(xValue: value,
                                 vLineColor: Color.White,
                                 Channels,
                                 dataIndex);
            }
        }

        /// <summary>
        /// Unchecks all the checked Checkboxes inside the <see cref="ChannelsStackPanel"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UncheckAllChannels_Click(object sender, RoutedEventArgs e)
        {
            foreach (var channel in Channels)
            {
                channel.IsActive = false;
            }

            foreach (CheckBox checkBox in ChannelsStackPanel.Children)
            {
                checkBox.IsChecked = false;
            }

            UpdateCharts();
        }
    }
}
