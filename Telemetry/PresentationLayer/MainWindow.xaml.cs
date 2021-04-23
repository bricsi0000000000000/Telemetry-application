﻿using System;
using System.Windows;
using LocigLayer.Groups;
using LocigLayer.Texts;
using LocigLayer.Tracks;
using LocigLayer.Units;
using LogicLayer.Configurations;
using LogicLayer.Errors;
using LogicLayer.Menus;
using LogicLayer.Menus.Live;
using PresentationLayer.Defaults;
using PresentationLayer.Extensions;

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
                UnitOfMeasureManager.InitializeUnitOfMeasures(TextManager.UnitOfMeasuresFileName.MakePath("default_files"));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }

            try
            {
                DriverlessTrackManager.LoadTracks(TextManager.DriverlessTracksFolderName.MakePath("default_files"));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }

            try
            {
                GroupManager.InitGroups(TextManager.GroupsFileName.MakePath("default_files"));
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message, nameof(MainWindow));
            }


            try
            {
                DefaultsManager.LoadDefaults(TextManager.DefaultFileName.MakePath("default_files"));
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
