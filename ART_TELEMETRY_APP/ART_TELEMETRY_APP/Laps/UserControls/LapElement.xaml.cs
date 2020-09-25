using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Settings.UserControls;
using System;
using System.Collections.Generic;
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

        public LapElement(int lapIndex, LapState lapState, bool disabled, List<string> fileNames, List<string> driverNames, List<TimeSpan> lapTimes)
        {
            InitializeComponent();

            switch (lapState)
            {
                case LapState.InLap:
                    LapIndexLbl.Content = "In lap";
                    break;
                case LapState.CenterLap:
                    LapIndexLbl.Content = $"{lapIndex}. lap";
                    break;
                case LapState.OutLap:
                    LapIndexLbl.Content = "Out lap";
                    break;
            }
            this.lapIndex = lapIndex;

            LapElementCard.IsEnabled = disabled;

            InitLapData(ref fileNames, ref driverNames, ref lapTimes);
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

        private void InitLapData(ref List<string> fileNames, ref List<string> driverNames, ref List<TimeSpan> lapTimes)
        {
            InputFilesStackPanel.Children.Clear();

            if (fileNames.Count != driverNames.Count)
            {
                return;
            }

            for (int i = 0; i < fileNames.Count; i++)
            {
                var lapElementData = new LapElementData(fileNames[i], driverNames[i], lapTimes[i]);
                InputFilesStackPanel.Children.Add(lapElementData);
            }
        }

        private void ChangeState()
        {
            SelectLapIcon.Kind = isSelected ? MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarked : MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline;
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
