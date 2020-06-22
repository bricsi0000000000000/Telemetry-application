using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Pilots;
using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for MapSettings.xaml
    /// </summary>
    public partial class MapSettings : UserControl
    {
        List<MapSettingsItem> map_settings_items = new List<MapSettingsItem>();
        public MapSettingsItem ActiveMapSettingsItem;

        public MapSettings()
        {
            InitializeComponent();

            InitMapSettingsItems();
            ActiveMapSettingsItem = map_settings_items.First();
            UpdateActiveMapSettingsContent();
        }

        public void InitMapSettingsItems()
        {
            savedMaps_stackpanel.Children.Clear();
            map_settings_items.Clear();
            foreach (Map map in MapManager.Maps)
            {
                MapSettingsItem item = new MapSettingsItem(map.Name, map.Year);
                map_settings_items.Add(item);
                savedMaps_stackpanel.Children.Add(item);
            }
        }

        public void UpdateActiveMapSettingsContent()
        {
            mapEditor_grid.Children.Clear();
            changeMapName_txtbox.Text = ActiveMapSettingsItem.MapName;
            changeMapDate_txtbox.Text = ActiveMapSettingsItem.MapYear;

            foreach (MapSettingsItem item in map_settings_items)
            {
                if (item.MapName == ActiveMapSettingsItem.MapName && item.MapYear == ActiveMapSettingsItem.MapYear)
                {
                    item.ChangeColorMode(true);
                }
                else
                {
                    item.ChangeColorMode(false);
                }
            }

            bool found = false;
            foreach (Pilot pilot in PilotManager.Pilots)
            {
                foreach (InputFile input_file in pilot.InputFiles)
                {
                    if (input_file.MapName == ActiveMapSettingsItem.MapName)
                    {
                        mapEditor_grid.Children.Add(new MapEditor_UC(input_file, ActiveMapSettingsItem.MapName));
                        found = true;
                    }
                }
            }

            if (!found)
            {
                notConnected_lbl.Visibility = Visibility.Visible;
                notConnected_lbl.Content = string.Format("{0} is not connected to any inputfile.", ActiveMapSettingsItem.MapName);
            }
            else
            {
                notConnected_lbl.Visibility = Visibility.Hidden;
                notConnected_lbl.Content = "";
            }
        }

        public void ChangeActiveMapSettingsItem()
        {
            ActiveMapSettingsItem = map_settings_items.Last();
        }

        private void addMap_Click(object sender, RoutedEventArgs e)
        {
            if (addMapTxtbox.Text.Equals("") && addMapDateTxtbox.Text.Equals(""))
            {
                error_snack_bar.MessageQueue.Enqueue(string.Format("Map's name and date are empty!"),
                                                     null, null, null, false, true, TimeSpan.FromSeconds(2));
            }
            else if (addMapTxtbox.Text.Equals(""))
            {
                error_snack_bar.MessageQueue.Enqueue(string.Format("Map's name is empty!"),
                                                     null, null, null, false, true, TimeSpan.FromSeconds(2));
            }
            else if (addMapDateTxtbox.Text.Equals(""))
            {
                error_snack_bar.MessageQueue.Enqueue(string.Format("Date is empty!"),
                                                     null, null, null, false, true, TimeSpan.FromSeconds(2));
            }
            else
            {
                if (MapManager.GetMap(addMapTxtbox.Text) == null)
                {
                    MapManager.AddMap(addMapTxtbox.Text, addMapDateTxtbox.Text);
                    InitMapSettingsItems();
                    ActiveMapSettingsItem = map_settings_items.Last();
                    UpdateActiveMapSettingsContent();
                }
                else
                {
                    error_snack_bar.MessageQueue.Enqueue(string.Format("{0} is already exists!", addMapTxtbox.Text),
                                                         null, null, null, false, true, TimeSpan.FromSeconds(2));
                }
            }
            addMapTxtbox.Text = "";
            addMapDateTxtbox.Text = "";
        }

        private void changeMapName_txtbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            changeMapName_txtbox.Text = "";
        }

        private void changeMapDate_txtbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            changeMapDate_txtbox.Text = "";
        }

        private void changeMapsDatas_Click(object sender, RoutedEventArgs e)
        {
            MapManager.GetMap(ActiveMapSettingsItem.MapName).Name = changeMapName_txtbox.Text;
            MapManager.GetMap(changeMapName_txtbox.Text).Year = changeMapDate_txtbox.Text;
            InitMapSettingsItems();
            ActiveMapSettingsItem = map_settings_items.Find(n => n.MapName == changeMapName_txtbox.Text);
            UpdateActiveMapSettingsContent();
        }

        public MapSettingsItem GetMapSettingsItem(string map_name, string map_date)
        {
            //return map_settings_items.Find(n => n.Name == map_name && n.MapYear == map_date);
            foreach (MapSettingsItem item in map_settings_items)
            {
                if (item.MapName == map_name && item.MapYear == map_date)
                {
                    return item;
                }
            }

            return null;
        }

        public List<MapSettingsItem> MapSettingsItems
        {
            get
            {
                return map_settings_items;
            }
        }
    }
}
