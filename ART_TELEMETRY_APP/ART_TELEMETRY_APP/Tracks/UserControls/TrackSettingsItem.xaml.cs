using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Tracks.UserControls
{
    /// <summary>
    /// Interaction logic for <seealso cref="TrackSettingsItem"/>.xaml
    /// </summary>
    public partial class TrackSettingsItem : UserControl
    {
        public Track ActiveTrack { get; set; }

        public TrackSettingsItem(Track track)
        {
            InitializeComponent();

            ActiveTrack = track;
            TrackNameLbl.Content = track.Name;
            TrackDescriptionLbl.Content = track.Description;
        }

        private void DeleteTrack_Click(object sender, RoutedEventArgs e)
        {
            TrackManager.DeleteTrack(ActiveTrack);
            ((TrackSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.TracksSettingsName).Content).InitTrackSettingsItems();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((TrackSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.TracksSettingsName).Content).ActiveTrackSettingsItem = this;
            ((TrackSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.TracksSettingsName).Content).UpdateActiveTrackSettingsContent();
            ((TrackSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.TracksSettingsName).Content).UpdateTrackData();
        }

        public void ChangeColorMode(bool change)
        {
            if (change)
            {
                colorZone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Inverted;
                TrackNameLbl.Foreground = Brushes.Black;
                TrackDescriptionLbl.Foreground = Brushes.Black;
            }
            else
            {
                colorZone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Dark;
                TrackNameLbl.Foreground = Brushes.White;
                TrackDescriptionLbl.Foreground = Brushes.White;
            }
        }
    }
}
