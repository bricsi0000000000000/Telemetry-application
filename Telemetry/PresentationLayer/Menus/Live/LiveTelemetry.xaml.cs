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
using System.Windows.Media;
using DataLayer.Groups;
using DataLayer.Models;
using PresentationLayer.Charts;
using PresentationLayer.Menus.Settings.Live;
using LocigLayer.Groups;
using LocigLayer.Texts;
using LocigLayer.Colors;

namespace PresentationLayer.Menus.Live
{
    /// <summary>
    /// Interaction logic for LiveTelemetry.xaml
    /// </summary>
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

        private List<Tuple<double, TimeSpan>> yawAngleData = new List<Tuple<double, TimeSpan>>(); 
        private object getDataLock = new object();
        private AutoResetEvent getDataSignal = new AutoResetEvent(false);

        /// <summary>
        /// In milliseconds
        /// </summary>
        private const int waitBetweenCollectData = 5000;

        public LiveTelemetry()
        {
            InitializeComponent();

            InitializeGroups();
            UpdateSectionTitle();
            UpdateCanRecieveDataStatus();
            InitilaizeHttpClient();
            UpdateNoGrids();
        }

        private void UpdateNoGrids()
        {
            NoSectionGrid.Visibility = isActiveSection ? Visibility.Hidden : Visibility.Visible;
            NoChannelsGrid.Visibility = isActiveSection ? Visibility.Hidden : Visibility.Visible;
            NoGroupsGrid.Visibility = isActiveSection ? Visibility.Hidden : Visibility.Visible;
            NoChartsGrid.Visibility = isActiveSection ? Visibility.Hidden : Visibility.Visible;
            RecieveDataStatusIcon.Opacity = isActiveSection ? 1 : .2f;
        }

        private void InitilaizeHttpClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

            client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1),
                BaseAddress = new Uri("http://192.168.1.33:5000")
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

            UpdateNoGrids();

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
            yawAngleData.Clear();
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

        /// <summary>
        /// Updates all charts with new data.
        /// </summary>
        private void CollectData()
        {
            while (canUpdateCharts)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var getYawAngleData = GetDataAsync(controller: "YawAngle", bufferAmount: getDataAmount).Result;
                stopwatch.Stop();

                lock (getDataLock)
                {
                    yawAngleData = getYawAngleData;
                }

                getDataSignal.Set();

                if (activeSection.IsLive)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var times = new List<TimeSpan>();
                        foreach (var item in getYawAngleData)
                        {
                            times.Add(item.Item2);
                        }
                        ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).UpdateCarStatus(times, stopwatch.ElapsedMilliseconds);
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

                Thread.Sleep(waitBetweenCollectData);
            }
        }

        private void UpdateCharts()
        {
            while (canUpdateCharts)
            {
                getDataSignal.WaitOne();
                if (canUpdateCharts)
                {
                    var values = new List<Tuple<double, TimeSpan>>();
                    lock (getDataLock)
                    {
                        values = yawAngleData;
                    }

                    if (values.Count > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var yawAngleChannel = GetChart("yaw_angle");
                            if (yawAngleChannel != null)
                            {
                                var updateableValues = new List<double>();
                                foreach (var item in values)
                                {
                                    updateableValues.Add(item.Item1);
                                }
                                yawAngleChannel.Update(updateableValues.ToArray(), "", "yaw angle");
                            }
                        });
                    }

                    Thread.Sleep(waitBetweenCollectData);
                }
            }
        }

        private Chart GetChart(string name) => charts.Find(x => x.ChartName.Equals(name));

        private async Task<List<Tuple<double, TimeSpan>>> GetDataAsync(string controller, int bufferAmount)
        {
            var returnData = new List<Tuple<double, TimeSpan>>();

            try
            {
                var response = await client.GetAsync($"/api/{controller}/last_buffer?id={activeSection.ID}&bufferAmount={bufferAmount}&lastIndex={yawAngleLastIndex}").ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                dynamic data = JsonConvert.DeserializeObject(resultString);

                for (int i = 0; i < data.Count; i++)
                {
                    double actualValue = double.Parse(data[i].value.ToString());
                    string actualDateString = data[i].ellapsedTime.ToString();
                    TimeSpan actualTime = new TimeSpan(long.Parse(actualDateString));
                    returnData.Add(new Tuple<double, TimeSpan>(actualValue, actualTime));
                }

                yawAngleLastIndex += data.Count;

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
                    ShowErrorMessage("Couldn't connect to the sever\t" + e.Message);
                });
            }

            return returnData;
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
            ErrorSnackbar.Background = error ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900)) :
                                               new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));

            ErrorSnackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }

        private void RecieveDataStatusCard_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isActiveSection)
            {
                RecieveDataStatusCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary200));
            }
        }

        private void RecieveDataStatusCard_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isActiveSection)
            {
                RecieveDataStatusCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));

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
                        ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).UpdateCarStatus(new List<TimeSpan>(), -1);
                    });
                }
            }
        }

        private void RecieveDataStatusCard_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isActiveSection)
            {
                RecieveDataStatusCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));
            }
        }

        private void RecieveDataStatusCard_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isActiveSection)
            {
                RecieveDataStatusCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
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
