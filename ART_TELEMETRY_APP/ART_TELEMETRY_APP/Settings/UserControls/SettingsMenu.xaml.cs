using ART_TELEMETRY_APP.Groups.UserControls;
using ART_TELEMETRY_APP.Tracks.UserControls;
using ART_TELEMETRY_APP.Sectors.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Collections.Generic;
using System.Windows.Controls;
using ART_TELEMETRY_APP.Settings.UserControls;

namespace ART_TELEMETRY_APP.Settings
{
    /// <summary>
    /// Interaction logic for <seealso cref="SettingsMenu"/>.xaml
    /// </summary>
    public partial class SettingsMenu : UserControl
    {
        private static readonly List<TabItem> settingsTabs = new List<TabItem>();

        public SettingsMenu()
        {
            InitializeComponent();
            InitSettingsTabs();
        }

        public void InitSettingsTabs()
        {
            settingsTabControl.Items.Clear();

            AddSettingsTab(new TabItem
            {
                Header = TextManager.TracksSettingsName,
                Content = new TrackSettings(),
                IsSelected = true
            });

            AddSettingsTab(new TabItem
            {
                Header = TextManager.SectorsSettingsName,
                // Content = new SectorsSettings()
            });

            AddSettingsTab(new TabItem
            {
                Header = TextManager.GroupsSettingsName,
                Content = new GroupSettings()
            });

            AddSettingsTab(new TabItem
            {
                Header = TextManager.GeneralSettingsName,
                Content = new GeneralSettings()
            });
        }

        private void AddSettingsTab(TabItem tabItem)
        {
            settingsTabs.Add(tabItem);
            settingsTabControl.Items.Add(tabItem);
        }

        public TabItem GetTab(string name) => settingsTabs.Find(x => x.Header.Equals(name));
    }
}
