using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Charts;
using Telemetry_presentation_layer.Menus.Settings;
using Telemetry_presentation_layer.Menus.Settings.Groups;
using Telemetry_data_and_logic_layer.Tracks;
using Telemetry_presentation_layer.Errors;
using Microsoft.Win32;
using Telemetry_data_and_logic_layer;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Telemetry_data_and_logic_layer.Defaults;

namespace Telemetry_presentation_layer.Menus.Driverless
{
    /// <summary>
    /// Represents the content of the driverless main menu.
    /// </summary>
    public partial class DriverlessMenu : UserControl
    {
        /// <summary>
        /// List of selected <see cref="Group"/>s.
        /// </summary>
        private readonly List<string> selectedGroups = new List<string>();

        private List<string> selectedChannels = new List<string>();
        private List<int> selectedInputFileIDs = new List<int>();

        /// <summary>
        /// Delta time for integrate YawAngle.
        /// 50 ms -> 0.05 sec
        /// </summary>
        private readonly float dt = .05f;

        /// <summary>
        /// The horizontal axis will be based on the <i>x</i> <see cref="Channel"/>.
        /// </summary>
        private Channel HorizontalAxisData(int inputFileID) => GetChannel(inputFileID, DefaultsManager.GetDefault(TextManager.DriverlessHorizontalAxis).Value);

        private readonly List<Tuple<string, List<int>>> ChannelNames = new List<Tuple<string, List<int>>>();

        private TrackChart trackChart;

        /// <summary>
        /// Constructor of the <see cref="DriverlessMenu"/> class.
        /// </summary>
        public DriverlessMenu()
        {
            InitializeComponent();

            InitializeGroupItems();
            InitializeTrackChart();
        }

        private void InitializeTrackChart()
        {
            trackChart = new TrackChart();
            TrackGrid.Children.Add(trackChart);

            var track = DriverlessTrackManager.GetTrack("Straight");
            if (track != null)
            {
                trackChart.AddTrackLayout(ConvertChannelDataToPlotData(track.LeftSide),
                                          ConvertChannelDataToPlotData(track.RightSide),
                                          ConvertChannelDataToPlotData(track.Center));
            }
            else
            {
                throw new Exception("Straight track can't be found!");
            }
        }

        /// <summary>
        /// Initializes <see cref="CheckBox"/>es based on <see cref="GroupManager.Groups"/>.
        /// </summary>
        private void InitializeGroupItems()
        {
            GroupsStackPanel.Children.Clear();

            NoGroupsGrid.Visibility = GroupManager.Groups.Count == 0 ? Visibility.Visible : Visibility.Hidden;

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
        /// Updates <see cref="Group"/>s after the <see cref="CheckBox"/>es state is changed.
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
            ChangeChartHighlight((int)DataSlider.Value);
        }

        /// <summary>
        /// Clears all the children of <see cref="ChannelsStackPanel"/> and fill it with the newly created <see cref="Chart"/>s.
        /// </summary>
        public void UpdateCharts()
        {
            ChartsGrid.Children.Clear();
            ChartsGrid.RowDefinitions.Clear();

            int rowIndex = 0;
            foreach (var channelName in ChannelNames)
            {
                if (selectedChannels.Contains(channelName.Item1))
                {
                    var group = new Group(GroupManager.LastGroupID++, channelName.Item1)
                    {
                        Driverless = true
                    };

                    foreach (var inputFileID in channelName.Item2)
                    {
                        group.AddAttribute(InputFileManager.GetDriverlessInputFile(inputFileID).GetChannel(channelName.Item1));
                    }

                    RowDefinition chartRow = new RowDefinition();
                    RowDefinition gridSplitterRow = new RowDefinition
                    {
                        Height = new GridLength(5)
                    };

                    ChartsGrid.RowDefinitions.Add(chartRow);
                    ChartsGrid.RowDefinitions.Add(gridSplitterRow);

                    GridSplitter splitter = new GridSplitter
                    {
                        ResizeDirection = GridResizeDirection.Rows,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };
                    ChartsGrid.Children.Add(splitter);

                    var chart = BuildGroupChart(group);
                    ChartsGrid.Children.Add(chart);

                    Grid.SetRow(chart, rowIndex++);
                    Grid.SetRow(splitter, rowIndex++);
                }
            }

            /*   foreach (var group in GroupManager.Groups)
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
               }*/
        }

