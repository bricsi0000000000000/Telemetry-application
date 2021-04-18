using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataLayer.Groups;
using DataLayer.Models;
using PresentationLayer.Charts;
using LocigLayer.Groups;
using LocigLayer.Colors;
using LogicLayer.Configurations;
using PresentationLayer.Extensions;
using System.Linq;
using LocigLayer.InputFiles;
using PresentationLayer.Errors;
using System.Drawing;

namespace PresentationLayer.Menus.Live
{
    public partial class LiveTelemetry : UserControl
    {
        private Section activeSection;
        private bool isActiveSection = false;
        private List<Group> activeGroups = new List<Group>();
        private List<Chart> charts = new List<Chart>();
        private bool canUpdateCharts = false;
        private static HttpClient client = new HttpClient();
        private const int getDataAmount = 5;



        private List<Tuple<string, bool>> channelNames = new List<Tuple<string, bool>>();
        private readonly List<string> selectedGroups = new List<string>();
        private readonly List<string> selectedChannels = new List<string>();
        private List<double> horizontalAxisData = new List<double>();

        private Package currentPackage = new Package();
        private readonly object getDataLock = new object();
        private readonly AutoResetEvent getDataSignal = new AutoResetEvent(false);

        private const int NO_OPACITY_VALUE = 1;
        private const float LITTLE_OPACITY_VALUE = .2f;

        private int lastPackageID = 0;

        public LiveTelemetry()
        {
            InitializeComponent();

            InitializeGroupItems(initCall: true);
            UpdateSectionTitle();
            UpdateCanRecieveDataStatus();
            InitilaizeHttpClient();

            UpdateCoverGridsVisibilities();
        }

        private void UpdateCoverGridsVisibilities()
        {
            NoSectionGrid.Visibility = isActiveSection ? Visibility.Hidden : Visibility.Visible;
            NoChannelsGrid.Visibility = isActiveSection ? Visibility.Hidden : Visibility.Visible;
            NoGroupsGrid.Visibility = isActiveSection ? Visibility.Hidden : Visibility.Visible;
            NoChartsGrid.Visibility = isActiveSection ? Visibility.Hidden : Visibility.Visible;
            RecieveDataStatusIcon.Opacity = isActiveSection ? NO_OPACITY_VALUE : LITTLE_OPACITY_VALUE;
        }

        private void InitilaizeHttpClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

            client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1),
                BaseAddress = new Uri(Configurations.Address)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void UpdateSectionTitle()
        {
            if (activeSection == null)
            {
                SectionNameTextBox.Text = "No active section";
            }
            else
            {
                SectionNameTextBox.Text = activeSection.Name;
                SectionDateLabel.Text = activeSection.Date.ToString();
            }
        }

        private void InitializeGroupItems(bool initCall = false)
        {
            GroupsStackPanel.Children.Clear();

            foreach (var group in GroupManager.Groups)
            {
                var checkBox = new CheckBox()
                {
                    Content = group.Name
                };
                checkBox.IsChecked = selectedGroups.Contains(group.Name);
                checkBox.Checked += GroupCheckBox_CheckedClick;
                checkBox.Unchecked += GroupCheckBox_CheckedClick;

                GroupsStackPanel.Children.Add(checkBox);
            }

            if (!initCall)
            {
                BuildCharts();
            }
        }

        private void GroupCheckBox_CheckedClick(object sender, RoutedEventArgs e)
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

