using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Drivers.UserControls
{
    /// <summary>
    /// On this page will be shown all the charts
    /// </summary>
    public partial class DriverContentTab : UserControl
    {
        public List<TabItem> Tabs { get; } = new List<TabItem>();

        private readonly Driver driver;

        public DriverContentTab(Driver driver)
        {
            InitializeComponent();

            this.driver = driver;

            InitTabs();
        }

        public void InitTabs()
        {
            TabControl.Items.Clear();
            foreach (Group group in GroupManager.Groups)
            {
                TabItem group_item = new TabItem
                {
                    Header = group.Name,
                    Content = new LapsContent(driver, group),
                    Name = string.Format("{0}GroupTabItem", driver.Name)
                };
                Tabs.Add(group_item);
                TabControl.Items.Add(group_item);
            }

            TabItem item = new TabItem
            {
                Header = TextManager.DiagramCustomTabName,
                Content = new LapsContent(driver, null),
                Name = string.Format("{0}GroupTabItem", driver.Name)
            };
            Tabs.Add(item);
            TabControl.Items.Add(item);

            /* item = new TabItem();
             item.Header = "Track";
             item.Content = new TrackContent();
             item.Name = string.Format("{0}_item_track", pilot.Name);
             tabs.Add(item);
             tabcontrol.Items.Add(item);

             item = new TabItem();
             item.Header = "Traction";
             item.Content = new GGDiagram_UC();
             item.Name = string.Format("{0}_item_traction", pilot.Name);
             tabs.Add(item);
             tabcontrol.Items.Add(item);

             item = new TabItem();
             item.Header = "LapReport";
             //item.Content = new LapsContent(pilot);
             item.Name = string.Format("{0}_item_lapreport", pilot.Name);
             tabs.Add(item);
             tabcontrol.Items.Add(item);*/
        }

        public TabItem GetTab(string name) => Tabs.Find(x => x.Header.Equals(name));
    }
}