        /// <summary>
        /// Builds one <see cref="Chart"/> with the <paramref name="group"/>s values.
        /// </summary>
        /// <param name="group"><see cref="Group"/> that will shown on the <see cref="Chart"/></param>
        /// <returns>A <see cref="Chart"/> with the <paramref name="group"/>s values.</returns>
        private Chart BuildGroupChart(Group group)
        {
            var chart = new Chart(group.Name);

            foreach (var attribute in group.Attributes)
            {
                chart.AddChannelName(attribute.Name);
            }

            List<double> horizontalAxisData = new List<double>();
            int dataIndex = (int)DataSlider.Value;

            foreach (var channelName in ChannelNames)
            {
                if (chart.HasChannelName(channelName.Item1))
                {
                    foreach (var inputFileID in channelName.Item2)
                    {
                        var actHorizontalAxisData = HorizontalAxisData(inputFileID).Data;
                        if (actHorizontalAxisData.Count > horizontalAxisData.Count)
                        {
                            horizontalAxisData = new List<double>(actHorizontalAxisData);
                        }

                        var channelData = GetChannel(inputFileID, channelName.Item1);

                        var channelDataPlotData = ConvertChannelDataToPlotData(channelData.Data.ToArray(), actHorizontalAxisData);
                        double yValue = dataIndex < channelData.Data.Count ? channelData.Data[dataIndex] : channelData.Data.Last();

                        chart.InitPlot(xAxisValues: channelDataPlotData.Item1,
                                       yAxisValues: channelDataPlotData.Item2,
                                       lineColor: ColorTranslator.FromHtml(channelData.Color),
                                       yAxisLabel: group.Name);


                        chart.UpdateSideValues(inputFileID, channelName.Item1, ref dataIndex);
                    }
                }
            }

            double xValue = dataIndex < horizontalAxisData.Count ? horizontalAxisData[dataIndex] : horizontalAxisData.Last();
            chart.UpdateVLine(xValue, Color.Black);

            chart.SetAxisLimitsToAuto();

            return chart;
        }

        /// <summary>
        /// Pushes the <paramref name="list"/> with <paramref name="offset"/>.
        /// </summary>
        /// <param name="list">List to be shifted.</param>
        /// <param name="offset">Value to be shifted.</param>
        /// <returns>Shifted list based on <paramref name="offset"/>.</returns>
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
        private Tuple<double[], double[]> ConvertChannelDataToPlotData(List<Telemetry_data_and_logic_layer.Point> points)
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
        /// Finds a <see cref="Channel"/> based on <paramref name="channelName"/>.
        /// </summary>
        /// <param name="channelName">The name of the <see cref="Channel"/> to find.</param>
        /// <returns>A single <see cref="Channel"/></returns>
        public Channel GetChannel(int inputFileID, string channelName)
        {
            if (InputFileManager.DriverlessInputFilesCount == 0)
            {
                return null;
            }

            return InputFileManager.GetDriverlessInputFile(inputFileID).GetChannel(channelName);
        }

        /// <summary>
        /// Initializes <see cref="integratedYawAngle"/> list based on <c>yawangle</c> and <c>yawrate</c>.
        /// Formula: <c>yawangle(x) = yawangle(x - 1) + dt * yawrate</c>,
        /// where <c>x</c> is the loop variable and <c>dt</c> ist the timestep in <b>ms</b>.
        /// </summary>
        public double[] GetYawAngle(int inputFileID)
        {
            List<double> integratedYawAngle = new List<double>();

            var yawrate = GetChannel(inputFileID, TextManager.DefaultYawRateChannelName);
            if (yawrate == null)
            {
                throw new Exception($"Can't find '{TextManager.DefaultYawRateChannelName}' channel");
            }

            integratedYawAngle.Add(0);
            for (int i = 1; i < yawrate.Data.Count; i++)
            {
                integratedYawAngle.Add(integratedYawAngle[i - 1] + dt * yawrate.Data[i]);
            }

            for (int i = 0; i < integratedYawAngle.Count; i++)
            {
                integratedYawAngle[i] *= 180 / Math.PI;
            }

            return integratedYawAngle.ToArray();
        }

        /// <summary>
        /// Creates the track.
        /// The <i>x</i> axis is the <see cref="HorizontalAxisData"/>.
        /// The <i>y</i> axis is the <b>y</b> Channels <see cref="Channel.Data"/>.
        /// </summary>
        public void UpdateTrack()
        {
            //TrackGrid.Children.Clear();
            try
            {
                //TrackGrid.Children.Add(BuildTrack());
                trackChart.Update((int)DataSlider.Value);
            }
            catch (Exception e)
            {
                ShowError.ShowErrorMessage(e.Message);
            }
        }

