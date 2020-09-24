using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Interaction logic for <seealso cref="GroupSettingsItem"/>.xaml
    /// </summary>
    public partial class GroupSettingsItem : UserControl
    {
        public string GroupName { get; set; }

        public GroupSettingsItem(string groupName)
        {
            InitializeComponent();

            GroupName = groupName;
            GroupLbl.Content = groupName;
        }

        private void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.RemoveGroup(GroupName);
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveGroupName = GroupManager.Groups.First().Name;
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();

            GroupManager.SaveGroups();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveGroupName = GroupName; 
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups(); 
        }

        public void ChangeColorMode(bool change)
        {
            ColorZone.Mode = change ?
                            MaterialDesignThemes.Wpf.ColorZoneMode.Inverted :
                            MaterialDesignThemes.Wpf.ColorZoneMode.Dark;

            GroupLbl.Foreground = change ? Brushes.Black : Brushes.White;
        }
    }
}
