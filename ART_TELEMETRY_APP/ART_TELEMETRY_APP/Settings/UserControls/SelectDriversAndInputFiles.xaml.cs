using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Datas.UserControls;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Settings.UserControls
{
    /// <summary>
    /// Interaction logic for SelectDriversAndInputFiles.xaml
    /// </summary>
    public partial class SelectDriversAndInputFiles : UserControl
    {
        public SelectDriversAndInputFiles()
        {
            InitializeComponent();

            InitDriversItems();
            InitLapItems();
        }

        public void InitDriversItems()
        {
            DriverItemsWrapPanel.Children.Clear();

            foreach (var driver in DriverManager.Drivers)
            {
                var driverItem = new DriverItem(driver);
                DriverItemsWrapPanel.Children.Add(driverItem);
            }
        }

        public void InitLapItems()
        {
            LapItemsStackPanel.Children.Clear();

            ///List<InputFile> and List<TimeSpan> have the same length
            var selectedLaps = new List<Tuple<List<InputFile>, int, int, List<TimeSpan>, bool>>(); //inputFiles, lapIndex, lapCount, times, outLap

            foreach (var inputFile in InputFileManager.InputFiles)
            {
                if (inputFile.IsSelected)
                {
                    foreach (var lap in inputFile.Laps)
                    {
                        if (selectedLaps.Find(x => x.Item2 == lap.Index) == null)
                        {
                            selectedLaps.Add(new Tuple<List<InputFile>, int, int, List<TimeSpan>, bool>(new List<InputFile> { inputFile },
                                                                                                        lap.Index,
                                                                                                        1,
                                                                                                        new List<TimeSpan> { lap.Time },
                                                                                                        lap.Index == inputFile.Laps.Count - 1));
                        }
                        else
                        {
                            var actLap = selectedLaps[selectedLaps.FindIndex(x => x.Item2 == lap.Index)];

                            var inputFileList = actLap.Item1;
                            inputFileList.Add(inputFile);

                            var lapTimesList = actLap.Item4;
                            lapTimesList.Add(lap.Time);

                            selectedLaps.Add(new Tuple<List<InputFile>, int, int, List<TimeSpan>, bool>(inputFileList,
                                                                                                        actLap.Item2,
                                                                                                        actLap.Item3 + 1,
                                                                                                        lapTimesList,
                                                                                                        actLap.Item5));
                            selectedLaps.Remove(actLap);
                        }
                    }
                }
            }

            /* foreach (var lap in laps)
             {
                 foreach (var item in lap.Item1)
                 {
                     Console.Write(item + ", ");
                 }
                 Console.Write("\t");
                 foreach (var item in lap.Item2)
                 {
                     Console.Write(item + ", ");
                 }
                 Console.Write("\t" + lap.Item3 + "\t" + lap.Item4 + "\t" + lap.Item5 + "\t");
                 foreach (var item in lap.Item6)
                 {
                     Console.Write(item + ", ");
                 }
                 Console.WriteLine();
             }*/

            ChartsSelectedData.SelectedLaps.Clear();

            var allSelectedInputFile = new List<InputFile>();
            foreach (var lap in selectedLaps)
            {
                foreach (var inputFile in lap.Item1)
                {
                    allSelectedInputFile.Add(inputFile);
                }
            }

            var allLapTimes = new List<TimeSpan>();
            foreach (var lap in selectedLaps)
            {
                foreach (var lapTime in lap.Item4)
                {
                    allLapTimes.Add(lapTime);
                }
            }

        /*    var allLapElement = new LapElement(allSelectedInputFile, -1, LapType.AllLap, false, allLapTimes);
            LapItemsStackPanel.Children.Add(allLapElement);*/

            foreach (var lap in selectedLaps)
            {
                LapType lapState = LapType.CenterLap;
                if (lap.Item2 == 0)
                {
                    lapState = LapType.InLap;
                }
                else if (lap.Item5)
                {
                    lapState = LapType.OutLap;
                }

                var lapElement = new LapElement(lap.Item1, lap.Item2, lapState, DriverManager.SelectedDriversCount > 0 || lap.Item3 > 1, lap.Item4);
                LapItemsStackPanel.Children.Add(lapElement);
            }
        }

        public void InitSelectedChannels()
        {
            /* SelectedChannelsStackPanel.Children.Clear();

             var channels = new HashSet<string>();

             foreach (var inputFile in InputFileManager.InputFiles)
             {
                 if (inputFile.IsSelected)
                 {
                     foreach (var channel in inputFile.Channels)
                     {
                         channels.Add(channel.ChannelName);
                     }
                 }
             }

             foreach (var channel in channels)
             {
                 var selectedChannel = new SelectedChannel(channel);
                 SelectedChannelsStackPanel.Children.Add(selectedChannel);
             }*/
        }

        private void UpdateCharts_Click(object sender, RoutedEventArgs e)
        {
            ChartBuilder.Build(Filter.kalman);
        }
    }
}