            BuildCharts();
        }

        public void BuildCharts()
        {
            ChartsGrid.Children.Clear();
            ChartsGrid.RowDefinitions.Clear();

            int rowIndex = 0;
            foreach (var channelName in channelNames)
            {
                if (selectedChannels.Contains(channelName.Item1))
                {
                    var group = new Group(GroupManager.LastGroupID++, channelName.Item1);

                    group.AddAttribute(InputFileManager.GetLiveFile(activeSection.Name).GetChannel(channelName.Item1));

                    BuildChartGrid(group, ref rowIndex);
                }
            }

            foreach (var group in GroupManager.Groups)
            {
                if (selectedGroups.Contains(group.Name))
                {
                    BuildChartGrid(group, ref rowIndex);
                }
            }

            RowDefinition chartRow = new RowDefinition()
            {
                Height = new GridLength()
            };

            ChartsGrid.RowDefinitions.Add(chartRow);

            Grid.SetRow(new Grid(), rowIndex++);

            UpdateCharts1();
        }

        private void UpdateCharts1()
        {
            if (horizontalAxisData.Count > 0)
            {
                int dataIndex = (int)DataSlider.Value;
                double xValue = dataIndex < horizontalAxisData.Count ? horizontalAxisData[dataIndex] : horizontalAxisData.Last();
                foreach (var item in ChartsGrid.Children)
                {
                    if (item is Chart chart)
                    {
                        chart.UpdateHighlight(xValue);
                        chart.UpdateSideValues(ref dataIndex);
                    }
                }
            }
        }

        private void BuildChartGrid(Group group, ref int rowIndex)
        {
            RowDefinition chartRow = new RowDefinition()
            {
                Height = new GridLength(200)
            };
            RowDefinition gridSplitterRow = new RowDefinition
            {
                Height = new GridLength(5)
            };

            ChartsGrid.RowDefinitions.Add(chartRow);
            ChartsGrid.RowDefinitions.Add(gridSplitterRow);

            GridSplitter splitter = new GridSplitter
            {
                ResizeDirection = GridResizeDirection.Rows,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = ColorManager.Secondary100.ConvertBrush()
            };
            ChartsGrid.Children.Add(splitter);

            var chart = BuildGroupChart(group);
            ChartsGrid.Children.Add(chart);

            Grid.SetRow(chart, rowIndex++);
            Grid.SetRow(splitter, rowIndex++);
        }

        /// <summary>
        /// Builds one <see cref="Chart"/> with the <paramref name="group"/>s values.
        /// </summary>
        /// <param name="group"><see cref="Group"/> that will shown on the <see cref="Chart"/></param>
        /// <returns>A <see cref="Chart"/> with the <paramref name="group"/>s values.</returns>
        private Chart BuildGroupChart(Group group)
        {
            var chart = new Chart(group.Name);

            int dataIndex = (int)DataSlider.Value;

            var addedChannelNames = new List<string>();

            foreach (var channelName in channelNames)
            {
                var attribute = group.GetAttribute(channelName.Item1);
                if (attribute != null)
                {
                    int lineWidth = group.GetAttribute(channelName.Item1).LineWidth;
                    var channel = GetChannel(channelName.Item1);

                    if (channelName.Item2) // aktiv e a channel
                    {
                        var color = group.GetAttribute(channel.Name).ColorText;

                        if (charts.Find(x => x.ChartName.Equals(group.Name)) == null)
                        {
                            charts.Add(chart);
                        }
                        /*var data = GetSensorData(channelName.Item1);
                        if (data.Any())
                        {
                            chart.PlotLive(data: data,
                                           yAxisLabel: channelName.Item1);
                        }*/

                        /*chart.AddPlot(xAxisValues: channelDataPlotData.Item1,
                                      yAxisValues: channel.Data.ToArray(),
                                      lineWidth: lineWidth,
                                      lineColor: ColorTranslator.FromHtml(color),
                                      xAxisLabel: group.HorizontalAxis);

                        chart.AddSideValue(channelName: channelName.Item1,
                                            xAxisValues: channelDataPlotData.Item2,
                                            isActive: true,
                                            inputFileID: inputFile.Item1,
                                            color: color,
                                            lineWidth: lineWidth);*/
                    }
                    else
                    {
                        chart.AddSideValue(channelName: channelName.Item1, xAxisValues: new double[0]/*, inputFileID: inputFile.Item1*/);
                    }

                    chart.AddChannelName(channelName.Item1);

                    addedChannelNames.Add(channelName.Item1);
                }
            }

            foreach (var attribute in group.Attributes)
            {
                if (!addedChannelNames.Contains(attribute.Name))
                {
                    chart.AddChannelName(attribute.Name);
                    chart.AddSideValue(channelName: attribute.Name, xAxisValues: new double[0]);
                }
            }

            if (horizontalAxisData.Count > 0)
            {
                double xValue = dataIndex < horizontalAxisData.Count ? horizontalAxisData[dataIndex] : horizontalAxisData.Last();
                chart.UpdateHighlight(xValue);
            }

            chart.SetAxisLimitsToAuto();

            return chart;
        }

        private Channel GetChannel(string channelName)
        {
            var liveFile = InputFileManager.GetLiveFile(activeSection.Name);
            if (liveFile == null)
            {
                return null;
            }

            return liveFile.GetChannel(channelName);
        }

        private double[] GetSensorData(string sensorName)
        {
            double[] returnData = new double[0];
            switch (sensorName)
            {
                case nameof(Time):
                    returnData = new double[currentPackage.Times.Count];
                    for (int i = 0; i < returnData.Length; i++)
                    {
                        returnData[i] = currentPackage.Times[i].Value;
                    }
                    return returnData;
                case nameof(Speed):
                    returnData = new double[currentPackage.Speeds.Count];
                    for (int i = 0; i < returnData.Length; i++)
                    {
                        returnData[i] = currentPackage.Speeds[i].Value;
                    }
                    return returnData;
            }
            return returnData;
        }

        private void InitializeChannelItems()
        {
            ChannelsStackPanel.Children.Clear();

            foreach (var channelName in channelNames)
            {
                var checkBox = new CheckBox()
                {
                    Content = channelName.Item1
                };
                checkBox.Click += ChannelCheckBox_Click;

                ChannelsStackPanel.Children.Add(checkBox);
            }

            if (selectedChannels.Any())
            {
                BuildCharts();
            }
        }

        private void ChannelCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string content = checkBox.Content.ToString();
            bool isChecked = (bool)checkBox.IsChecked;

            if (isChecked)
            {
                selectedChannels.Add(content);
            }
            else
            {
                selectedChannels.Remove(content);
            }

            int index = channelNames.FindIndex(x => x.Item1.Equals(content));
            channelNames[index] = new Tuple<string, bool>(content, isChecked);

            BuildCharts();
        }

        /// <summary>
        /// Updates the active section to <paramref name="section"/> and initializes the channels and charts.
        /// </summary>
        /// <param name="section"></param>
        public void UpdateSection(Section section, List<string> channelNames)
        {
            activeSection = section;

            isActiveSection = true;

            UpdateCoverGridsVisibilities();

            selectedChannels.Clear();
            activeGroups.Clear();
            charts.Clear();

            foreach (CheckBox item in GroupsStackPanel.Children)
            {
                item.IsChecked = false;
            }

            Stop(); //TODO biztos?
            canUpdateCharts = false;

            UpdateSectionTitle();

            this.channelNames = new List<Tuple<string, bool>>();
            foreach (var name in channelNames)
            {
                this.channelNames.Add(new Tuple<string, bool>(name, false));
            }
            InitializeChannelItems();
        }

        private void CollectData()
        {
            while (canUpdateCharts)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var getPackage = GetPackageAsync(packageID: lastPackageID++).Result;
                stopwatch.Stop();

                if (getPackage != null)
                {
                    lock (getDataLock)
                    {
                        currentPackage = getPackage;
                    }

                    getDataSignal.Set();

                    if (activeSection.IsLive)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MenuManager.LiveSettings.UpdateCarStatus(currentPackage.SentTime, stopwatch.ElapsedMilliseconds);
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            canUpdateCharts = false;
                            UpdateCanRecieveDataStatus();
                        });
                    }
                }

                Thread.Sleep(Configurations.WaitBetweenCollectData);
            }
        }

        private void UpdateCharts()
        {
            while (canUpdateCharts)
            {
                getDataSignal.WaitOne();
                if (canUpdateCharts)
                {
                    var package = new Package();
                    lock (getDataLock)
                    {
                        package = currentPackage;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (Chart chart in charts)
                        {
                            foreach (var name in selectedChannels)
                            {
                                if (chart.HasChannelName(name))
                                {
                                    chart.PlotLive(GetSensorData(name), name);
                                }
                            }
                        }
                        /*var yawAngleChannel = GetChart("yaw_angle");
                        if (yawAngleChannel != null)
                        {
                            var updateableValues = new List<double>();
                            foreach (var item in package)
                            {
                                updateableValues.Add(item.Item1);
                            }
                            yawAngleChannel.Update(updateableValues.ToArray(), "", "yaw angle");
                        }*/
                    });

                    Thread.Sleep(Configurations.WaitBetweenCollectData);
                }
            }
        }

        private Chart GetChart(string name) => charts.Find(x => x.ChartName.Equals(name));

        private async Task<Package> GetPackageAsync(int packageID)
        {
            try
            {
                var response = await client.GetAsync($"{Configurations.GetPackageByIDAPICall}{packageID}").ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                dynamic package = JsonConvert.DeserializeObject(resultString);

                var speeds = new List<Speed>();
                for (int i = 0; i < package.speeds.Count; i++)
                {
                    speeds.Add(new Speed()
                    {
                        ID = package.speeds[i].id,
                        Value = package.speeds[i].value,
                    });
                }

                var times = new List<Time>();
                for (int i = 0; i < package.times.Count; i++)
                {
                    times.Add(new Time()
                    {
                        ID = package.times[i].id,
                        Value = package.times[i].value,
                    });
                }

                return new Package()
                {
                    Speeds = speeds,
                    Times = times,
                    SentTime = TimeSpan.FromTicks((long)package.sentTime)
                };
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowErrorMessage($"Couldn't connect to the sever\t{e.Message}");
                });
            }

            return null;
        }

        /* private Chart BuildChart(string channelName)
         {


             var horizontalAxisData = HorizontalAxisData.Data;

             var data = ConvertChannelDataToPlotData(channel.Data.ToArray(), horizontalAxisData);
             int dataIndex = (int)DataSlider.Value;

             double xValue = dataIndex < horizontalAxisData.Count ? horizontalAxisData[dataIndex] : horizontalAxisData.Last();
             double yValue = dataIndex < channel.Data.Count ? channel.Data[dataIndex] : channel.Data.Last();

             chart.InitPlot(xValue: xValue,
                            yValue: yValue,
                            xAxisValues: data.Item1,
                            yAxisValues: data.Item2,
                            vLineColor: Color.Black,
                            lineColor: ColorTranslator.FromHtml(channel.Color),
                            channels: Channels,
                            dataIndex: dataIndex,
                            yAxisLabel: channel.Name);

             return chart;
         }*/

        private void DataSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error">If true, it's an error message, if not, it's a regular one.</param>
        /// <param name="time"></param>
        private void ShowErrorMessage(string message, bool error = true, double time = 3)
        {
            ErrorSnackbar.Background = error ? ColorManager.Primary900.ConvertBrush() :
                                               ColorManager.Secondary900.ConvertBrush();

            ErrorSnackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }

        private void RecieveDataStatusCard_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isActiveSection)
            {
                RecieveDataStatusCard.Background = ColorManager.Secondary200.ConvertBrush();
            }
        }

        private void RecieveDataStatusCard_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isActiveSection)
            {
                RecieveDataStatusCard.Background = ColorManager.Secondary100.ConvertBrush();

                canUpdateCharts = !canUpdateCharts;
                UpdateCanRecieveDataStatus();

                if (canUpdateCharts)
                {
                    var collectDataThread = new Thread(new ThreadStart(CollectData));
                    collectDataThread.Start();

                    var updateThread = new Thread(new ThreadStart(UpdateCharts));
                    updateThread.Start();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MenuManager.LiveSettings.UpdateCarStatus();
                    });
                }
            }
        }

        private void RecieveDataStatusCard_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isActiveSection)
            {
                RecieveDataStatusCard.Background = ColorManager.Secondary100.ConvertBrush();
            }
        }

        private void RecieveDataStatusCard_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isActiveSection)
            {
                RecieveDataStatusCard.Background = ColorManager.Secondary50.ConvertBrush();
            }
        }

        private void UpdateCanRecieveDataStatus()
        {
            RecieveDataStatusIcon.Kind = canUpdateCharts ? MaterialDesignThemes.Wpf.PackIconKind.Pause :
                                                           MaterialDesignThemes.Wpf.PackIconKind.Play;
        }

        public void Stop()
        {
            canUpdateCharts = false;
        }
    }
}
