using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;

namespace ART_TELEMETRY_APP.Tracks.UserControls
{
    /// <summary>
    /// Interaction logic for MapSettings.xaml
    /// </summary>
    public partial class TrackSettings : UserControl
    {
        public TrackSettingsItem ActiveTrackSettingsItem { get; set; }
        public List<TrackSettingsItem> TrackSettingsItems { get; } = new List<TrackSettingsItem>();

        public TrackSettings()
        {
            InitializeComponent();
            InitTrackSettingsItems();
        }

        public void InitTrackSettingsItems()
        {
            int activeTrackSettingsItemIndex =
                ActiveTrackSettingsItem == null ?
                                              0 :
                                              TrackSettingsItems.FindIndex(x => x.ActiveTrack.Equals(ActiveTrackSettingsItem.ActiveTrack));

            TracksStackPanel.Children.Clear();
            TrackSettingsItems.Clear();
            for (int i = 0; i < TrackManager.Tracks.Count; i++)
            {
                AddTrackSettingsItem(TrackManager.Tracks[i], i == activeTrackSettingsItemIndex);
            }

            UpdateActiveTrackSettingsContent();
        }

        private void AddTrackSettingsItem(Track track, bool isSelected = false)
        {
            TrackSettingsItem item = new TrackSettingsItem(track);
            if (isSelected)
            {
                ActiveTrackSettingsItem = item;
            }
            item.ChangeColorMode(isSelected);
            TrackSettingsItems.Add(item);
            TracksStackPanel.Children.Add(item);
        }

        public void UpdateActiveTrackSettingsContent(Grid progressBarGrid = null)
        {
            TrackEditorGrid.Children.Clear();

            if (ActiveTrackSettingsItem != null)
            {
                ChangeTrackNameTxtBox.Text = ActiveTrackSettingsItem.ActiveTrack.Name;
                ChangeTrackDescriptionTxtBox.Text = ActiveTrackSettingsItem.ActiveTrack.Description;

                foreach (var trackSettinsItem in TrackSettingsItems)
                {
                    trackSettinsItem.ChangeColorMode(trackSettinsItem.ActiveTrack.Equals(ActiveTrackSettingsItem.ActiveTrack));
                }

                bool found = false;
                foreach (Driver driver in DriverManager.Drivers)
                {
                    foreach (InputFile inputFile in InputFileManager.InputFiles)
                    {
                        if (inputFile.ActiveTrack != null)
                        {
                            if (inputFile.ActiveTrack.Name.Equals(ActiveTrackSettingsItem.ActiveTrack.Name))
                            {
                                TrackManager.GetTrack(inputFile.ActiveTrack).Processed = false;
                                TrackEditorGrid.Children.Add(new TrackEditor(inputFile, ActiveTrackSettingsItem.ActiveTrack, progressBarGrid));
                                found = true;
                            }
                        }
                    }
                }

                if (!found)
                {
                    NotConnectedLbl.Visibility = Visibility.Visible;
                    NotConnectedLbl.Content = string.Format("{0} is not connected to any inputfile.", ActiveTrackSettingsItem.ActiveTrack.Name);
                }
                else
                {
                    NotConnectedLbl.Visibility = Visibility.Hidden;
                    NotConnectedLbl.Content = string.Empty;
                }
            }
        }

        private void CancelAddTrack_Click(object sender, RoutedEventArgs e)
        {
            AddTrackNameTxtBox.Text = string.Empty;
            AddTrackDescriptionTxtBox.Text = string.Empty;
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            if (AddTrackNameTxtBox.Text.Equals(string.Empty))
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, "Track name is empty!", 2);
            }
            else
            {
                if (TrackManager.GetTrack(AddTrackNameTxtBox.Text, AddTrackDescriptionTxtBox.Text) == null)
                {
                    TrackManager.AddTrack(AddTrackNameTxtBox.Text, AddTrackDescriptionTxtBox.Text);
                    InitTrackSettingsItems();
                    ActiveTrackSettingsItem = TrackSettingsItems.Last();
                    UpdateActiveTrackSettingsContent();
                    ((DriversMenu)MenuManager.GetTab(TextManager.DriversMenuName).Content).InitDriverCards();
                }
                else
                {
                    ShowError.ShowErrorMessage(ref ErrorSnackbar, string.Format("{0} is already exists!", AddTrackNameTxtBox.Text), 2);
                }
            }

            AddTrackNameTxtBox.Text = string.Empty;
            AddTrackDescriptionTxtBox.Text = string.Empty;
        }

        //   private void ChangeTrackpNameTxtBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => changeMapName_txtbox.Text = string.Empty;

        //  private void changeMapDate_txtbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => changeMapDate_txtbox.Text = string.Empty;

        private void ChangeTracksData_Click(object sender, RoutedEventArgs e)
        {
            TrackManager.ChangeTrack(ActiveTrackSettingsItem.ActiveTrack, ChangeTrackNameTxtBox.Text, ChangeTrackDescriptionTxtBox.Text);
            InitTrackSettingsItems();
            ActiveTrackSettingsItem = TrackSettingsItems.Find(x => x.ActiveTrack.Name.Equals(ChangeTrackNameTxtBox.Text) &&
                                                                   x.ActiveTrack.Description.Equals(ChangeTrackDescriptionTxtBox.Text));
            UpdateActiveTrackSettingsContent();
        }

        public TrackSettingsItem GetMapSettingsItem(string mapName, string mapDescription)
        {
            foreach (TrackSettingsItem item in TrackSettingsItems)
            {
                if (item.ActiveTrack.Name.Equals(mapName) && item.ActiveTrack.Description.Equals(mapDescription))
                {
                    return item;
                }
            }

            return null;
        }

        public void SetActiveTrackSettingsItem(string mapName, string mapDescription)
        {
            ActiveTrackSettingsItem = GetMapSettingsItem(mapName, mapDescription);
        }

        public void UpdateTrackData()
        {
            foreach (Driver driver in DriverManager.Drivers)
            {
                foreach (InputFile inputFile in InputFileManager.InputFiles)
                {
                    if (inputFile.ActiveTrack != null)
                    {
                        if (inputFile.ActiveTrack.Name.Equals(ActiveTrackSettingsItem.ActiveTrack.Name))
                        {
                            LapsCountLbl.Content = $"Laps: {inputFile.Laps.Count}";
                            TrackLengthCountLbl.Content = $"Track length: {inputFile.AllDistances.Last()} m";
                            OneLapAverageLengthLbl.Content = $"Average lap length: {inputFile.AverageLapLength} m";
                        }
                    }
                }
            }

        }
    }
}
