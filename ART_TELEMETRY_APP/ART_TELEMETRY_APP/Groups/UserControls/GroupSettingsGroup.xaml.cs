using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Settings;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Interaction logic for GroupSettingsGroup.xaml
    /// </summary>
    public partial class GroupSettingsGroup : UserControl
    {
        public string GroupName { get; set; }

        public GroupSettingsGroup(string group_name)
        {
            InitializeComponent();

            GroupName = group_name;
            group_lbl.Content = group_name;
        }

        private void deleteGroup_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.RemoveGroup(GroupName);
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).ActiveGroupName = GroupManager.Groups.First().Name;
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).InitGroups();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).ActiveGroupName = GroupName; 
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).InitGroups(); 
        }

        public void ChangeColorMode(bool change)
        {
            if (change)
            {
                colorZone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Inverted;
                group_lbl.Foreground = Brushes.Black;
            }
            else
            {
                colorZone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Dark;
                group_lbl.Foreground = Brushes.White;
            }
        }
    }
}
