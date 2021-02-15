using System;
using System.Windows;
using Telemetry_data_and_logic_layer;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_data_and_logic_layer.Tracks;
using Telemetry_data_and_logic_layer.Units;
using Telemetry_presentation_layer.Errors;
using Telemetry_presentation_layer.Menus;
using Telemetry_presentation_layer.Menus.Live;

namespace Telemetry_presentation_layer
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
                GroupManager.InitGroups(TextManager.GroupsFileName);
                MenuManager.InitMainMenuTabs(MainMenuTabControl);
                DriverlessTrackManager.LoadTracks();
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
