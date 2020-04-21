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

namespace ART_TELEMETRY_APP.Pilots
{
    /// <summary>
    /// Interaction logic for PilotsMenuContent.xaml
    /// </summary>
    public partial class PilotsMenuContent : UserControl
    {
        public PilotsMenuContent()
        {
            InitializeComponent();

            InitPilots();
        }

        public void InitPilots()
        {
            pilots_wrappanel.Children.Clear();
            foreach (Pilot pilot in PilotManager.Pilots)
            {
                PilotSettings pilot_settings = new PilotSettings(pilot.Name);
                pilots_wrappanel.Children.Add(pilot_settings);
            }
        }

        private void addPilotTxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PilotManager.AddPilot(new Pilot(addPilot_txtbox.Text));
                PilotSettings pilot_settings = new PilotSettings(addPilot_txtbox.Text);
                pilots_wrappanel.Children.Add(pilot_settings);

                addPilot_txtbox.Text = string.Empty;

                ((DatasMenuContent)TabManager.GetTab("Datas").Content).InitPilotsTabs();
            }
        }
    }
}
