using ART_TELEMETRY_APP.Driverless.UserControls;
using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Settings.UserControls;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    static class MenuManager
    {
        private readonly static List<TabItem> menuItems = new List<TabItem>();

        public static void InitMainMenuTabs(TabControl tabControl)
        {
            AddTab(TextManager.DriverlessMenuName, new DriverlessMenu(), "driverlessMenuTab", tabControl);
            AddTab(TextManager.SettingsMenuName, new SettingsMenu(), "settingsMenuTab", tabControl);
            AddTab(TextManager.DriversMenuName, new DriversMenu(), "driversMenuTab", tabControl);
            AddTab(TextManager.DiagramsMenuName, new Diagrams(), "diagramsMenuTab", tabControl);
            AddTab(TextManager.DiagramsSettingsMenuName, new SelectDriversAndInputFiles(), "diagramsSettingsMenuTab", tabControl);
            //AddTab("test", new Experimental(), "test", tabControl);
        }

        private static void AddTab(string header, object content, string name, TabControl tabControl)
        {
            TabItem tab = new TabItem
            {
                Header = header,
                Content = content,
                Name = name
            };

            menuItems.Add(tab);
            tabControl.Items.Add(tab);
        }

        public static TabItem GetTab(string name) => menuItems.Find(x => x.Header.Equals(name));
    }
}
