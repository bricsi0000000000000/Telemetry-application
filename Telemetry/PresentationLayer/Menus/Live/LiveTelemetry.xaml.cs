using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using DataLayer.Groups;
using DataLayer.Models;
using LogicLayer.Colors;
using LogicLayer.Extensions;
using LogicLayer.Configurations;
using PresentationLayer.Charts;
using PresentationLayer.Groups;
using PresentationLayer.InputFiles;
using PresentationLayer;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using PresentationLayer.RangeSlider;

namespace LogicLayer.Menus.Live
{
    public partial class LiveTelemetry : PlotTelemetry
    {
        private Session activeSession;
        public bool IsSelectedSession { get; set; } = false;

        public bool CanUpdateCharts { get; private set; } = false;
        private static HttpClient client = new HttpClient();

        private List<string> channelNamesFromSession = new List<string>();
        private int lastPackageID = 0;
        private readonly object getDataLock = new object();
        private readonly AutoResetEvent getDataSignal = new AutoResetEvent(false);

        private const int NO_OPACITY_VALUE = 1;
        private const float LITTLE_OPACITY_VALUE = .2f;

        private bool gettingData = false;

        private double rangeSliderDistance = 0;

        #region range slider

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(double), typeof(MainWindow), new PropertyMetadata(0d));
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(double), typeof(MainWindow), new PropertyMetadata(100d));
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(double), typeof(MainWindow), new PropertyMetadata(RangeSlider.MinStartValue));
        public static readonly DependencyProperty EndProperty = DependencyProperty.Register("End", typeof(double), typeof(MainWindow), new PropertyMetadata(RangeSlider.MaxStartValue));

        public double Max
        {
            get => (double)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public double Min
        {
            get => (double)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public double Start
        {
            get => (double)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }

        public double End
        {
            get => (double)GetValue(EndProperty);
            set => SetValue(EndProperty, value);
        }

        #endregion

        public LiveTelemetry()
        {
            InitializeComponent();

            rangeSliderDistance = End - Start;

            InitializeGroupItems(GroupsStackPanel);
            UpdateSessionTitle();
            UpdateCanRecieveDataStatus();
            InitilaizeHttpClient();

            UpdateCoverGridsVisibilities();
        }

        private void UpdateCoverGridsVisibilities()
        {
            NoSessionGrid.Visibility = IsSelectedSession ? Visibility.Hidden : Visibility.Visible;
            NoChannelsGrid.Visibility = IsSelectedSession ? Visibility.Hidden : Visibility.Visible;
            NoGroupsGrid.Visibility = IsSelectedSession ? Visibility.Hidden : Visibility.Visible;
            NoChartsGrid.Visibility = IsSelectedSession ? Visibility.Hidden : Visibility.Visible;
            RecieveDataStatusIcon.Opacity = IsSelectedSession ? NO_OPACITY_VALUE : LITTLE_OPACITY_VALUE;
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

        private void UpdateSessionTitle()
        {
            if (activeSession == null)
            {
                SessionNameTextBox.Text = "No active session";
            }
            else
            {
                SessionNameTextBox.Text = activeSession.Name;
                SessionDateLabel.Text = activeSession.Date.ToString();
            }
        }

        public override void BuildCharts()
        {
            ChartsGrid.Children.Clear();
            ChartsGrid.RowDefinitions.Clear();

            int rowIndex = 0;
            foreach (var channelName in selectedChannels) // basic single chart
            {
                var group = new Group(GroupManager.LastGroupID++, channelName);

                group.AddAttribute(InputFileManager.GetLiveFile(activeSession.ID).GetChannel(channelName));

                BuildChartGrid(group, ref rowIndex, ref ChartsGrid);
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

            RefreshCharts();
        }

        /// <summary>
        /// Builds one <see cref="Chart"/> with the <paramref name="group"/>s values.
        /// </summary>
        /// <param name="group"><see cref="Group"/> that will shown on the <see cref="Chart"/></param>
        /// <returns>A <see cref="Chart"/> with the <paramref name="group"/>s values.</returns>
        protected override Chart BuildGroupChart(Group group)
        {
            var chart = new Chart(group.Name);

            foreach (var attribute in group.Attributes)
            {
                chart.AddChannelName(attribute.Name);
                var channel = GetChannel(attribute.Name);
                double[] values;
                if (channel != null)
                {
                    values = channel.Data.ToArray();
                    if (values.Any())
                    {
                        double renderRate = values.Length / (double)MaxProperty.DefaultMetadata.DefaultValue;

                        int minRenderIndex = (int)(Start * renderRate);
                        double end = End;
                        if (end - Start > Start + rangeSliderDistance)
                        {
                            end = Start + rangeSliderDistance;
                        }
                        int maxRenderIndex = (int)(end * renderRate);

                        chart.AddLivePlot(xAxisValues: values,
                                          lineColor: attribute.ColorText.ConvertToChartColor(),
                                          lineWidth: 1,
                                          xAxisLabel: "",
                                          minRenderIndex,
                                          maxRenderIndex);
                    }
                }
                else
                {
                    values = new double[0];
                }

                chart.AddSideValue(attribute.Name, values, color: attribute.ColorText, isActive: values.Any());
                //chart.UpdateLiveSideValue();
            }

            chart.SetAxisLimitsToAuto();

            return chart;
        }

        public override Channel GetChannel(string channelName, int? inputFileID = null)
        {
            var liveFile = InputFileManager.GetLiveFile(activeSession.ID);
            if (liveFile == null)
            {
                return null;
            }

            return liveFile.GetChannel(channelName);
        }

        private double[] GetSensorData(int packageID, string sensorName, List<Package> packages)
        {
            var package = packages.Find(x => x.ID == packageID);
            double[] returnData = new double[0];
            switch (sensorName)
            {
                case nameof(Time):
                    if (package.TimeValues != null)
                    {
                        returnData = new double[package.TimeValues.Count];
                        for (int i = 0; i < returnData.Length; i++)
                        {
                            returnData[i] = package.TimeValues[i].Value;
                        }
                    }
                    return returnData;
                case nameof(Speed):
                    if (package.SpeedValues != null)
                    {
                        returnData = new double[package.SpeedValues.Count];
                        for (int i = 0; i < returnData.Length; i++)
                        {
                            returnData[i] = package.SpeedValues[i].Value;
                        }
                    }
                    return returnData;
                case nameof(Yaw):
                    if (package.YawValues != null)
                    {
                        returnData = new double[package.YawValues.Count];
                        for (int i = 0; i < returnData.Length; i++)
                        {
                            returnData[i] = package.YawValues[i].Value;
                        }
                    }
                    return returnData;
            }
            return returnData;
        }

        protected override void UpdateChannelsList()
        {
            ChannelsStackPanel.Children.Clear();

            foreach (var channelName in channelNamesFromSession)
            {
                var checkBox = new CheckBox()
                {
                    Content = channelName
                };
                checkBox.Checked += ChannelCheckBox_Checked;
                checkBox.Unchecked += ChannelCheckBox_Checked;
                checkBox.PreviewMouseRightButtonDown += ChannelCheckboc_PreviewMouseRightButtonDown;
                checkBox.MouseEnter += CheckBox_MouseEnter;
                checkBox.MouseLeave += CheckBox_MouseLeave;

                ChannelsStackPanel.Children.Add(checkBox);
            }
        }

        private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void CheckBox_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void ChannelCheckboc_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string channelName = "";

            foreach (CheckBox item in ChannelsStackPanel.Children)
            {
                if (item.Content.Equals(checkBox.Content))
                {
                    channelName = item.Content.ToString();
                }
            }

            if (!channelName.Equals(string.Empty))
            {
                DragDrop.DoDragDrop(checkBox, channelName, DragDropEffects.Move);
            }
        }

        private void ChannelCheckBox_Checked(object sender, RoutedEventArgs e)
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

            BuildCharts();
        }

        public void ReplaceChannelWithTemporaryGroup(string channelName, string groupName)
        {
            foreach (CheckBox item in ChannelsStackPanel.Children)
            {
                if (item.Content.ToString().Equals(channelName))
                {
                    item.IsChecked = false;
                }
            }

            InitializeGroupItems();

            foreach (CheckBox item in GroupsStackPanel.Children)
            {
                if (item.Content.ToString().Equals(groupName))
                {
                    item.IsChecked = true;
                }
            }
        }

        public override void InitializeGroupItems(StackPanel groupsStackPanel = null)
        {
            NoGroupsGrid.Visibility = GroupManager.Groups.Count == 0 ? Visibility.Visible : Visibility.Hidden;

            base.InitializeGroupItems(GroupsStackPanel);
        }

        protected override void GroupCheckBox_CheckedClick(object sender, RoutedEventArgs e)
        {
            base.GroupCheckBox_CheckedClick(sender, e);

            BuildCharts();
        }


        /// <summary>
        /// Updates the active session to <paramref name="session"/> and initializes the channels and charts.
        /// </summary>
        /// <param name="session"></param>
        public void UpdateSession(Session session, List<string> channelNames)
        {
            activeSession = session;

            ChangeSessionStatusIconState(session.IsLive);

            IsSelectedSession = true;

            UpdateCoverGridsVisibilities();

            ResetCharts();

            CanUpdateCharts = false;

            UpdateSessionTitle();

            channelNamesFromSession = new List<string>(channelNames);

            UpdateChannelsList();

            lastPackageID = 0;

            SetUpDataSlider();
        }

        public void ChangeSessionStatusIconState(bool isLive)
        {
            SessionStatusIcon.Kind = isLive ? PackIconKind.AccessPoint : PackIconKind.AccessPointOff;
            SessionStatusIcon.Foreground = isLive ? ColorManager.Secondary900.ConvertBrush() :
                                                                  ColorManager.Primary900.ConvertBrush();

            activeSession.IsLive = isLive;

            UpdateCanRecieveDataStatus();
        }

        public void ResetCharts()
        {
            selectedChannels.Clear();
            selectedGroups.Clear();

            foreach (CheckBox item in GroupsStackPanel.Children)
            {
                item.IsChecked = false;
            }

            ChartsGrid.Children.Clear();
        }

        private void CollectData()
        {
            while (CanUpdateCharts)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var packages = new List<Package>();

                // if lastPackageID is 0, so it's the first call, then get all packages and continue getting from that
                packages = GetPackagesAsync(lastPackageID, all: lastPackageID == 0).Result;
                stopwatch.Stop();

                if (packages != null && packages.Any())
                {
                    foreach (var package in packages)
                    {
                        foreach (var channel in InputFileManager.GetLiveFile(activeSession.ID).Channels)
                        {
                            var sensorData = GetSensorData(packageID: package.ID, sensorName: channel.Name, packages: packages);
                            if (sensorData.Any())
                            {
                                InputFileManager.AddDataToLiveFile(fileID: activeSession.ID, channelName: channel.Name, values: sensorData);
                            }
                        }
                    }

                    lastPackageID = packages.Last().ID;

                    getDataSignal.Set();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        BuildCharts();
                        SetUpDataSlider();
                        MenuManager.LiveSettings.ChangeLoadedPackagesLabel(lastPackageID);
                    });

                    if (activeSession.IsLive)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MenuManager.LiveSettings.UpdateCarStatus(packages.First().SentTime, stopwatch.ElapsedMilliseconds);
                        });
                    }
                }

                if (!activeSession.IsLive)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        gettingData = false;
                        CanUpdateCharts = false;
                        UpdateCanRecieveDataStatus();
                    });
                }

                Thread.Sleep(ConfigurationManager.WaitBetweenCollectData);
            }
        }

        private void SetUpDataSlider()
        {
            DataSlider.Maximum = MaxDataCount;
            DataSlider.Value = MaxDataCount;
        }

        private int MaxDataCount
        {
            get
            {
                var file = InputFileManager.GetLiveFile(activeSession.ID);
                if (file == null)
                {
                    return 0;
                }
                var channels = file.Channels;
                return channels == null ? 0 : channels.Max(x => x.Data.Count);
            }
        }

        private Chart GetChart(string name)
        {
            foreach (var item in ChartsGrid.Children)
            {
                if (item is Chart chart)
                {
                    if (chart.ChartName.Equals(name))
                    {
                        return chart;
                    }
                }
            }

            return null;
        }

        private async Task<List<Package>> GetPackagesAsync(int lastPackageID, bool all = false)
        {
            try
            {
                string apiCall;
                if (all)
                {
                    apiCall = $"{ConfigurationManager.GetAllPackages_APICall}/{activeSession.ID}";
                }
                else
                {
                    apiCall = $"{ConfigurationManager.GetPackagesAfter_APICall}/{lastPackageID}/{activeSession.ID}";
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
                        var speedValues = new List<Speed>();
                        for (int i = 0; i < packages[packageIndex].speedValues.Count; i++)
                        {
                            speedValues.Add(new Speed()
                            {
                                ID = packages[packageIndex].speedValues[i].id,
                                Value = packages[packageIndex].speedValues[i].value,
                            });
                        }

                        var timeValues = new List<Time>();
                        for (int i = 0; i < packages[packageIndex].timeValues.Count; i++)
                        {
                            timeValues.Add(new Time()
                            {
                                ID = packages[packageIndex].timeValues[i].id,
                                Value = packages[packageIndex].timeValues[i].value,
                            });
                        }

                        var yawValues = new List<Yaw>();
                        for (int i = 0; i < packages[packageIndex].yawValues.Count; i++)
                        {
                            yawValues.Add(new Yaw()
                            {
                                ID = packages[packageIndex].yawValues[i].id,
                                Value = packages[packageIndex].yawValues[i].value,
                            });
                        }

                        returnPackages.Add(new Package()
                        {
                            ID = packages[packageIndex].id,
                            SpeedValues = speedValues,
                            TimeValues = timeValues,
                            YawValues = yawValues,
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

        private void ShowErrorMessage(string message, bool error = true, double time = 3)
        {
            ErrorSnackbar.Background = error ? ColorManager.Primary900.ConvertBrush() :
                                               ColorManager.Secondary900.ConvertBrush();

            ErrorSnackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }

        private void RecieveDataStatusCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!gettingData)
            {
                if (IsSelectedSession)
                {
                    RecieveDataStatusCard.Background = ColorManager.Secondary200.ConvertBrush();
                }
            }
        }

        private void RecieveDataStatusCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!gettingData)
            {
                if (IsSelectedSession)
                {
                    RecieveDataStatusCard.Background = ColorManager.Secondary100.ConvertBrush();

                    if (activeSession.IsLive)
                    {
                        CanUpdateCharts = !CanUpdateCharts;
                    }
                    else
                    {
                        gettingData = true;
                        CanUpdateCharts = true;
                    }

                    if (CanUpdateCharts)
                    {
                        var collectDataThread = new Thread(new ThreadStart(CollectData));
                        collectDataThread.Start();
                    }
                    else
                    {
                        InputFileManager.SaveFile(activeSession.Name);
                        MenuManager.LiveSettings.UpdateCarStatus();
                    }

                    UpdateCanRecieveDataStatus();
                }
            }
        }

        private void RecieveDataStatusCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!gettingData)
            {
                if (IsSelectedSession)
                {
                    RecieveDataStatusCard.Background = ColorManager.Secondary100.ConvertBrush();
                }
            }
        }

        private void RecieveDataStatusCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!gettingData)
            {
                if (IsSelectedSession)
                {
                    RecieveDataStatusCard.Background = ColorManager.Secondary50.ConvertBrush();
                }
            }
        }

        private void UpdateCanRecieveDataStatus()
        {
            if (activeSession != null)
            {
                if (activeSession.IsLive)
                {
                    RecieveDataStatusIcon.Kind = CanUpdateCharts ? PackIconKind.Pause :
                                                                   PackIconKind.Play;
                }
                else
                {
                    RecieveDataStatusIcon.Kind = PackIconKind.TrayArrowDown;
                    RecieveDataStatusIcon.Opacity = gettingData ? LITTLE_OPACITY_VALUE : NO_OPACITY_VALUE;
                    Mouse.OverrideCursor = gettingData ? Cursors.Wait : null;
                }
            }
        }

        public void UpdateRangeSlider()
        {
            rangeSliderDistance = End - Start;
            BuildCharts();
        }

        private void RefreshCharts()
        {
            int dataIndex = (int)DataSlider.Value;
            foreach (var item in ChartsGrid.Children)
            {
                if (item is Chart chart)
                {
                    chart.UpdateLiveHighlight(ref dataIndex);
                    chart.UpdateLiveSideValue(ref dataIndex);
                }
            }
        }

        private void DataSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshCharts();
        }
    }
}
