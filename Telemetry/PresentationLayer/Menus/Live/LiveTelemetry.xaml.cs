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

namespace PresentationLayer.Menus.Live
{
    public partial class LiveTelemetry : UserControl
    {
        private Section activeSection;
        private bool isActiveSection = false;
        private List<string> channelNames = new List<string>();
        private List<string> activeChannelNames = new List<string>();
        private List<Group> activeGroups = new List<Group>();
        private List<Chart> charts = new List<Chart>();
        private bool canUpdateCharts = false;
        private static HttpClient client = new HttpClient();
        private const int getDataAmount = 5;
        private int yawAngleLastIndex = 0;

        private Package currentPackage = new Package();
        private object getDataLock = new object();
        private AutoResetEvent getDataSignal = new AutoResetEvent(false);

        private const int NO_OPACITY_VALUE = 1;
        private const float LITTLE_OPACITY_VALUE = .2f;

        private int lastPackageID = 0;

        public LiveTelemetry()
        {
            InitializeComponent();

            InitializeGroups();
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

        private void InitializeGroups()
        {
            GroupsStackPanel.Children.Clear();

            foreach (var group in GroupManager.Groups)
            {
                var checkBox = new CheckBox()
                {
                    Content = group.Name
                };
                checkBox.Click += GroupCheckBox_Click;

                GroupsStackPanel.Children.Add(checkBox);
            }
        }

        private void GroupCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string content = checkBox.Content.ToString();
            var group = GroupManager.GetGroup(content);
            if (group == null)
            {
                ShowErrorMessage($"Can't find group '{content}'");
            }

            if ((bool)checkBox.IsChecked)
            {
                activeGroups.Add(group);
            }
            else
            {
                activeGroups.Remove(group);
            }

            InitializeCharts();
        }

        private void InitializeChannels()
        {
            ChannelsStackPanel.Children.Clear();

            foreach (var channelName in channelNames)
            {
                var checkBox = new CheckBox()
                {
                    Content = channelName
                };
                checkBox.Click += ChannelCheckBox_Click;

                ChannelsStackPanel.Children.Add(checkBox);
            }
        }

        private void ChannelCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string content = checkBox.Content.ToString();

            if ((bool)checkBox.IsChecked)
            {
                activeChannelNames.Add(content);
            }
            else
            {
                activeChannelNames.Remove(content);
            }

            InitializeCharts();
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

            this.channelNames = channelNames;
            activeChannelNames.Clear();
            activeGroups.Clear();
            charts.Clear();

            foreach (CheckBox item in GroupsStackPanel.Children)
            {
                item.IsChecked = false;
            }

            yawAngleLastIndex = 0;
            Stop(); //TODO biztos?
                    //  currentPackage.Clear();
            canUpdateCharts = false;

            UpdateSectionTitle();
            InitializeChannels();
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            ChartsStackPanel.Children.Clear();

            foreach (var channelName in activeChannelNames)
            {
                var chart = new Chart(channelName);
                chart.AddChannelName(channelName);
                charts.Add(chart);
                ChartsStackPanel.Children.Add(chart);
            }

            foreach (var group in activeGroups)
            {
                var chart = new Chart(group.Name);
                foreach (var attribute in group.Attributes)
                {
                    chart.AddChannelName(attribute.Name);
                }
                charts.Add(chart);
                ChartsStackPanel.Children.Add(chart);
            }
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

                Thread.Sleep(ConfigurationManager.WaitBetweenCollectData);
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

        private Chart GetChart(string name) => charts.Find(x => x.ChartName.Equals(name));

        private async Task<Package> GetPackageAsync(int packageID)
        {
            try
            {
                var response = await client.GetAsync($"{ConfigurationManager.GetPackageByIDAPICall}{packageID}").ConfigureAwait(false);
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

                /*for (int i = 0; i < data.Count; i++)
                {
                    double actualValue = double.Parse(data[i].value.ToString());
                    string actualDateString = data[i].ellapsedTime.ToString();
                    TimeSpan actualTime = new TimeSpan(long.Parse(actualDateString));
                    returnData.Add(new Tuple<double, TimeSpan>(actualValue, actualTime));
                }*/

                // yawAngleLastIndex += data.Count;

                /* if (yawAngleLastIndex == 0)
                 {
                     yawAngleLastIndex += data.Count;
                 }
                 else
                 {
                     yawAngleLastIndex += bufferAmount;
                 }*/
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
