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
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Pilots
{
    /// <summary>
    /// Interaction logic for PilotsSettings.xaml
    /// </summary>
    public partial class PilotsSettings : Window
    {
        public PilotsSettings()
        {
            InitializeComponent();

        }

        private void addPilotClick(object sender, RoutedEventArgs e)
        {
            Pilot pilot = new Pilot(addPilotTxtbox.Text);
            PilotManager.AddPilot(pilot);

            TabItem item = new TabItem();
            item.Header = pilot.Name;
            item.IsSelected = true;
            pilots_tabs.Items.Add(item);

            addPilotTxtbox.Text = string.Empty;
        }
    }
}
