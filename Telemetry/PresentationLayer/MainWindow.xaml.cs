using System;
using System.IO;
using System.Windows;
using LocigLayer.Defaults;
using LocigLayer.Groups;
using LocigLayer.Texts;
using LocigLayer.Tracks;
using LocigLayer.Units;
using LogicLayer.Configurations;
using PresentationLayer.Errors;
using PresentationLayer.Menus;
using PresentationLayer.Menus.Live;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ROOT_DIRECTORY = @"..\..\..\..\..\";

        private string MakeDirectoryPath(string folder) => $"{ROOT_DIRECTORY}/{folder}";

        public MainWindow()
        {
            InitializeComponent();


            try
            {
                ConfigurationManager.LoadConfigurations(Path.Combine(MakeDirectoryPath("configuration_files"), TextManager.ConfigurationFileName));
                Title = $"Telemetry {ConfigurationManager.Version}";
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message);
            }

            try
            {
                UnitOfMeasureManager.InitializeUnitOfMeasures(Path.Combine(MakeDirectoryPath("default_files"), TextManager.UnitOfMeasuresFileName));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message);
            }

            try
            {
                DriverlessTrackManager.LoadTracks(Path.Combine(MakeDirectoryPath("default_files"), TextManager.DriverlessTracksFolderName));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message);
            }

            try
            {
                GroupManager.InitGroups(Path.Combine(MakeDirectoryPath("default_files"), TextManager.GroupsFileName));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message);
            }


            try
            {
                DefaultsManager.LoadDefaults(Path.Combine(MakeDirectoryPath("default_files"), TextManager.DefaultFileName));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message);
            }

            try
            {
                MenuManager.InitMainMenuTabs(MainMenuTabControl);
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var liveMenuTab = MenuManager.GetMenuTab(TextManager.LiveMenuName);
            if (liveMenuTab != null)
            {
                var liveMenuTab1 = ((LiveMenu)liveMenuTab.Content).GetTab(TextManager.LiveMenuName);
                if (liveMenuTab1 != null)
                {
                    ((LiveTelemetry)liveMenuTab1.Content).Stop();
                }
            }
        }
    }
}
