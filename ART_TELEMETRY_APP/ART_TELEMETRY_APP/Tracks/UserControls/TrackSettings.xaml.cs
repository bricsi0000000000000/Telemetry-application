using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;

namespace ART_TELEMETRY_APP.Tracks.UserControls
{
    /// <summary>
    /// Interaction logic for MapSettings.xaml
    /// </summary>
    public partial class TrackSettings : UserControl
    {
        public TrackSettingsItem ActiveTrackSettingsItem { get; set; }

        public TrackSettings()
        {
            InitializeComponent();
            InitTrackSettingsItems();
        }

        public void InitTrackSettingsItems()
        {
            TracksStackPanel.Children.Clear();
            TrackSettingsItems.Clear();
            for (int i = 0; i < TrackManager.Tracks.Count; i++)
            {
                AddTrackSettingsItem(TrackManager.Tracks[i], i == 0);
            }

            ChangeActiveMapSettingsItem();
            UpdateActiveMapSettingsContent();
        }

        private void AddTrackSettingsItem(Track track, bool isSelected = false)
        {
            TrackSettingsItem item = new TrackSettingsItem(track);
            item.ChangeColorMode(isSelected);
            TrackSettingsItems.Add(item);
            TracksStackPanel.Children.Add(item);
        }

        public void UpdateActiveMapSettingsContent(Grid progressBarGrid = null)
        {
            TrackEditorGrid.Children.Clear();

            ChangeTrackNameTxtBox.Text = ActiveTrackSettingsItem.ActiveTrack.Name;
            ChangeTrackDateTxtBox.Text = ActiveTrackSettingsItem.ActiveTrack.Year.ToString();

            foreach (var trackSettinsItem in TrackSettingsItems)
            {
                trackSettinsItem.ChangeColorMode(trackSettinsItem.ActiveTrack.Equals(ActiveTrackSettingsItem.ActiveTrack));
            }

            bool found = false;
            foreach (Driver driver in DriverManager.Drivers)
            {
                foreach (InputFile inputFile in InputFileManager.InputFiles)
                {
                    if (inputFile.ActiveTrack.Name.Equals(ActiveTrackSettingsItem.ActiveTrack.Name))
                    {
                        TrackManager.GetTrack(inputFile.ActiveTrack).Processed = false;
                        TrackEditorGrid.Children.Add(new TrackEditor(inputFile, ActiveTrackSettingsItem.ActiveTrack, progressBarGrid));
                        found = true;
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

        public void ChangeActiveMapSettingsItem() => ActiveTrackSettingsItem = TrackSettingsItems.First();

        private void CancelAddTrack_Click(object sender, RoutedEventArgs e)
        {
            /* addMapTxtbox.Text = string.Empty;
             addMapDateTxtbox.Text = string.Empty;*/
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            /* if (addMapTxtbox.Text.Equals(string.Empty) && addMapDateTxtbox.Text.Equals(string.Empty))
             {
                 //error_message(ref error_snack_bar, "Map's name and date are empty!", 2);
             }
             else if (addMapTxtbox.Text.Equals(string.Empty))
             {
                // error_message(ref error_snack_bar, "Map's name is empty!", 2);
             }
             else if (addMapDateTxtbox.Text.Equals(string.Empty))
             {
                // error_message(ref error_snack_bar, "Date is empty!", 2);
             }
             else
             {
                 if (TrackManager.GetTrack(addMapTxtbox.Text, int.Parse(addMapDateTxtbox.Text)) == null)
                 {
                     TrackManager.AddMap(addMapTxtbox.Text, int.Parse(addMapDateTxtbox.Text));
                     InitTrackSettingsItems();
                     ActiveMapSettingsItem = TrackSettingsItems.Last();
                     UpdateActiveMapSettingsContent();
                     ((DriversMenu)MenuManager.GetTab(TextManager.DriversMenuName).Content).InitDriverCards();
                 }
                 else
                 {
                     //error_message(ref error_snack_bar, string.Format("{0} is already exists!", addMapTxtbox.Text), 2);
                 }
             }

             addMapTxtbox.Text = string.Empty;
             addMapDateTxtbox.Text = string.Empty;*/
        }

        //   private void ChangeTrackpNameTxtBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => changeMapName_txtbox.Text = string.Empty;

        //  private void changeMapDate_txtbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => changeMapDate_txtbox.Text = string.Empty;

        private void ChangeTracksData_Click(object sender, RoutedEventArgs e)
        {
            /* TrackManager.ChangeMap(ActiveMapSettingsItem.ActiveMap, changeMapName_txtbox.Text, int.Parse(changeMapDate_txtbox.Text));
             InitTrackSettingsItems();
             ActiveMapSettingsItem = TrackSettingsItems.Find(n => n.ActiveMap.Name.Equals(changeMapName_txtbox.Text) && n.ActiveMap.Year.Equals(int.Parse(changeMapDate_txtbox.Text)));
             UpdateActiveMapSettingsContent();*/
        }

        public TrackSettingsItem GetMapSettingsItem(string mapName, int mapYear)
        {
            foreach (TrackSettingsItem item in TrackSettingsItems)
            {
                if (item.ActiveTrack.Name.Equals(mapName) && item.ActiveTrack.Year.Equals(mapYear))
                {
                    return item;
                }
            }

            return null;
        }

        public void SetActiveTrackSettingsItem(string mapName, int mapYear)
        {
            ActiveTrackSettingsItem = GetMapSettingsItem(mapName, mapYear);
        }

        public List<TrackSettingsItem> TrackSettingsItems { get; } = new List<TrackSettingsItem>();

        private void showError(ref Snackbar snackbar, string message, double time)
            => snackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
    }
}
