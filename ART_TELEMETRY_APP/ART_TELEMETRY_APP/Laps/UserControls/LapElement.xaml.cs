using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Settings.UserControls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Laps.UserControls
{
    /// <summary>
    /// Interaction logic for LapElement.xaml
    /// </summary>
    public partial class LapElement : UserControl
    {
        int lapIndex;
        bool isSelected = false;
        private readonly BrushConverter converter = new BrushConverter();

        public LapElement(List<InputFile> inputFiles, int lapIndex, LapType lapType, bool disabled, List<TimeSpan> lapTimes)
        {
            InitializeComponent();

            switch (lapType)
            {
                case LapType.InLap:
                    LapIndexLbl.Content = "In lap";
                    LapElementCard.Background = (Brush)converter.ConvertFromString("#FF383838");
                    break;
                case LapType.CenterLap:
                    LapIndexLbl.Content = $"{lapIndex}. lap";
                    break;
                case LapType.OutLap:
                    LapIndexLbl.Content = "Out lap";
                    LapElementCard.Background = (Brush)converter.ConvertFromString("#FF383838");
                    break;
                case LapType.AllLap:
                    LapIndexLbl.Content = "All lap";
                    LapElementCard.Background = (Brush)converter.ConvertFromString("#FF383838");
                    break;
            }
            this.lapIndex = lapIndex;

            LapElementCard.IsEnabled = disabled;

            InitLapData(ref inputFiles, ref lapTimes, lapType == LapType.AllLap);
            ChangeState();
            /*  Console.Write(lapIndex + "\t");
              foreach (var item in fileNames)
              {
                  Console.Write(item + ", ");
              }
              Console.Write("\t");
              foreach (var item in driverNames)
              {
                  Console.Write(item + ", ");
              }
              Console.Write("\t");
              foreach (var item in lapTimes)
              {
                  Console.Write(item + ", ");
              }
              Console.WriteLine();*/
        }

        private void InitLapData(ref List<InputFile> inputFiles, ref List<TimeSpan> lapTimes, bool allLap)
        {
            InputFilesStackPanel.Children.Clear();

            if (inputFiles.Count != lapTimes.Count)
            {
                return;
            }

            if (allLap)
            {
                var allInputFiles = inputFiles.ToHashSet();
                foreach (var item in allInputFiles)
                {

                }
                long allLapTimesTicks = 0;
                foreach (var lapTime in lapTimes)
                {
                    allLapTimesTicks += lapTime.Ticks;
                }
                var allLapTimes = TimeSpan.FromTicks(allLapTimesTicks);

                var lapElementData = new LapElementData("", "", allLapTimes, LapState.None);
                InputFilesStackPanel.Children.Add(lapElementData);
            }
            else
            {
                for (int i = 0; i < inputFiles.Count; i++)
                {
                    LapState lapState = LapState.None;
                    if (inputFiles[i].BestLapTime == lapTimes[i])
                    {
                        lapState = LapState.Best;
                    }
                    else if (inputFiles[i].WorstLapTime == lapTimes[i])
                    {
                        lapState = LapState.Worst;
                    }

                    var lapElementData = new LapElementData(inputFiles[i].FileName, inputFiles[i].DriverName, lapTimes[i], lapState);
                    InputFilesStackPanel.Children.Add(lapElementData);
                }
            }
        }

        private void ChangeState()
        {
            SelectLapIcon.Kind = isSelected ? PackIconKind.CheckboxMarked : PackIconKind.CheckboxBlankOutline;
            SelectLapIcon.Foreground = isSelected ? (Brush)converter.ConvertFromString("#FFE21B1B") : Brushes.White;
        }

        private void SelectLap_Click(object sender, RoutedEventArgs e)
        {
            isSelected = !isSelected;
            ChangeState();
            if (isSelected)
            {
                ChartsSelectedData.SelectedLaps.Add(lapIndex);
            }
            else
            {
                if (ChartsSelectedData.SelectedLaps.Contains(lapIndex))
                {
                    ChartsSelectedData.SelectedLaps.Remove(lapIndex);
                }
            }
        }
    }
}
