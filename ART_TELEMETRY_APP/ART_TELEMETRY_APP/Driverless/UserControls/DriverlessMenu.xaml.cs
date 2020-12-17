using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Tracks.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public List<Channel> Channels { get; private set; } = new List<Channel>();

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
        private void InitChannels()
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

            
        }

        /// <summary>
        /// Adds a single <see cref="CheckBox"/> to the <see cref="ChannelsStackPanel"/> based on the <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel">The <see cref="CheckBox"/> will be made based on this <see cref="Channel"/>.</param>
        private void AddChannelCheckBox(Channel channel)
        {
            var checkBox = new CheckBox()
            {
                Content = channel.ChannelName,
                IsChecked = channel.ChannelIsActive
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

            GetChannel(checkBox.Content.ToString()).ChannelIsActive = (bool)checkBox.IsChecked;

            UpdateCharts();
            UpdateTrack();
        }

        /// <summary>
        /// Clears all the children of <see cref="ChannelsStackPanel"/> and fill it with the newly created Charts.
        /// </summary>
        private void UpdateCharts()
        {
            ChartsStackPanel.Children.Clear();
            foreach (var channel in Channels)
            {
                if (channel.ChannelIsActive)
                {
                    ChartsStackPanel.Children.Add(BuildChart(channel));
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="Chart"/> based on the <paramref name="channel"/>-s <see cref="Channel.ChannelData"/>.
        /// </summary>
        /// <param name="channel">The <see cref="Chart"/> will be made based on this <see cref="Channel"/>.</param>
        /// <returns>A <see cref="Chart"/> with the highligthed point and the v-line</returns>
        private Chart BuildChart(Channel channel)
        {
            var chart = new Chart(channel.ChannelName, channel.ChannelName);
            var data = ConvertChannelDataToPlotData(channel.ChannelData.ToArray(), HorizontalAxisData.ChannelData);
            int dataIndex = (int)DataSlider.Value;

            if (dataIndex < HorizontalAxisData.ChannelData.Count)
            {
                chart.InitPlot(xValue: HorizontalAxisData.ChannelData[dataIndex],
                               yValue: channel.ChannelData[dataIndex],
                               xAxisValues: data.Item1,
                               yAxisValues: data.Item2,
                               vLineColor: Color.LightGreen,
                               yAxisLabel: channel.ChannelName);
            }
            else
            {
                chart.InitPlot(xValue: HorizontalAxisData.ChannelData.Last(),
                               yValue: channel.ChannelData.Last(),
                               xAxisValues: data.Item1,
                               yAxisValues: data.Item2,
                               vLineColor: Color.LightGreen,
                               yAxisLabel: channel.ChannelName);
            }

            return chart;
        }

        /// <summary>
        /// Builds the track.
        /// </summary>
        /// <returns>The <see cref="Chart"/> that contains the selected track width the cars path.</returns>
        private Chart BuildTrack()
        {
            var chart = new Chart("TrackChart", "track");

            if ((bool)TrackStraightRadioButton.IsChecked)
            {
                var track = DriverlessTrackManager.GetTrack("Straight");
                if (track != null)
                {
                    var data = ConvertChannelDataToPlotData(track.LeftSide);

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

                    var channelData = CreateOffset(GetChannel("y").ChannelData, (float)GetChannel("c0ref").ChannelData.First());
                    data = ConvertChannelDataToPlotData(HorizontalAxisData.ChannelData.ToArray(), channelData);
                    int dataIndex = (int)DataSlider.Value;
                    double xValue = 0;
                    double yValue = 0;
                    if (dataIndex < HorizontalAxisData.ChannelData.Count)
                    {
                        xValue = channelData[dataIndex];
                        yValue = HorizontalAxisData.ChannelData[dataIndex];
                    }
                    else
                    {
                        xValue = channelData.Last();
                        yValue = HorizontalAxisData.ChannelData.Last();
                    }

                    chart.InitPlot(xValue: xValue,
                                   yValue: yValue,
                                   xAxisValues: data.Item1,
                                   yAxisValues: data.Item2,
                                   vLineColor: Color.LightGreen,
                                   yAxisLabel: "y",
                                   plotVLine: false,
                                   plotHighlightPoint: false,
                                   labelEnabled: true);

                    chart.SetAxisLimitsToAuto();
                    chart.SetFrameBorder(left: false, bottom: false, top: false, right: false);
                    var yawChannelData = GetChannel("yawangle").ChannelData;
                    if (yawChannelData.Count < dataIndex)
                    {
                        chart.PlotImage(xValue, yValue, yawChannelData[dataIndex]);
                    }
                    else
                    {
                        chart.PlotImage(xValue, yValue, yawChannelData.Last());
                    }
                }
                else
                {
                    ShowError.ShowErrorMessage(ref ErrorSnackbar, "Straight track can't be found!");
                }
            }

            return chart;
        }

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
        private Channel GetChannel(string channelName)
        {
            return Channels.Find(x => x.ChannelName.Equals(channelName));
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
        /// Creates the track.
        /// The <i>x</i> axis is the <see cref="HorizontalAxisData"/>.
        /// The <i>y</i> axis is the <b>y</b> Channels <see cref="Channel.ChannelData"/>.
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
        /// <description><see cref="InitChannels()"/></description>
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
        public void AddChannels(List<Channel> channels)
        {
            Channels = channels;
            InitChannels();
            InitGroups();
            SetUpDataSlider();

            // TODO: ha nem kell a highlight karika, akkor a kikommentelt sor kell, az UpdateCharts(); pedig nem
            UpdateCharts();
            //ChangeChartHighlight((int)DataSlider.Value);
        }

        /// <summary>
        /// Sets the <see cref="DataSlider"/>s maximum value based on the <see cref="Channels"/> first <see cref="Channel.ChannelData"/>-s count.
        /// </summary>
        private void SetUpDataSlider()
        {
            DataSlider.Maximum = Channels.First().ChannelData.Count;
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

        private void ChangeChartHighlight(int dataIndex)
        {
            foreach (Chart chart in ChartsStackPanel.Children)
            {
                if (dataIndex < HorizontalAxisData.ChannelData.Count)
                {
                    var channel = GetChannel(chart.ChartName);
                    chart.RenderPlot(xValue: HorizontalAxisData.ChannelData[dataIndex],
                                     yValue: channel.ChannelData[dataIndex],
                                     yAxisLabel: chart.ChartName,
                                     vLineColor: Color.LightGreen);
                }
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
                channel.ChannelIsActive = false;
            }

            foreach (CheckBox checkBox in ChannelsStackPanel.Children)
            {
                checkBox.IsChecked = false;
            }

            UpdateCharts();
        }
    }
}
