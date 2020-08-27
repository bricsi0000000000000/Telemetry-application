using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Groups.UserControls;
using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Maps.UserControls;
using ART_TELEMETRY_APP.Sectors.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
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

namespace ART_TELEMETRY_APP.Settings
{
    /// <summary>
    /// Interaction logic for SettingsMenuContent.xaml
    /// </summary>
    public partial class SettingsMenuContent : UserControl
    {
        List<TabItem> settings_tabs = new List<TabItem>();

        public SettingsMenuContent()
        {
            InitializeComponent();

            InitSettingsTabs();
        }

        public void InitSettingsTabs()
        {
            settings_tabcontrol.Items.Clear();

            TabItem map_settings_item = new TabItem();
            map_settings_item.Header = TextManager.MapsSettingsName;
            map_settings_item.Content = new MapSettings();
            map_settings_item.IsSelected = true;
            settings_tabs.Add(map_settings_item);
            settings_tabcontrol.Items.Add(map_settings_item);

            TabItem sector_settings_item = new TabItem();
            sector_settings_item.Header = TextManager.SectorsSettingsName;
            sector_settings_item.Content = new SectorsSettings();
            settings_tabs.Add(sector_settings_item);
            settings_tabcontrol.Items.Add(sector_settings_item);

            TabItem group_settings_item = new TabItem();
            group_settings_item.Header = TextManager.GroupsSettingsName;
            group_settings_item.Content = new GroupSettings();
            settings_tabs.Add(group_settings_item);
            settings_tabcontrol.Items.Add(group_settings_item);
        }

        public TabItem GetTab(string name)
        {
            return settings_tabs.Find(n => n.Header.Equals(name));
        }
    }
}
