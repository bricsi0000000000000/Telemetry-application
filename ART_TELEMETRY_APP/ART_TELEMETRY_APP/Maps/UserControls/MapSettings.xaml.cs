using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.ShowError.Classes;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ART_TELEMETRY_APP.Maps.UserControls
{
    /// <summary>
    /// Interaction logic for MapSettings.xaml
    /// </summary>
    public partial class MapSettings : UserControl
    {
        public MapSettingsItem ActiveMapSettingsItem { get; set; }

        private ErrorMessage error_message;

        public MapSettings()
        {
            InitializeComponent();

            error_message = showError;

            InitMapSettingsItems();
            if (MapSettingsItems.Count > 0)
            {
                ActiveMapSettingsItem = MapSettingsItems.First();
                UpdateActiveMapSettingsContent();
            }
        }

        public void InitMapSettingsItems()
        {
            savedMaps_stackpanel.Children.Clear();
            MapSettingsItems.Clear();
            foreach (Map map in MapManager.Maps)
            {
                MapSettingsItem item = new MapSettingsItem(map);
                MapSettingsItems.Add(item);
                savedMaps_stackpanel.Children.Add(item);
            }
        }

        public void UpdateActiveMapSettingsContent(Grid progressbar_grid = null)
        {
            mapEditor_grid.Children.Clear();
            changeMapName_txtbox.Text = ActiveMapSettingsItem.ActiveMap.Name;
            changeMapDate_txtbox.Text = ActiveMapSettingsItem.ActiveMap.Year.ToString();

            foreach (MapSettingsItem item in MapSettingsItems)
            {
                if (item.ActiveMap.Equals(ActiveMapSettingsItem.ActiveMap))
                {
                    item.ChangeColorMode(true);
                }
                else
                {
                    item.ChangeColorMode(false);
                }
            }

            bool found = false;
            foreach (Driver pilot in DriverManager.Drivers)
            {
                foreach (InputFile input_file in pilot.InputFiles)
                {
                    if (input_file.ActiveMap.Name.Equals(ActiveMapSettingsItem.ActiveMap.Name))
                    {
                        MapManager.GetMap(input_file.ActiveMap).Processed = false;
                        mapEditor_grid.Children.Add(new MapEditor_UC(input_file, ActiveMapSettingsItem.ActiveMap, progressbar_grid));
                        found = true;
                    }
                }
            }

            if (!found)
            {
                notConnected_lbl.Visibility = Visibility.Visible;
                notConnected_lbl.Content = string.Format("{0} is not connected to any inputfile.", ActiveMapSettingsItem.ActiveMap.Name);
            }
            else
            {
                notConnected_lbl.Visibility = Visibility.Hidden;
                notConnected_lbl.Content = "";
            }
        }

        public void ChangeActiveMapSettingsItem() => ActiveMapSettingsItem = MapSettingsItems.Last();

        private void cancelAddMap_Click(object sender, RoutedEventArgs e)
        {
            addMapTxtbox.Text = string.Empty;
            addMapDateTxtbox.Text = string.Empty;
        }

        private void addMap_Click(object sender, RoutedEventArgs e)
        {
            if (addMapTxtbox.Text.Equals(string.Empty) && addMapDateTxtbox.Text.Equals(string.Empty))
            {
                error_message(ref error_snack_bar, "Map's name and date are empty!", 2);
            }
            else if (addMapTxtbox.Text.Equals(string.Empty))
            {
                error_message(ref error_snack_bar, "Map's name is empty!", 2);
            }
            else if (addMapDateTxtbox.Text.Equals(string.Empty))
            {
                error_message(ref error_snack_bar, "Date is empty!", 2);
            }
            else
            {
                if (MapManager.GetMap(addMapTxtbox.Text, int.Parse(addMapDateTxtbox.Text)) == null)
                {
                    MapManager.AddMap(addMapTxtbox.Text, int.Parse(addMapDateTxtbox.Text));
                    InitMapSettingsItems();
                    ActiveMapSettingsItem = MapSettingsItems.Last();
                    UpdateActiveMapSettingsContent();
                    ((DriversMenuContent)TabManager.GetTab(TextManager.DriversMenuName).Content).InitDrivers();
                }
                else
                {
                    error_message(ref error_snack_bar, string.Format("{0} is already exists!", addMapTxtbox.Text), 2);
                }
            }

            addMapTxtbox.Text = string.Empty;
            addMapDateTxtbox.Text = string.Empty;
        }

        private void changeMapName_txtbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => changeMapName_txtbox.Text = string.Empty;

        private void changeMapDate_txtbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => changeMapDate_txtbox.Text = string.Empty;

        private void changeMapsDatas_Click(object sender, RoutedEventArgs e)
        {
            MapManager.ChangeMap(ActiveMapSettingsItem.ActiveMap, changeMapName_txtbox.Text, int.Parse(changeMapDate_txtbox.Text));
            InitMapSettingsItems();
            ActiveMapSettingsItem = MapSettingsItems.Find(n => n.ActiveMap.Name.Equals(changeMapName_txtbox.Text) && n.ActiveMap.Year.Equals(int.Parse(changeMapDate_txtbox.Text)));
            UpdateActiveMapSettingsContent();
        }

        public MapSettingsItem GetMapSettingsItem(string map_name, int map_date)
        {
            //return map_settings_items.Find(n => n.Name == map_name && n.MapYear == map_date);
            foreach (MapSettingsItem item in MapSettingsItems)
            {
                if (item.ActiveMap.Name.Equals(map_name) && item.ActiveMap.Year.Equals(map_date))
                {
                    return item;
                }
            }

            return null;
        }

        public List<MapSettingsItem> MapSettingsItems { get; } = new List<MapSettingsItem>();

        private void showError(ref Snackbar snackbar, string message, double time) 
            => snackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
    }
}
