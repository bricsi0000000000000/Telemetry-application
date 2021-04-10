using System.Collections.Generic;
using System.Windows.Controls;
using LocigLayer.Texts;
using PresentationLayer.Menus.Driverless;
using PresentationLayer.Menus.Live;
using PresentationLayer.Menus.Settings;

namespace PresentationLayer.Menus
{
    /// <summary>
    /// Manages menus.
    /// </summary>
    public static class MenuManager
    {
        /// <summary>
        /// Menu items.
        /// </summary>
        private readonly static List<TabItem> menuItems = new List<TabItem>();

        /// <summary>
        /// Initializes main menu tabs.
        /// </summary>
        /// <param name="tabControl"><see cref="TabControl"/>, where the tabs will created.</param>
        public static void InitMainMenuTabs(TabControl tabControl)
        {
            AddTab(TextManager.SettingsMenuName, new SettingsMenu(), "settingsMenuTab", tabControl);
            AddTab(TextManager.DriverlessMenuName, new DriverlessMenu(), "driverlessMenuTab", tabControl, selected: true);
            AddTab(TextManager.LiveMenuName, new LiveMenu(), "liveMenuTab", tabControl);
          //  AddTab(TextManager.DriversMenuName, new DriversMenu(), "driversMenuTab", tabControl, false);
           // AddTab(TextManager.DiagramsMenuName, new Diagrams(), "diagramsMenuTab", tabControl, false);
          //  AddTab(TextManager.DiagramsSettingsMenuName, new SelectDriversAndInputFiles(), "diagramsSettingsMenuTab", tabControl, false);
        }

        /// <summary>
        /// Adds a newly created <see cref="TabItem"/> to the <paramref name="tabControl"/>s items.
        /// </summary>
        /// <param name="header"><see cref="TabItem"/>s header.</param>
        /// <param name="content">
        /// <see cref="TabItem"/>s content.
        /// It can be anything, I use it with a <see cref="UserControl"/>.
        /// </param>
        /// <param name="name"><see cref="TabItem"/>s name.</param>
        /// <param name="tabControl"><see cref="TabControl"/>, where the newly created <see cref="TabItem"/> will be.</param>
        /// <param name="selected">
        /// If true, the <see cref="TabItem"/> will be selected.
        /// If false, the <see cref="TabItem"/> will not be selected.
        /// Default is false.
        /// </param>
        private static void AddTab(string header, object content, string name, TabControl tabControl, bool selected = false)
        {
            TabItem tab = new TabItem
            {
                Header = header,
                Content = content,
                Name = name,
                IsSelected = selected
            };

            menuItems.Add(tab);
            tabControl.Items.Add(tab);
        }

        /// <summary>
        /// Finds a <see cref="TabItem"/> in <see cref="menuItems"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="TabItem"/>s name.</param>
        /// <returns>A <see cref="TabItem"/> whose name is <paramref name="name"/>.</returns>
        public static TabItem GetTab(string name) => menuItems.Find(x => x.Header.Equals(name));
    }
}
