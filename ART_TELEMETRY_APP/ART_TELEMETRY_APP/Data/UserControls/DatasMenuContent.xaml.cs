using ART_TELEMETRY_APP.Pilots;
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

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// On this page will be shown all the drivers with charts
    /// </summary>
    public partial class DatasMenuContent : UserControl
    {
        private readonly List<TabItem> driver_tabs = new List<TabItem>();

        public DatasMenuContent()
        {
            InitializeComponent();

            InitDriversTabs();
        }

        public void InitDriversTabs()
        {
            drivers_tabcontrol.Items.Clear();
            foreach (Driver driver in DriverManager.Drivers)
            {
                TabItem item = new TabItem();
                item.Header = driver.Name;
                item.Content = new DriverContentTab(driver);
                item.Name = string.Format("{0}_item", driver.Name);
                drivers_tabcontrol.Items.Add(item);
                driver_tabs.Add(item);
            }
        }

        public TabItem GetTab(string name) => driver_tabs.Find(n => n.Header.Equals(name));
    }
}
