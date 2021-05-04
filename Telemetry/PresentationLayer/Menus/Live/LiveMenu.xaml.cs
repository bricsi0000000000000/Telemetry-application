using System.Collections.Generic;
using System.Windows.Controls;
using PresentationLayer.Texts;
using LogicLayer.Menus.Settings.Live;

namespace LogicLayer.Menus.Live
{
    public partial class LiveMenu : UserControl
    {
        /// <summary>
        /// Menu items.
        /// </summary>
        private readonly List<TabItem> tabs = new List<TabItem>();

        public LiveMenu()
        {
            InitializeComponent();

            InitializeTabs();
        }

        private void InitializeTabs()
        {
            TabItem liveSettingsTab = MakeTab(TextManager.SettingsMenuName, new LiveSettings(), "liveSettingsTab", selected: true);
            MenuManager.LiveSettings = (LiveSettings)liveSettingsTab.Content;
            AddTab(liveSettingsTab);

            TabItem liveTelemetryTab = MakeTab(TextManager.LiveMenuName, new LiveTelemetry(), "liveTelemetryTab", selected: false);
            MenuManager.LiveTelemetry = (LiveTelemetry)liveTelemetryTab.Content;
            AddTab(liveTelemetryTab);
        }

        private TabItem MakeTab(string header, object content, string name, bool selected = false)
        {
            return new TabItem
            {
                Header = header,
                Content = content,
                Name = name,
                IsSelected = selected
            };
        }

        private void AddTab(TabItem tab)
        {
            tabs.Add(tab);
            TabsTabControl.Items.Add(tab);
        }

        /// <summary>
        /// Finds a <see cref="TabItem"/> in <see cref="tabs"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="TabItem"/>s name.</param>
        /// <returns>A <see cref="TabItem"/> whose name is <paramref name="name"/>.</returns>
        public TabItem GetTab(string name) => tabs.Find(x => x.Header.Equals(name));
    }
}
