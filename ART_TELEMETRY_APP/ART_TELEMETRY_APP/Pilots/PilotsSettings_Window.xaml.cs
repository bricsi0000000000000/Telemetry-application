using ART_TELEMETRY_APP.Settings;
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
    public partial class PilotsSettings_Window : Window
    {
        public PilotsSettings_Window()
        {
            InitializeComponent();

            initPilotTabs();
        }

        private void initPilotTabs()
        {
            if (PilotManager.Pilots.Count > 0)
            {
                pilots_nothing.Visibility = Visibility.Hidden;
            }

            foreach (Pilot pilot in PilotManager.Pilots)
            {
                TabItem item = new TabItem();
                item.Header = pilot.Name;
                item.IsSelected = true;
                item.Content = new PilotTab_UC(pilot);

                pilots_tabs.Items.Add(item);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PilotManager.SettingsIsOpen = false;
        }

        private void addPilotTxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!addPilotTxtbox.Text.Equals(string.Empty))
                {
                    if (PilotManager.GetPilot(addPilotTxtbox.Text) == null)
                    {
                        Pilot pilot = new Pilot(addPilotTxtbox.Text);
                        PilotManager.AddPilot(pilot);

                        TabItem item = new TabItem();
                        item.Header = pilot.Name;
                        item.IsSelected = true;
                        item.Content = new PilotTab_UC(pilot, error_snack_bar);

                        pilots_tabs.Items.Add(item);
                        SettingsManager.UpdatePilotsTabs(pilot);

                        if (PilotManager.Pilots.Count > 0)
                        {
                            pilots_nothing.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        error_snack_bar.MessageQueue.Enqueue(string.Format("{0} pilot is already exists!", addPilotTxtbox.Text),
                                                             null, null, null, false, true, TimeSpan.FromSeconds(1));
                    }
                }
                else
                {
                    error_snack_bar.MessageQueue.Enqueue("Pilot's name is empty!",
                                                         null, null, null, false, true,
                                                         TimeSpan.FromSeconds(1));
                }

                addPilotTxtbox.Text = string.Empty;
            }
        }
    }
}
