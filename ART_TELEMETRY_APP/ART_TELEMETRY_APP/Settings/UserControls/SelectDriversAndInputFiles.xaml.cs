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

            var selectedLaps = new List<Tuple<List<string>, List<string>, int, int, bool, List<TimeSpan>>>(); //fileName, driverNames, lapIndex, lapCount, lastLap

            foreach (var inputFile in InputFileManager.InputFiles)
            {
                if (inputFile.IsSelected)
                {
                    foreach (var lap in inputFile.Laps)
                    {
                        if (selectedLaps.Find(x => x.Item3 == lap.Index) == null)
                        {
                            selectedLaps.Add(new Tuple<List<string>, List<string>, int, int, bool, List<TimeSpan>>(new List<string> { inputFile.FileName },
                                                                                                           new List<string> { inputFile.DriverName },
                                                                                                           lap.Index,
                                                                                                           1,
                                                                                                           lap.Index == inputFile.Laps.Count - 1,
                                                                                                           new List<TimeSpan> { lap.Time }));
                        }
                        else
                        {
                            var actLap = selectedLaps[selectedLaps.FindIndex(x => x.Item3 == lap.Index)];

                            var driverNameList = actLap.Item2;
                            driverNameList.Add(inputFile.DriverName);

                            var inputFileNameList = actLap.Item1;
                            inputFileNameList.Add(inputFile.FileName);

                            var lapTimesList = actLap.Item6;
                            lapTimesList.Add(lap.Time);

                            selectedLaps.Add(new Tuple<List<string>, List<string>, int, int, bool, List<TimeSpan>>(inputFileNameList,
                                                                                                           driverNameList,
                                                                                                           actLap.Item3,
                                                                                                           actLap.Item4 + 1,
                                                                                                           actLap.Item5,
                                                                                                           lapTimesList));
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

            foreach (var lap in selectedLaps)
            {
                LapState lapState = LapState.CenterLap;
                if (lap.Item3 == 0)
                {
                    lapState = LapState.InLap;
                }
                else if (lap.Item5)
                {
                    lapState = LapState.OutLap;
                }

                var lapElement = new LapElement(lap.Item3, lapState, DriverManager.SelectedDriversCount > 0 || lap.Item4 > 1, lap.Item1, lap.Item2, lap.Item6);
                LapItemsStackPanel.Children.Add(lapElement);
            }
        }

        public void InitSelectedChannels()
        {
            SelectedChannelsStackPanel.Children.Clear();

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
            }
        }
    }
}