        /// <summary>
        /// Calls
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="UpdateAfterInputFileChoose()"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="InitChooseInputFileComboBox()"/></description>
        /// </item>
        /// </list>
        /// </summary>
        public void UpdateAfterReadFile()
        {
            DisableNoInputFileAndChannelsGrids();
            if (InputFileManager.InputFiles.Count > 0)
            {
                InputFileManager.ActiveDriverlessInputFileID = InputFileManager.GetLastDriverlessInputFile.ID;
                AddInputFileItem(InputFileManager.GetLastDriverlessInputFile.ID, InputFileManager.GetLastDriverlessInputFile.Name);
            }

            // UpdateAfterInputFileChoose();
            // InitChooseInputFileComboBox();
        }

        private void DisableNoInputFileAndChannelsGrids(bool disable = true)
        {
            NoInputFilesGrid.Visibility =
            NoChannelsGrid.Visibility = disable ? Visibility.Hidden : Visibility.Visible;
        }

        private void AddInputFileItem(int inputFileID, string fileName)
        {
            var checkbox = new CheckBox { Content = fileName };
            checkbox.Checked += InputFileItem_Checked;
            checkbox.Unchecked += InputFileItem_Checked;
            InputFilesStackPanel.Children.Add(checkbox);
        }

        public void RemoveInputFileItem(string fileName)
        {
            int index = GetInputFileItemIndex(fileName);
            if (index != -1)
            {
                InputFilesStackPanel.Children.RemoveAt(index);
            }

            if (InputFileManager.DriverlessInputFilesCount <= 0)
            {
                DisableNoInputFileAndChannelsGrids(disable: false);
                ChannelsStackPanel.Children.Clear();
            }
        }

        private int GetInputFileItemIndex(string fileName)
        {
            int index = 0;

            foreach (CheckBox item in InputFilesStackPanel.Children)
            {
                if (item.Content.Equals(fileName))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Sets the <see cref="DataSlider"/>s maximum value based on the <see cref="ChannelNames"/> first <see cref="Channel.Data"/>-s count.
        /// </summary>
        private void SetUpDataSlider()
        {
            int max = 0;
            foreach (var channelName in ChannelNames)
            {
                foreach (var inputFileID in channelName.Item2)
                {
                    if (selectedInputFileIDs.Contains(inputFileID))
                    {
                        var channelDataCount = GetChannel(inputFileID, channelName.Item1).Data.Count;
                        if (channelDataCount > max)
                        {
                            max = channelDataCount;
                        }
                    }
                }
            }

            DataSlider.Maximum = max;
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
            // UpdateTrack();
            //ChangeChartHighlight((int)((Slider)sender).Value);

        }

        /// <summary>
        /// Changes the charts highlights, so the VLines.
        /// </summary>
        /// <param name="dataIndex">Index of the data on the horizontal axis.</param>
        private void ChangeChartHighlight(int dataIndex)
        {
            /*foreach (Chart chart in ChartsStackPanel.Children)
            {
                double value = dataIndex < HorizontalAxisData.Data.Count ? HorizontalAxisData.Data[dataIndex] : HorizontalAxisData.Data.Last();
                chart.RenderPlot(xValue: value,
                                 vLineColor: Color.Black,
                                 Channels,
                                 dataIndex);
            }*/
        }

        /// <summary>
        /// Unselects all <see cref="Channel"/> in <see cref="ChannelNames"/>.
        /// </summary>
        private void UnselectAllChannel()
        {
            /*  if (Channels != null)
              {
                  foreach (var channel in Channels)
                  {
                      channel.IsActive = false;
                  }
              }*/
        }


        private void InputFileItem_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string name = checkBox.Content.ToString();

            var actInputFile = InputFileManager.GetDriverlessInputFile(name);

            if ((bool)checkBox.IsChecked)
            {
                selectedInputFileIDs.Add(actInputFile.ID);

                MergeChannelNames(actInputFile.Channels, actInputFile.ID);

                var yChannel = GetChannel(actInputFile.ID, DefaultsManager.GetDefault(TextManager.DriverlessYChannel).Value);
                if (yChannel == null)
                {
                    ShowError.ShowErrorMessage($"Can't find '{DefaultsManager.GetDefault(TextManager.DriverlessYChannel).Value}', so can't build track");
                    return;
                }

                var c0refChannel = GetChannel(actInputFile.ID, DefaultsManager.GetDefault(TextManager.DriverlessC0refChannel).Value);
                if (c0refChannel == null)
                {
                    ShowError.ShowErrorMessage($"Can't find '{DefaultsManager.GetDefault(TextManager.DriverlessC0refChannel).Value}', so can't build track");
                    return;
                }

                trackChart.AddTrackData(name, HorizontalAxisData(actInputFile.ID).Data.ToArray(), yChannel.Data.ToArray(), GetYawAngle(actInputFile.ID), (float)c0refChannel.Data.First());
            }
            else
            {
                selectedInputFileIDs.Remove(actInputFile.ID);

                UnMergeChannelNames(actInputFile.Channels, actInputFile.ID);
            }

            var removableElements = new List<string>();

            foreach (var selectedChannel in selectedChannels)
            {
                bool found = false;
                int index = 0;
                while (index < ChannelNames.Count && !found)
                {
                    if (selectedChannel.Equals(ChannelNames[index].Item1))
                    {
                        found = true;
                    }

                    index++;
                }

                if (!found)
                {
                    removableElements.Add(selectedChannel);
                }
            }

            foreach (var item in removableElements)
            {
                selectedChannels.Remove(item);
            }

          /*  Trace.WriteLine("###################################");
            foreach (var item in selectedChannels)
            {
                Trace.WriteLine(item);
            }*/

            UpdateChannelsList();
            UpdateCharts();
            SetUpDataSlider();
            /*foreach (var item in Channels)
            {
                Trace.Write(item.Item1.Name + "\t");
                foreach (var i in item.Item2)
                {
                    Trace.Write(i + ";");
                }
                Trace.WriteLine("");
            }*/
        }

        private void ChannelItem_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string name = checkBox.Content.ToString();
            if (name.Contains(" - "))
            {
                name = name.Split(" - ")[0];
            }

            if ((bool)checkBox.IsChecked)
            {
                selectedChannels.Add(name);
            }
            else
            {
                selectedChannels.Remove(name);
            }

            UpdateCharts();
        }

