using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Settings;
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

namespace ART_TELEMETRY_APP.Maps.UserControls
{
    /// <summary>
    /// Interaction logic for MapSettingsItem.xaml
    /// </summary>
    public partial class MapSettingsItem : UserControl
    {
        string map_name;
        string map_year;
        public MapSettingsItem(string map_name, string map_date)
        {
            InitializeComponent();

            this.map_name = map_name;
            this.map_year = map_date;
            mapName_lbl.Content = map_name;
            mapDate_lbl.Content = string.Format("- {0}", map_date);
        }

        private void deleteMap_Click(object sender, RoutedEventArgs e)
        {
            MapManager.DeleteMap(map_name);
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

        public string MapName
        {
            get
            {
                return map_name;
            }
            set
            {
                map_name = value;
            }
        }

        public string MapYear { get => map_year; set => map_year = value; }
    }
}
