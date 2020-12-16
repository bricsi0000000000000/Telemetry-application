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
    /// This class contains the **main menu** to the driverless section
    /// </summary>

    public partial class DriverlessMenu : UserControl
    {
        private List<Channel> channels = new List<Channel>();

        public DriverlessMenu()
        {
            InitializeComponent();
        }

        private void InitChannels()
        {
            ChannelsStackPanel.Children.Clear();

            foreach (var channel in channels)
            {
                AddChannelCheckBox(channel);
            }
        }

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

        private void ChannelCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            GetChannel(checkBox.Content.ToString()).ChannelIsActive = (bool)checkBox.IsChecked;

            UpdateCharts();
        }

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

        private Chart BuildChart(Channel channel)
        {
            var chart = new Chart(channel.ChannelName, channel.ChannelName);
            var data = ConvertChannelDataToPlotData(channel.ChannelData.ToArray());
            int dataIndex = (int)DataSlider.Value;

            chart.InitPlot(HorizontalAxisData.ChannelData[dataIndex],
                           GetChannel(chart.ChartName).ChannelData[dataIndex],
                           data.Item1,
                           data.Item2,
                           channel.ChannelName);

            return chart;
        }

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

        private Channel HorizontalAxisData => GetChannel("x");

        private Channel GetChannel(string channelName)
        {
            return channels.Find(x => x.ChannelName.Equals(channelName));
        }

        private void ReadFileBtn_Click(object sender, System.Windows.RoutedEventArgs e)
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

        public void UpdateTrack()
        {
            TrackStackPanel.Children.Clear();
            TrackStackPanel.Children.Add(BuildChart(GetChannel("y")));
        }

        public void AddChannels(List<Channel> channels)
        {
            this.channels = channels;
            InitChannels();
            SetUpDataSlider();

            // TODO: ha nem kell a highlight karika, akkor a kikommentelt sor kell, az UpdateCharts(); pedig nem
            UpdateCharts();
            //ChangeChartHighlight((int)DataSlider.Value);
        }

        private void SetUpDataSlider()
        {
            DataSlider.Maximum = channels.First().ChannelData.Count;
        }

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
