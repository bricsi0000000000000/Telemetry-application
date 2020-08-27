using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Settings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Maps.UserControls
{
    /// <summary>
    /// Interaction logic for MapSettingsItem.xaml
    /// </summary>
    public partial class MapSettingsItem : UserControl
    {
        public Map ActiveMap { get; set; }

        public MapSettingsItem(Map map)
        {
            InitializeComponent();

            ActiveMap = map;
            mapName_lbl.Content = map.Name;
            mapDate_lbl.Content = string.Format("- {0}", map.Year);
        }

        private void deleteMap_Click(object sender, RoutedEventArgs e)
        {
            MapManager.DeleteMap(ActiveMap);
            ((MapSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Maps").Content).InitMapSettingsItems();
            ((MapSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Maps").Content).ChangeActiveMapSettingsItem();
            ((MapSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Maps").Content).UpdateActiveMapSettingsContent();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((MapSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Maps").Content).ActiveMapSettingsItem = this;
            ((MapSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Maps").Content).UpdateActiveMapSettingsContent();
        }

        public void ChangeColorMode(bool change)
        {
            if (change)
            {
                colorZone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Inverted;
                mapName_lbl.Foreground = Brushes.Black;
                mapDate_lbl.Foreground = Brushes.Black;
            }
            else
            {
                colorZone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Dark;
                mapName_lbl.Foreground = Brushes.White;
                mapDate_lbl.Foreground = Brushes.White;
            }
        }
    }
}
