using System;
using System.Windows;
using PresentationLayer.Groups;
using PresentationLayer.Texts;
using PresentationLayer.Tracks;
using PresentationLayer.Units;
using LogicLayer.Configurations;
using LogicLayer.Errors;
using LogicLayer.Menus;
using LogicLayer.Menus.Live;
using PresentationLayer.Defaults;
using LogicLayer.Extensions;

namespace LogicLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            try
            {
                ConfigurationManager.LoadConfigurations(TextManager.ConfigurationFileName.MakePath("configuration_files"));
                Title = $"Telemetry {ConfigurationManager.Version}";
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }

            try
            {
                UnitOfMeasureManager.InitializeUnitOfMeasures(TextManager.UnitOfMeasuresFileName.MakePath(TextManager.DefaultFilesFolderName));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }

            try
            {
                DriverlessTrackManager.LoadTracks(TextManager.DriverlessTracksFolderName.MakePath(TextManager.DefaultFilesFolderName));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }

            try
            {
                GroupManager.InitGroups(TextManager.GroupsFileName.MakePath(TextManager.DefaultFilesFolderName));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }


            try
            {
                DefaultsManager.LoadDefaults(TextManager.DefaultFileName.MakePath(TextManager.DefaultFilesFolderName));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }

            try
            {
                MenuManager.InitMainMenuTabs(MainMenuTabControl);
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MenuManager.LiveTelemetry.CanUpdateCharts)
            {
                ShowError.ShowErrorMessage("Stop the data stream before closing application", nameof(MainWindow));
                e.Cancel = true;
            }
        }
    }
}
