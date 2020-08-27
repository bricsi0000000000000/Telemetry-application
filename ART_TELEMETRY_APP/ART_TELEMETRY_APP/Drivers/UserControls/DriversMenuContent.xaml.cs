using ART_TELEMETRY_APP.Settings.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Pilots
{
    /// <summary>
    /// On this page will be shown all the added <seealso cref="Driver"/>s
    /// </summary>
    public partial class DriversMenuContent : UserControl
    {
        private readonly List<DriverSettings> all_driver_settings = new List<DriverSettings>();

        public DriversMenuContent()
        {
            InitializeComponent();

            InitDrivers();
            StreamReader sr = new StreamReader("drivers.csv");
            while (!sr.EndOfStream)
            {
                string row = sr.ReadLine();
                DriverManager.AddDriver(new Driver(row));
                DriverSettings pilot_settings = new DriverSettings(row);
                all_driver_settings.Add(pilot_settings);
                drivers_wrappanel.Children.Add(pilot_settings);

                ((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).InitDriversTabs();

                updateAddDriverVisibility();
            }
            sr.Close();
        }

        public void InitDrivers()
        {
            drivers_wrappanel.Children.Clear();
            foreach (Driver driver in DriverManager.Drivers)
            {
                DriverSettings driver_settings = new DriverSettings(driver.Name);
                drivers_wrappanel.Children.Add(driver_settings);
            }
            updateAddDriverVisibility();
        }

        private void updateAddDriverVisibility() => noDrivers_lbl.Visibility = DriverManager.Drivers.Count == 0 ? Visibility.Visible : Visibility.Hidden;

        public void DisableAllDrivers(bool disable, string driver_name)
        {
            addDriver_btn.IsEnabled = !disable;
            foreach (DriverSettings driver_settings in all_driver_settings)
            {
                if (!driver_settings.DriverName.Equals(driver_name))
                {
                    driver_settings.disable_grid.Visibility = disable ? Visibility.Visible : Visibility.Hidden;
                }
            }

            initDrivers();
        }

        private void initDrivers()
        {
            drivers_wrappanel.Children.Clear();
            foreach (DriverSettings driver_settings in all_driver_settings)
            {
                drivers_wrappanel.Children.Add(driver_settings);
            }
        }

        public void ShowError(string message) => error_snack_bar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(1));

        private void addDriver_Click(object sender, RoutedEventArgs e)
        {
            if (addDriver_txtbox.Text.Equals(""))
            {
                error_snack_bar.MessageQueue.Enqueue(string.Format("Driver name is empty!"),
                                                     null, null, null, false, true, TimeSpan.FromSeconds(1));
            }
            else
            {
                DriverManager.AddDriver(new Driver(addDriver_txtbox.Text));
                DriverSettings pilot_settings = new DriverSettings(addDriver_txtbox.Text);
                all_driver_settings.Add(pilot_settings);
                drivers_wrappanel.Children.Add(pilot_settings);

                addDriver_txtbox.Text = string.Empty;

                ((DatasMenuContent)TabManager.GetTab("Diagrams").Content).InitDriversTabs();

                updateAddDriverVisibility();
            }
        }

        public DriverSettings GetDriverSetting(string name) => all_driver_settings.Find(n => n.DriverName.Equals(name));
    }
}
