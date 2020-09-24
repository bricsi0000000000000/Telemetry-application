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
    /// This represents one single group attribute
    /// </summary>
    public partial class GroupSettingsAttribute : UserControl
    {
        public string Attribute { get; set; }

        private readonly string groupName;

        public GroupSettingsAttribute(string attribute, string groupName)
        {
            InitializeComponent();

            Attribute = attribute;
            this.groupName = groupName;
            AttributeLbl.Content = attribute;
        }

        private void DeleteAttribute_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.GetGroup(groupName).RemoveAttribute(Attribute);
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveAttribute = GroupManager.Groups.First().Attributes.First();
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveAttribute = Attribute;
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitAttributes();
        }

        public void ChangeColorMode(bool change)
        {
            ColorZone.Mode = change ?
                             MaterialDesignThemes.Wpf.ColorZoneMode.Inverted :
                             MaterialDesignThemes.Wpf.ColorZoneMode.Dark;

            AttributeLbl.Foreground = change ? Brushes.Black : Brushes.White;
        }
    }
}
