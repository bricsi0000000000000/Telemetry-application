using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Drivers.UserControls;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// On this page will be shown all the drivers with charts
    /// </summary>
    public partial class DiagramsMenu : UserControl
    {
        private static readonly List<TabItem> driverTabs = new List<TabItem>();

        public DiagramsMenu()
        {
            InitializeComponent();
            InitDriversTabs();
        }

        public void InitDriversTabs()
        {
            driversTabControl.Items.Clear();
            foreach (Driver driver in DriverManager.Drivers)
            {
                TabItem driverTab = new TabItem
                {
                    Header = driver.Name,
                    Content = new DriverContentTab(driver)
                };
                driversTabControl.Items.Add(driverTab);
                driverTabs.Add(driverTab);
            }
        }

        public TabItem GetTab(string driverName) => driverTabs.Find(x => x.Header.Equals(driverName));
    }
}
