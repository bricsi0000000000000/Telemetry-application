using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ART_TELEMETRY_APP.Drivers.Classes;

namespace ART_TELEMETRY_APP.Drivers.UserControls
{
    /// <summary>
    /// On this page will be shown all the added <seealso cref="Driver"/>s
    /// </summary>
    public partial class DriversMenu : UserControl
    {
        private static readonly List<DriverCard> driverCards = new List<DriverCard>();

        public DriversMenu()
        {
            InitializeComponent();
            ReadDrivers();
            InitDriverCards();
        }

        private void ReadDrivers()
        {
            try
            {
                DriverReader();
            }
            catch (FileNotFoundException)
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, string.Format("Couldn't load drivers, because file '{0}' not found!", TextManager.DriversCSV), 3);
            }
        }

        private void DriverReader()
        {
            using (var reader = new StreamReader(TextManager.DriversCSV, Encoding.Default))
            {
                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();
                    if (!row.Equals(string.Empty))
                    {
                        DriverManager.AddDriver(new Driver(row));
                    }
                }
            };
        }

        private void SaveDriver(Driver driver)
        {
            if (File.Exists(TextManager.DriversCSV))
            {
                using var writer = new StreamWriter(TextManager.DriversCSV, true);
                writer.WriteLine(driver.Name);
            }
            else
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, string.Format("Couldn't save '{0}' into '{1}', because file '{1}' not found!", driver.Name, TextManager.DriversCSV), 3);
            }
        }

        public void DeleteDriver(string driverName)
        {
            if (File.Exists(TextManager.DriversCSV))
            {
                using var writer = new StreamWriter(TextManager.DriversCSV, false);
                foreach (var driver in DriverManager.Drivers)
                {
                    if (!driver.Name.EndsWith(driverName))
                    {
                        writer.WriteLine(driver.Name);
                    }
                }
            }
            else
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, string.Format("Couldn't delete '{0}' from '{1}', because file '{1}' not found!", driverName, TextManager.DriversCSV), 3);
            }
        }

        public void InitDriverCards()
        {
            DriversWrappanel.Children.Clear();

            foreach (Driver driver in DriverManager.Drivers)
            {
                AddDriverCard(driver);
            }

            NoDriverAddedLbl.Visibility = DriverManager.Drivers.Count == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void AddDriverCard(Driver driver)
        {
            DriversWrappanel.Children.Add(new DriverCard(driver));
        }

         public void DisableAllDrivers(bool disable, string driverName)
         {
             AddDriverBtn.IsEnabled = !disable;
             foreach (var driverCard in driverCards)
             {
                 if (!driverCard.Driver.Name.Equals(driverName))
                 {
                     driverCard.DisableGrid.Visibility = disable ? Visibility.Visible : Visibility.Hidden;
                 }
             }
         }

        private void AddDriver_Click(object sender, RoutedEventArgs e)
        {
            string driverName = AddDriverTxtBox.Text;
            if (driverName.Equals(string.Empty))
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, "Driver name is empty!", 1);
            }
            else
            {
                AddDriverTxtBox.Text = string.Empty;

                var driver = new Driver(driverName);
                DriverManager.AddDriver(driver);
                AddDriverCard(driver);
                SaveDriver(driver);
            }
        }

        public DriverCard GetDriverSetting(string name) => driverCards.Find(x => x.Driver.Name.Equals(name));
    }
}
