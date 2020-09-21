using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    static class MenuManager
    {
        private readonly static List<TabItem> menuItems = new List<TabItem>();

        public static void InitMainMenuTabs(TabControl tabControl)
        {
            TabItem menuTab = new TabItem
            {
                Header = TextManager.SettingsMenuName,
                Content = new SettingsMenu(),
                Name = "settingsMenuTab"
            };
            menuItems.Add(menuTab);
            tabControl.Items.Add(menuTab);

            menuTab = new TabItem
            {
                Header = TextManager.DiagramsMenuName,
                Content = new DiagramsMenu(),
                Name = "diagramsMenuTab"
            };
            menuItems.Add(menuTab);
            tabControl.Items.Add(menuTab);

            menuTab = new TabItem
            {
                Header = TextManager.DriversMenuName,
                Content = new DriversMenu(),
                Name = "pilotsMenuTab",
                IsSelected = true
            };
            menuItems.Add(menuTab);
            tabControl.Items.Add(menuTab);
        }

        public static TabItem GetTab(string name) => menuItems.Find(x => x.Header.Equals(name));
    }
}
