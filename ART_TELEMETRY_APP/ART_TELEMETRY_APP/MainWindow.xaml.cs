using System.Windows;
using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Groups.Classes;

namespace ART_TELEMETRY_APP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MapManager.LoadMaps();
            GroupManager.InitGroups();
            TabManager.InitMenuItems(menu_tabcontrol);
        }
    }
}