        private void UpdateChannelsList()
        {
            ChannelsStackPanel.Children.Clear();

            int max = 0;

            foreach (var item in ChannelNames)
            {
                if (item.Item2.Count > max)
                {
                    max = item.Item2.Count;
                }
            }

            foreach (var item in ChannelNames)
            {
                var checkBox = new CheckBox
                {
                    IsChecked = selectedChannels.Contains(item.Item1)
                };
                checkBox.Checked += ChannelItem_Checked;
                checkBox.Unchecked += ChannelItem_Checked;
                //TODO drag and drop

                if (item.Item2.Count == max)
                {
                    checkBox.Content = item.Item1;
                }
                else
                {
                    string content = item.Item1 + " - ";
                    foreach (var fileID in item.Item2)
                    {
                        content += InputFileManager.GetInputFile(fileID).Name + ";";
                    }
                    checkBox.Content = content[0..^1];
                }

                ChannelsStackPanel.Children.Add(checkBox);
            }
        }

        private void MergeChannelNames(List<Channel> channels, int inputFileID)
        {
            foreach (var channel in channels)
            {
                var actChannel = ChannelNames.Find(x => x.Item1.Equals(channel.Name));
                if (actChannel != null)
                {
                    actChannel.Item2.Add(inputFileID);
                }
                else
                {
                    ChannelNames.Add(new Tuple<string, List<int>>(channel.Name, new List<int>() { inputFileID }));
                }
            }
        }

        private void UnMergeChannelNames(List<Channel> channels, int inputFileID)
        {
            foreach (var channel in channels)
            {
                var actChannel = ChannelNames.Find(x => x.Item1.Equals(channel.Name));
                if (actChannel != null)
                {
                    actChannel.Item2.Remove(inputFileID);
                    if (actChannel.Item2.Count == 0)
                    {
                        ChannelNames.Remove(actChannel);
                    }
                }
            }

           /* Trace.WriteLine("###################################");

            foreach (var item in ChannelNames)
            {
                Trace.Write(item.Item1 + "\t");
                foreach (var itemi in item.Item2)
                {
                    Trace.Write(itemi + ";");
                }
                Trace.WriteLine("");
            }*/
        }

        private void ReadInputFileBtn_Click(object sender, RoutedEventArgs e)
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
                try
                {
                    string fileName = openFileDialog.FileName.Split('\\').Last();

                    if (InputFileManager.GetInputFile(fileName) == null)
                    {
                        ReadFileProgressBarLabel.Content = $"Reading \"{fileName}\"";
                        var dataReader = new DataReader();
                        dataReader.SetupReader(ReadFileProgressBarGrid,
                                               ReadFileProgressBar,
                                               FileType.Driverless);
                        dataReader.ReadFile(openFileDialog.FileName);
                    }
                    else
                    {
                        throw new Exception($"File '{fileName}' already exists");
                    }
                }
                catch (Exception exception)
                {
                    ShowError.ShowErrorMessage(exception.Message);
                }
            }
        }

    }
}
