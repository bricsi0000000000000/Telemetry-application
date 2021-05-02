using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataLayer.Groups;
using PresentationLayer.Charts;
using DataLayer.Tracks;
using Microsoft.Win32;
using DataLayer;
using System.Windows.Input;
using PresentationLayer.Texts;
using PresentationLayer.Tracks;
using PresentationLayer.Groups;
using PresentationLayer.InputFiles;
using LogicLayer.Errors;
using LogicLayer;
using PresentationLayer.Defaults;

namespace PresentationLayer.Menus.Driverless
{
    public partial class DriverlessMenu : PlotTelemetry
    {
        /// <summary>
        /// List of selected <see cref="Group"/>s.
        /// </summary>

        private List<int> selectedInputFileIDs = new List<int>();

        private readonly List<Tuple<string, List<Tuple<int, bool>>>> channelNames = new List<Tuple<string, List<Tuple<int, bool>>>>();

        private TrackChart trackChart;


        public DriverlessMenu()
        {
            InitializeComponent();

            Init(GroupsStackPanel);
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

        protected override void GroupCheckBox_CheckedClick(object sender, RoutedEventArgs e)
        {
            base.GroupCheckBox_CheckedClick(sender, e);

            BuildCharts();
        }

        /// <summary>
        /// Initializes <see cref="CheckBox"/>es based on <see cref="GroupManager.Groups"/>.
        /// </summary>
        public override void InitializeGroupItems(StackPanel groupsStackPanel = null)
        {
            NoGroupsGrid.Visibility = GroupManager.Groups.Count == 0 ? Visibility.Visible : Visibility.Hidden;

            base.InitializeGroupItems(GroupsStackPanel);

            BuildCharts();
        }

        public void AddRenamedGroupToSelectedGroups(string groupName)
        {
            selectedGroups.Add(groupName);

            InitializeGroupItems();
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

                    foreach (var inputFile in channelName.Item2)
                    {
                        group.AddAttribute(InputFileManager.GetDriverlessFile(inputFile.Item1).GetChannel(channelName.Item1));
                    }

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

            RefreshCharts();
        }

        private void RefreshCharts()
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
                    foreach (var inputFile in channelName.Item2)
                    {
                        int lineWidth = group.GetAttribute(channelName.Item1).LineWidth;
                        var channel = GetChannel(channelName: channelName.Item1, inputFileID: inputFile.Item1);
                        var color = group.GetAttribute(channel.Name).ColorText;

                        if (inputFile.Item2) // aktiv e a channel
                        {

                            var currentHorizontalAxis = GetChannel(channelName: group.HorizontalAxis, inputFileID: inputFile.Item1);
                            if (currentHorizontalAxis != null)
                            {
                                var currentHorizontalAxisData = GetChannel(channelName: group.HorizontalAxis, inputFileID: inputFile.Item1).Data;
                                if (currentHorizontalAxisData == null)
                                {
                                    ShowError.ShowErrorMessage($"Can't find '{group.HorizontalAxis}', so can't show diagram properly", nameof(DriverlessMenu));
                                }

                                if (currentHorizontalAxisData.Count > horizontalAxisData.Count)
                                {
                                    horizontalAxisData = new List<double>(currentHorizontalAxisData);
                                }

                                var channelDataPlotData = ConvertChannelDataToPlotData(channel.Data.ToArray(), currentHorizontalAxisData);


                                chart.AddPlot(xAxisValues: channelDataPlotData.Item1,
                                              yAxisValues: channelDataPlotData.Item2,
                                              lineWidth: lineWidth,
                                              lineColor: ColorTranslator.FromHtml(color),
                                              xAxisLabel: group.HorizontalAxis);

                                chart.AddSideValue(channelName: channelName.Item1,
                                              xAxisValues: channelDataPlotData.Item2,
                                              isActive: true,
                                              inputFileID: inputFile.Item1,
                                              color: color,
                                              lineWidth: lineWidth);
                            }
                            else
                            {
                                chart.AddPlot(xAxisValues: channel.Data.ToArray(),
                                              lineWidth: lineWidth,
                                              lineColor: ColorTranslator.FromHtml(color),
                                              xAxisLabel: group.HorizontalAxis);

                                chart.AddSideValue(channelName: channelName.Item1,
                                             xAxisValues: channel.Data.ToArray(),
                                             isActive: true,
                                             inputFileID: inputFile.Item1,
                                             color: color,
                                             lineWidth: lineWidth);
                            }
                        }
                        else
                        {
                            chart.AddSideValue(channelName: channelName.Item1, xAxisValues: new double[0], inputFileID: inputFile.Item1);
                        }

                        chart.AddChannelName(channelName.Item1);

                        addedChannelNames.Add(channelName.Item1);
                    }
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

        public override void SetChannelActivity(string channelName, int inputFileID, bool isActive)
        {
            bool found = false;
            for (int index = 0; index < channelNames.Count && !found; index++)
            {
                if (channelNames[index].Item1.Equals(channelName))
                {
                    for (int i = 0; i < channelNames[index].Item2.Count && !found; i++)
                    {
                        if (channelNames[index].Item2[i].Item1 == inputFileID)
                        {
                            channelNames[index].Item2[i] = new Tuple<int, bool>(inputFileID, isActive);
                            found = true;
                        }
                    }
                }
            }

            base.SetChannelActivity(channelName, inputFileID, isActive);
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
        private Tuple<double[], double[]> ConvertChannelDataToPlotData(List<DataLayer.Point> points)
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
        public override Channel GetChannel(string channelName, int? inputFileID = null)
        {
            if (inputFileID == null)
            {
                throw new Exception("Input file ID is null");
            }

            if (!InputFileManager.IsAnyDriverlessFile)
            {
                return null;
            }

            return InputFileManager.GetDriverlessFile((int)inputFileID).GetChannel(channelName);
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

        /// <summary>
        /// Initializes <see cref="integratedYawAngle"/> list based on <c>yawangle</c> and <c>yawrate</c>.
        /// Formula: <c>yawangle(x) = yawangle(x - 1) + dt * yawrate</c>,
        /// where <c>x</c> is the loop variable and <c>dt</c> ist the timestep in <b>ms</b>.
        /// </summary>
        public double[] GetYawAngle(int inputFileID)
        {
            List<double> integratedYawAngle = new List<double>();

            var yawrate = GetChannel(channelName: TextManager.DefaultYawRateChannelName, inputFileID: inputFileID);
            if (yawrate == null)
            {
                throw new Exception($"Can't find '{TextManager.DefaultYawRateChannelName}' channel");
            }

            integratedYawAngle.Add(0);
            for (int i = 1; i < yawrate.Data.Count; i++)
            {
                integratedYawAngle.Add(integratedYawAngle[i - 1] + DELTA_TIME * yawrate.Data[i]);
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
            if (trackChart != null)
            {
                trackChart.Update((int)DataSlider.Value);
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
                InputFileManager.ActiveDriverlessInputFileID = InputFileManager.GetLastDriverlessFile.ID;
                AddInputFileItem(InputFileManager.GetLastDriverlessFile.Name, InputFileManager.GetLastDriverlessFile.ID);
            }

            // UpdateAfterInputFileChoose();
            // InitChooseInputFileComboBox();
        }

        public void UpdateAfterFileTypeChanges()
        {
            selectedChannels.Clear();
            channelNames.Clear();

            var newSelectedInputFileIDs = new List<int>();

            InputFilesStackPanel.Children.Clear();
            foreach (var inputFile in InputFileManager.InputFiles)
            {
                if (inputFile.InputFileType == DataLayer.InputFiles.InputFileTypes.driverless)
                {
                    AddInputFileItem(inputFile.Name, inputFile.ID);
                    newSelectedInputFileIDs.Add(inputFile.ID);
                }
            }

            selectedInputFileIDs = new List<int>(newSelectedInputFileIDs);
            UpdateChannelsList();

            BuildCharts();

            DisableNoInputFileAndChannelsGrids(!InputFileManager.IsAnyDriverlessFile);
        }

        public void UpdateAfterFileRename(string newName)
        {
            foreach (CheckBox item in InputFilesStackPanel.Children)
            {
                if (InputFileManager.Get(item.Content.ToString()) == null)
                {
                    item.Content = newName;
                }
            }
        }

        private void DisableNoInputFileAndChannelsGrids(bool disable = true)
        {
            NoInputFilesGrid.Visibility = NoChannelsGrid.Visibility = disable ? Visibility.Hidden : Visibility.Visible;
        }

        private void AddInputFileItem(string fileName, int id)
        {
            var checkbox = new CheckBox { Content = $"{id} {fileName}" };
            checkbox.Checked += InputFileItem_Checked;
            checkbox.Unchecked += InputFileItem_Checked;
            checkbox.IsChecked = selectedInputFileIDs.Contains(id);
            InputFilesStackPanel.Children.Add(checkbox);
        }

        public void RemoveInputFileItem(string fileName)
        {
            int index = GetInputFileItemIndex(fileName);
            if (index != -1)
            {
                InputFilesStackPanel.Children.RemoveAt(index);
            }

            if (!InputFileManager.IsAnyDriverlessFile)
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
        /// Sets the <see cref="DataSlider"/>s maximum value based on the <see cref="channelNames"/> first <see cref="Channel.Data"/>-s count.
        /// </summary>
        private void SetUpDataSlider()
        {
            int max = 0;
            foreach (var channelName in channelNames)
            {
                foreach (var inputFile in channelName.Item2)
                {
                    if (selectedInputFileIDs.Contains(inputFile.Item1))
                    {
                        var channelDataCount = GetChannel(channelName: channelName.Item1, inputFileID: inputFile.Item1).Data.Count;
                        if (channelDataCount > max)
                        {
                            max = channelDataCount;
                        }
                    }
                }
            }

            DataSlider.Maximum = max;
        }

        private void DataSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshCharts();
            UpdateTrack();
        }

        private void InputFileItem_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var splittedName = checkBox.Content.ToString().Split(" ");
            string name = "";
            for (int i = 1; i < splittedName.Length; i++)
            {
                name += splittedName[i] + " ";
            }
            name = name[0..^1];

            var actInputFile = InputFileManager.GetDriverlessFile(name);

            if ((bool)checkBox.IsChecked)
            {
                selectedInputFileIDs.Add(actInputFile.ID);

                MergeChannelNames(actInputFile.Channels, actInputFile.ID);

                var yChannel = GetChannel(channelName: DefaultsManager.GetDefault(TextManager.DriverlessYChannel).Value, inputFileID: actInputFile.ID);
                if (yChannel == null)
                {
                    ShowError.ShowErrorMessage($"Can't find '{DefaultsManager.GetDefault(TextManager.DriverlessYChannel).Value}', so can't build track", nameof(DriverlessMenu));
                }

                var c0refChannel = GetChannel(channelName: DefaultsManager.GetDefault(TextManager.DriverlessC0refChannel).Value, inputFileID: actInputFile.ID);
                if (c0refChannel == null)
                {
                    ShowError.ShowErrorMessage($"Can't find '{DefaultsManager.GetDefault(TextManager.DriverlessC0refChannel).Value}', so can't build track", nameof(DriverlessMenu));
                }

                if (yChannel != null && c0refChannel != null)
                {
                    trackChart.AddTrackData(actInputFile.ID, name, HorizontalAxis(actInputFile.ID).Data.ToArray(), yChannel.Data.ToArray(), GetYawAngle(actInputFile.ID), (float)c0refChannel.Data.First());
                }
            }
            else
            {
                selectedInputFileIDs.Remove(actInputFile.ID);

                UnMergeChannelNames(actInputFile.Channels, actInputFile.ID);

                trackChart.RemoveTrackData(actInputFile.ID);
            }

            var removableElements = new List<string>();

            foreach (var selectedChannel in selectedChannels)
            {
                bool found = false;
                int index = 0;
                while (index < channelNames.Count && !found)
                {
                    if (selectedChannel.Equals(channelNames[index].Item1))
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
            SetUpDataSlider();
            BuildCharts();
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

            BuildCharts();
        }

        protected override void UpdateChannelsList()
        {
            ChannelsStackPanel.Children.Clear();

            int max = 0;

            foreach (var item in channelNames)
            {
                if (item.Item2.Count > max)
                {
                    max = item.Item2.Count;
                }
            }

            foreach (var item in channelNames)
            {
                var checkBox = new CheckBox
                {
                    IsChecked = selectedChannels.Contains(item.Item1)
                };
                checkBox.Checked += ChannelItem_Checked;
                checkBox.Unchecked += ChannelItem_Checked;
                checkBox.PreviewMouseRightButtonDown += ChannelCheckboc_PreviewMouseRightButtonDown;
                checkBox.MouseEnter += CheckBox_MouseEnter;
                checkBox.MouseLeave += CheckBox_MouseLeave;

                if (item.Item2.Count == max)
                {
                    checkBox.Content = item.Item1;
                }
                else
                {
                    string content = item.Item1 + " - ";
                    foreach (var inputFile in item.Item2)
                    {
                        content += InputFileManager.Get(inputFile.Item1).Name + ";";
                    }
                    checkBox.Content = content[0..^1];
                }

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

        private void MergeChannelNames(List<Channel> channels, int inputFileID)
        {
            foreach (var channel in channels)
            {
                var actChannel = channelNames.Find(x => x.Item1.Equals(channel.Name));
                if (actChannel != null)
                {
                    actChannel.Item2.Add(new Tuple<int, bool>(inputFileID, true));
                }
                else
                {
                    channelNames.Add(new Tuple<string, List<Tuple<int, bool>>>(channel.Name, new List<Tuple<int, bool>>() { new Tuple<int, bool>(inputFileID, true) }));
                }
            }
        }

        private void UnMergeChannelNames(List<Channel> channels, int inputFileID)
        {
            foreach (var channel in channels)
            {
                var actChannel = channelNames.Find(x => x.Item1.Equals(channel.Name));
                if (actChannel != null)
                {
                    int index = 0;
                    foreach (var item in actChannel.Item2)
                    {
                        if (item.Item1 == inputFileID)
                        {
                            break;
                        }
                        index++;
                    }
                    actChannel.Item2.RemoveAt(index);
                    if (actChannel.Item2.Count == 0)
                    {
                        channelNames.Remove(actChannel);
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

                    if (!InputFileManager.HasInputFile(fileName))
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
                    ShowError.ShowErrorMessage(exception.Message, nameof(DriverlessMenu));
                }
            }
        }

        public void SetLoadingGrid(bool visibility)
        {
            ReadFileProgressBarGrid.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
            ReadFileProgressBarLabel.Content = string.Empty;
            ReadFileProgressBar.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
        }

        private Channel HorizontalAxis(int inputFileID)
        {
            return GetChannel(channelName: DefaultsManager.GetDefault(TextManager.DriverlessHorizontalAxis).Value, inputFileID: inputFileID);
        }
    }
}
