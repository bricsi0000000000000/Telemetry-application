using System;
using System.Collections.Generic;
using System.IO;
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
        List<PilotSettings> all_pilot_settings = new List<PilotSettings>();

        public PilotsMenuContent()
        {
            InitializeComponent();

            InitPilots();
          /*  StreamReader sr = new StreamReader("pilots.csv");
            while (!sr.EndOfStream)
            {
                /* string row = sr.ReadLine();
                 PilotManager.AddPilot(new Pilot(row));
                 PilotSettings pilot_settings = new PilotSettings(row);
                 all_pilot_settings.Add(pilot_settings);
                 pilots_wrappanel.Children.Add(pilot_settings);

                 ((DatasMenuContent)TabManager.GetTab("Diagrams").Content).InitPilotsTabs();
                */

               /* string row = sr.ReadLine();
                PilotManager.AddPilot(new Pilot(row));
                PilotSettings pilot_settings = new PilotSettings(row);
                all_pilot_settings.Add(pilot_settings);
                pilots_wrappanel.Children.Add(pilot_settings);

                ((DatasMenuContent)TabManager.GetTab("Diagrams").Content).InitPilotsTabs();

                updateAddPilotVisibility();
            }
            sr.Close();*/
        }

        public void InitPilots()
        {
            pilots_wrappanel.Children.Clear();
            foreach (Pilot pilot in PilotManager.Pilots)
            {
                PilotSettings pilot_settings = new PilotSettings(pilot.Name);
                pilots_wrappanel.Children.Add(pilot_settings);
            }
            updateAddPilotVisibility();
        }

        private void updateAddPilotVisibility()
        {
            noPilots_lbl.Visibility = PilotManager.Pilots.Count == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        public void DisableAllPilots(bool disable, string pilots_name)
        {
            addPilot_btn.IsEnabled = !disable;
            foreach (PilotSettings pilot_settings in all_pilot_settings)
            {
                if (!pilot_settings.PilotsName.Equals(pilots_name))
                {
                    pilot_settings.disable_grid.Visibility = disable ? Visibility.Visible : Visibility.Hidden;
                }
            }

            initPilots();
        }

        private void initPilots()
        {
            pilots_wrappanel.Children.Clear();
            foreach (PilotSettings pilot_settings in all_pilot_settings)
            {
                pilots_wrappanel.Children.Add(pilot_settings);
            }
        }

        public void ShowError(string message)
        {
            error_snack_bar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(1));
        }

        private void addPilot_Click(object sender, RoutedEventArgs e)
        {
            if (addPilot_txtbox.Text.Equals(""))
            {
                error_snack_bar.MessageQueue.Enqueue(string.Format("Pilot's name is empty!"),
                                                     null, null, null, false, true, TimeSpan.FromSeconds(1));
            }
            else
            {
                PilotManager.AddPilot(new Pilot(addPilot_txtbox.Text));
                PilotSettings pilot_settings = new PilotSettings(addPilot_txtbox.Text);
                all_pilot_settings.Add(pilot_settings);
                pilots_wrappanel.Children.Add(pilot_settings);

                addPilot_txtbox.Text = string.Empty;

                ((DatasMenuContent)TabManager.GetTab("Diagrams").Content).InitPilotsTabs();

                updateAddPilotVisibility();
            }
        }
    }
}
