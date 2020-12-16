using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Tracks.Classes;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TrackManager.LoadTracks(ref ErrorSnackbar);
            GroupManager.InitGroups(ref ErrorSnackbar);
            MenuManager.InitMainMenuTabs(MainMenuTabControl);
            DriverlessTrackManager.LoadTracks(ref ErrorSnackbar);
        }
    }
}


