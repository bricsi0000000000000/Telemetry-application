using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Maps.UserControls;
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
            map_settings_item.Header = "Maps";
            map_settings_item.Content = new MapSettings();
            map_settings_item.IsSelected = true;
            settings_tabs.Add(map_settings_item);
            settings_tabcontrol.Items.Add(map_settings_item);
        }

        public TabItem GetTab(string name)
        {
            return settings_tabs.Find(n => n.Header.Equals(name));
        }
    }
}
