using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.Datas.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        private List<Channel> channels = new List<Channel>();

        /// <summary>
        /// Constructor of the <see cref="DriverlessMenu"/> class.
        /// </summary>
        public DriverlessMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the channels based on <see cref="channels"/>.
        /// </summary>
        private void InitChannels()
        {
            ChannelsStackPanel.Children.Clear();

            foreach (var channel in channels)
            {
                AddChannelCheckBox(channel);
            }
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
        /// Updates the Charts after a <see cref="CheckBox"/>-es state is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            GetChannel(checkBox.Content.ToString()).ChannelIsActive = (bool)checkBox.IsChecked;

            UpdateCharts();
        }

        /// <summary>
        /// Clears all the children of <see cref="ChannelsStackPanel"/> and fill it with the newly created Charts.
        /// </summary>
        private void UpdateCharts()
        {
            ChartsStackPanel.Children.Clear();
            foreach (var channel in channels)
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
            var data = ConvertChannelDataToPlotData(channel.ChannelData.ToArray());
            int dataIndex = (int)DataSlider.Value;

            if (dataIndex < HorizontalAxisData.ChannelData.Count)
            {
                chart.InitPlot(HorizontalAxisData.ChannelData[dataIndex],
                               channel.ChannelData[dataIndex],
                               data.Item1,
                               data.Item2,
                               channel.ChannelName);
            }
            else
            {
                chart.InitPlot(HorizontalAxisData.ChannelData.Last(),
                               channel.ChannelData.Last(),
                               data.Item1,
                               data.Item2,
                               channel.ChannelName);
            }

            return chart;
        }

        private Chart BuildTrack()
        {
            var chart = new Chart("TrackChart", "track");
            

            return chart;
        }

        /// <summary>
        /// Converts all the data from a <see cref="Channel"/> to plot data.
        /// The horizontal <i>(x)</i> axis is based on <see cref="HorizontalAxisData"/>.
        /// </summary>
        /// <param name="lapData">Data from a single <see cref="Channel"/> converted to a double array.</param>
        /// <returns>Horizontal and vertical axes data.</returns>
        private Tuple<double[], double[]> ConvertChannelDataToPlotData(double[] lapData)
        {
            List<double> x = new List<double>();
            List<double> y = new List<double>();

            for (ushort i = 0; i < lapData.Length; i++)
            {
                x.Add(HorizontalAxisData.ChannelData[i]);
                y.Add(lapData[i]);
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
            return channels.Find(x => x.ChannelName.Equals(channelName));
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
        public void CreateTrack()
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
            this.channels = channels;
            InitChannels();
            SetUpDataSlider();

            // TODO: ha nem kell a highlight karika, akkor a kikommentelt sor kell, az UpdateCharts(); pedig nem
            UpdateCharts();
            //ChangeChartHighlight((int)DataSlider.Value);
        }

        /// <summary>
        /// Sets the <see cref="DataSlider"/>s maximum value based on the <see cref="channels"/> first <see cref="Channel.ChannelData"/>-s count.
        /// </summary>
        private void SetUpDataSlider()
        {
            DataSlider.Maximum = channels.First().ChannelData.Count;
        }

        /// <summary>
        /// Updates the Charts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // TODO: ha nem kell a highlight karika, akkor a kikommentelt sor kell, az UpdateCharts(); pedig nem
            UpdateCharts();
            //ChangeChartHighlight((int)((Slider)sender).Value);
        }

       /* private void ChangeChartHighlight(int dataIndex)
        {
            foreach (Chart chart in ChartsStackPanel.Children)
            {
                if (dataIndex <= HorizontalAxisData.ChannelData.Count)
                {
                    chart.RenderPlot(HorizontalAxisData.ChannelData[dataIndex],
                                     GetChannel(chart.ChartName).ChannelData[dataIndex]);
                }
            }
        }*/

        /// <summary>
        /// Unchecks all the checked Checkboxes inside the <see cref="ChannelsStackPanel"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UncheckAllChannels_Click(object sender, RoutedEventArgs e)
        {
            foreach (var channel in channels)
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
