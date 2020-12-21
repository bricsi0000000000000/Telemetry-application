using ART_TELEMETRY_APP.Driverless.UserControls;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Groups.Windows;
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
    /// This represents one single group attribute
    /// </summary>
    public partial class GroupSettingsAttribute : UserControl
    {
        public string AttributeName { get; set; }

        private readonly string groupName;

        public GroupSettingsAttribute(string channelName, string groupName, Color color)
        {
            InitializeComponent();

            AttributeName = channelName;
            this.groupName = groupName;
            AttributeLbl.Content = channelName;
            ChangeColorBtn.Background = new SolidColorBrush(color);
        }

        private void DeleteAttribute_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.GetGroup(groupName).RemoveAttribute(AttributeName);
            if (GroupManager.Groups.Count > 0)
            {
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveAttribute = GroupManager.Groups.First().Attributes.First();
            }
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitActiveChannelSelectableAttributes();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveAttribute =
                 GroupManager.GetGroup(groupName).GetAttribute(AttributeName);
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitAttributes();
        }

        public void ChangeColorMode(bool change)
        {
            ColorZone.Mode = change ?
                             MaterialDesignThemes.Wpf.ColorZoneMode.Inverted :
                             MaterialDesignThemes.Wpf.ColorZoneMode.Dark;

            AttributeLbl.Foreground = change ? Brushes.Black : Brushes.White;
        }

        private void ChangeColorBtn_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickColor = new PickColor();
            if (pickColor.ShowDialog() == true)
            {
                GroupManager.GetGroup(groupName).GetAttribute(AttributeName).Color = pickColor.ColorPicker.Color;
                GroupManager.SaveGroups();
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitAttributes();
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateCharts();
            }
        }
    }
}
