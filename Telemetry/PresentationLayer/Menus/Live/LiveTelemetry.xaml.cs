﻿using Newtonsoft.Json;
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
using PresentationLayer.Groups;
using LogicLayer.Colors;
using System.Linq;
using PresentationLayer.InputFiles;
using LogicLayer.Extensions;
using LogicLayer.Configurations;
using PresentationLayer;

namespace LogicLayer.Menus.Live
{
    public partial class LiveTelemetry : PlotTelemetry
    {
        private Section activeSection;
        private bool isActiveSection = false; // TODO never set to false, why?

        private readonly List<Chart> charts = new List<Chart>();
        private bool canUpdateCharts = false;
        private static HttpClient client = new HttpClient();

        /// <summary>
        /// Item1: name
        /// Item2: is active
        /// </summary>
        private List<Tuple<string, bool>> channelNames = new List<Tuple<string, bool>>();

        private List<Package> currentPackages = new List<Package>();
        private int lastPackageID = 0;
        private readonly object getDataLock = new object();
        private readonly AutoResetEvent getDataSignal = new AutoResetEvent(false);

        private const int NO_OPACITY_VALUE = 1;
        private const float LITTLE_OPACITY_VALUE = .2f;

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
                BaseAddress = new Uri(ConfigurationManager.Address)
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
            base.InitializeGroupItems(GroupsStackPanel);

            if (!initCall)
            {
                BuildCharts();
            }
        }

        public override void BuildCharts()
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

                    BuildChartGrid(group, ref rowIndex, ref ChartsGrid);
                }
            }

            foreach (var group in GroupManager.Groups)
            {
                if (selectedGroups.Contains(group.Name))
                {
                    BuildChartGrid(group, ref rowIndex, ref ChartsGrid);
                }
            }

            RowDefinition chartRow = new RowDefinition()
            {
                Height = new GridLength()
            };

            ChartsGrid.RowDefinitions.Add(chartRow);

            Grid.SetRow(new Grid(), rowIndex++);

            // RefreshCharts();
        }

        protected override void RefreshCharts()
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

        /// <summary>
        /// Builds one <see cref="Chart"/> with the <paramref name="group"/>s values.
        /// </summary>
        /// <param name="group"><see cref="Group"/> that will shown on the <see cref="Chart"/></param>
        /// <returns>A <see cref="Chart"/> with the <paramref name="group"/>s values.</returns>
        protected override Chart BuildGroupChart(Group group)
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

        public override Channel GetChannel(string channelName, int? inputFileID = null)
        {
            var liveFile = InputFileManager.GetLiveFile(activeSection.Name);
            if (liveFile == null)
            {
                return null;
            }

            return liveFile.GetChannel(channelName);
        }

        private double[] GetSensorData(int packageID, string sensorName)
        {
            var package = currentPackages.Find(x => x.ID == packageID);
            double[] returnData = new double[0];
            switch (sensorName)
            {
                case nameof(Time):
                    if (package.Times != null)
                    {
                        returnData = new double[package.Times.Count];
                        for (int i = 0; i < returnData.Length; i++)
                        {
                            returnData[i] = package.Times[i].Value;
                        }
                    }
                    return returnData;
                case nameof(Speed):
                    if (package.Speeds != null)
                    {
                        returnData = new double[package.Speeds.Count];
                        for (int i = 0; i < returnData.Length; i++)
                        {
                            returnData[i] = package.Speeds[i].Value;
                        }
                    }
                    return returnData;
            }
            return returnData;
        }

        protected override void UpdateChannelsList()
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
            selectedGroups.Clear();
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
            UpdateChannelsList();
        }

        private void CollectData()
        {
            while (canUpdateCharts)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var getPackages = new List<Package>();

                // if lastPackageID is 0, so it's the first call, then get all packages and continue getting from that
                getPackages = GetPackagesAsync(lastPackageID, all: lastPackageID == 0).Result;

                stopwatch.Stop();

                if (getPackages != null && getPackages.Any())
                {
                    lastPackageID = getPackages.Last().ID;

                    lock (getDataLock)
                    {
                        currentPackages = getPackages;
                    }

                    getDataSignal.Set();

                    if (activeSection.IsLive)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MenuManager.LiveSettings.UpdateCarStatus(currentPackages.First().SentTime, stopwatch.ElapsedMilliseconds);
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

                Thread.Sleep(ConfigurationManager.WaitBetweenCollectData);
            }
        }

        /// <summary>
        /// Updates charts with new incoming data
        /// </summary>
        private void UpdateCharts()
        {
            while (canUpdateCharts)
            {
                getDataSignal.WaitOne();
                if (canUpdateCharts)
                {
                    var packages = new List<Package>();
                    lock (getDataLock)
                    {
                        packages = currentPackages;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Plot(ref packages);
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

                    Thread.Sleep(ConfigurationManager.WaitBetweenCollectData);
                }
            }
        }

        private void Plot(ref List<Package> packages)
        {
            foreach (Chart chart in charts)
            {
                foreach (var name in selectedChannels)
                {
                    if (chart.HasChannelName(name))
                    {
                        foreach (var package in packages)
                        {
                            var sensorData = GetSensorData(packageID: package.ID, sensorName: name);
                            if (sensorData.Length > 0)
                            {
                                InputFileManager.AddDataToLiveFile(liveFileName: activeSection.Name, channelName: name, values: sensorData);

                                //chart.PlotLive(InputFileManager.GetLiveFile(activeSection.Name).GetChannel(name).Data.ToArray(), name);
                                chart.PlotLive(sensorData, name);
                            }
                        }
                    }
                }
            }
        }

        private Chart GetChart(string name) => charts.Find(x => x.ChartName.Equals(name));

        private async Task<List<Package>> GetPackagesAsync(int lastPackageID, bool all = false)
        {
            try
            {
                string apiCall;
                if (all)
                {
                    apiCall = $"{ConfigurationManager.GetAllPackages_APICall}/{activeSection.ID}";
                }
                else
                {
                    apiCall = $"{ConfigurationManager.GetPackagesAfter_APICall}/{lastPackageID}/{activeSection.ID}";
                }

                var response = await client.GetAsync(apiCall).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                dynamic packages = JsonConvert.DeserializeObject(resultString);

                var returnPackages = new List<Package>();

                if (packages != null)
                {
                    for (int packageIndex = 0; packageIndex < packages.Count; packageIndex++)
                    {
                        var speeds = new List<Speed>();
                        for (int i = 0; i < packages[packageIndex].speeds.Count; i++)
                        {
                            speeds.Add(new Speed()
                            {
                                ID = packages[packageIndex].speeds[i].id,
                                Value = packages[packageIndex].speeds[i].value,
                            });
                        }

                        var times = new List<Time>();
                        for (int i = 0; i < packages[packageIndex].times.Count; i++)
                        {
                            times.Add(new Time()
                            {
                                ID = packages[packageIndex].times[i].id,
                                Value = packages[packageIndex].times[i].value,
                            });
                        }

                        returnPackages.Add(new Package()
                        {
                            ID = packages[packageIndex].id,
                            Speeds = speeds,
                            Times = times,
                            SentTime = TimeSpan.FromTicks((long)packages[packageIndex].sentTime)
                        });
                    }
                }

                return returnPackages;
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
                    InputFileManager.SaveFile(activeSection.Name);

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
