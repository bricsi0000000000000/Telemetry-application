using System;
using System.Windows;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_data_and_logic_layer.Tracks;
using Telemetry_presentation_layer.Errors;
using Telemetry_presentation_layer.Menus;

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
                GroupManager.InitGroups(TextManager.GroupsFileName);
                MenuManager.InitMainMenuTabs(MainMenuTabControl);
                DriverlessTrackManager.LoadTracks();
            }
            catch (Exception e)
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, e.Message, 4);
            }
        }
    }
}
