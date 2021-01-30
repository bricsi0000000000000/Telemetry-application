using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.Models;
using Telemetry_presentation_layer.Charts;

namespace Telemetry_presentation_layer.Menus.Live
{
    /// <summary>
    /// Interaction logic for LiveTelemetry.xaml
    /// </summary>
    public partial class LiveTelemetry : UserControl
    {
        private Section activeSection;
        private List<string> channelNames = new List<string>();
        private List<string> activeChannelNames = new List<string>();
        private List<Group> activeGroups = new List<Group>();
        private List<Chart> charts = new List<Chart>();
        private bool canUpdateCharts = false;
        private static HttpClient client = new HttpClient();
        private const int getDataAmount = 5;

        private Queue<double> yawAngleData = new Queue<double>();
        private int yawAngleLastIndex = 0;
        private object getDataLock = new object();
        private AutoResetEvent getDataSignal = new AutoResetEvent(false);

        /// <summary>
        /// In milliseconds
        /// </summary>
        private const int waitBetweenCollectData = 700;

        public LiveTelemetry()
        {
            InitializeComponent();

            InitializeGroups();
            UpdateSectionTitle();
            UpdateCanRecieveDataStatus();
            InitilaizeHttpClient();
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
            this.channelNames = channelNames;
            activeChannelNames.Clear();
            activeGroups.Clear();
            charts.Clear();

            foreach (CheckBox item in GroupsStackPanel.Children)
            {
                item.IsChecked = false;
            }

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
                var getYawAngleData = GetDataAsync(controller: "YawAngle", amount: getDataAmount).Result;

                for (int i = 0; i < getYawAngleData.Count; i++)
                {
                    lock (getDataLock)
                    {
                        yawAngleData.Enqueue(getYawAngleData[i]);
                    }
                }

                getDataSignal.Set();

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
                    var values = new List<double>();
                    while (yawAngleData.Count > 0)
                    {
                        lock (getDataLock)
                        {
                            values.Add(yawAngleData.Dequeue());
                        }
                    }
                    if (values.Count > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            GetChart("yaw_angle").Update(values.ToArray());
                        });
                    }

                    Thread.Sleep(waitBetweenCollectData);
                }
            }
        }

        private Chart GetChart(string name) => charts.Find(x => x.ChartName.Equals(name));

        private async Task<List<double>> GetDataAsync(string controller, int amount)
        {
            var returnData = new List<double>();

            try
            {
                var response = await client.GetAsync($"/api/{controller}/last_buffer?amount={amount}&lastIndex={yawAngleLastIndex}").ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                dynamic data = JsonConvert.DeserializeObject(resultString);
                for (int i = 0; i < data.Count; i++)
                {
                    returnData.Add(double.Parse(data[i].ToString()));
                }

                yawAngleLastIndex += amount;
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowErrorMessage("Couldn't connect to the sever");
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
            RecieveDataStatusCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary200));
        }

        private void RecieveDataStatusCard_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RecieveDataStatusCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));

            canUpdateCharts = !canUpdateCharts;
            UpdateCanRecieveDataStatus();

            if (canUpdateCharts)
            {
                var t2 = new Thread(new ThreadStart(CollectData));
                t2.Start();

                var updateThread = new Thread(new ThreadStart(UpdateCharts));
                updateThread.Start();
            }
        }

        private void RecieveDataStatusCard_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RecieveDataStatusCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));
        }

        private void RecieveDataStatusCard_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RecieveDataStatusCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
        }

        private void UpdateCanRecieveDataStatus()
        {
            RecieveDataStatusIcon.Foreground = canUpdateCharts ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900)) :
                                                                new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
            RecieveDataStatusIcon.Kind = canUpdateCharts ? MaterialDesignThemes.Wpf.PackIconKind.DatabaseImport :
                                                           MaterialDesignThemes.Wpf.PackIconKind.DatabaseRemove;
        }

        public void Stop()
        {
            canUpdateCharts = false;
        }
    }
}
