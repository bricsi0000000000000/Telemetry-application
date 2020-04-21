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
    /// Interaction logic for DatasMenuContent.xaml
    /// </summary>
    public partial class DatasMenuContent : UserControl
    {
        List<TabItem> pilot_tabs = new List<TabItem>();

        public DatasMenuContent()
        {
            InitializeComponent();

            InitPilotsTabs();
        }

        public void InitPilotsTabs()
        {
            pilots_tabcontrol.Items.Clear();
            foreach (Pilot pilot in PilotManager.Pilots)
            {
                TabItem item = new TabItem();
                item.Header = pilot.Name;
                item.Content = new PilotContentTab(pilot);
                item.Name = string.Format("{0}_item", pilot.Name);
                pilots_tabcontrol.Items.Add(item);
                pilot_tabs.Add(item);
            }
        }

        public TabItem GetTab(string name)
        {
            return pilot_tabs.Find(n => n.Header.Equals(name));
        }
    }
}
