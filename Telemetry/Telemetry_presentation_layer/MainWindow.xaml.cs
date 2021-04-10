using System;
using System.Windows;
using LocigLayer.Groups;
using LocigLayer.Texts;
using LocigLayer.Tracks;
using LocigLayer.Units;
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
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                UnitOfMeasureManager.InitializeUnitOfMeasures(TextManager.UnitOfMeasuresFileName);
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message);
            }

            try
            {
                DriverlessTrackManager.LoadTracks();
            }
            catch (Exception exception)
            {
                ShowError.ShowErrorMessage(exception.Message);
            }

            try
            {
                GroupManager.InitGroups(TextManager.GroupsFileName);
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
            var liveMenuTab = MenuManager.GetTab(TextManager.LiveMenuName);
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
