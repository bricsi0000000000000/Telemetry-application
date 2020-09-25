﻿using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.InputFiles.UserControls;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Settings.UserControls;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Drivers.UserControls
{
    /// <summary>
    /// Interaction logic for DriverItem.xaml
    /// </summary>
    public partial class DriverItem : UserControl
    {
        private readonly Driver driver;
        readonly BrushConverter converter = new BrushConverter();

        public DriverItem(Driver driver)
        {
            InitializeComponent();

            this.driver = driver;

            DriverNameLbl.Content = driver.Name;
            ChangeState();
            InitInputFileItems();
        }

        private void InitInputFileItems()
        {
            InputFilesStackPanel.Children.Clear();
            foreach (var inputFile in InputFileManager.InputFiles)
            {
                if (inputFile.DriverName.Equals(driver.Name))
                {
                    InputFileItem inputFileItem = new InputFileItem(inputFile);
                    InputFilesStackPanel.Children.Add(inputFileItem);
                }
            }
        }

        private void ChangeState()
        {
            SelectDriverIcon.Kind = driver.IsSelected ? MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarked : MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline;
            SelectDriverIcon.Foreground = driver.IsSelected ? (Brush)converter.ConvertFromString("#FFE21B1B") : Brushes.White;
        }

        private void SelectDriverBtn_Click(object sender, RoutedEventArgs e)
        {
            driver.IsSelected = !driver.IsSelected;
            ChangeState();

            if (!driver.IsSelected)
            {
                foreach (var inputFile in InputFileManager.InputFiles)
                {
                    if (inputFile.DriverName.Equals(driver.Name))
                    {
                        inputFile.IsSelected = false;
                    }
                }

                InitInputFileItems();
            }

            ((SelectDriversAndInputFiles)MenuManager.GetTab(TextManager.DiagramsSettingsMenuName).Content).InitLapItems();
            ((SelectDriversAndInputFiles)MenuManager.GetTab(TextManager.DiagramsSettingsMenuName).Content).InitSelectedChannels();
        }
    }
}
