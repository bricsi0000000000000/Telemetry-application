using System.Windows;

namespace ART_TELEMETRY_APP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TrackManager.LoadTracks(ref ErrorSnackbar);
            //GroupManager.InitGroups();
            MenuManager.InitMainMenuTabs(MainMenuTabControl);
        }
    }
}


