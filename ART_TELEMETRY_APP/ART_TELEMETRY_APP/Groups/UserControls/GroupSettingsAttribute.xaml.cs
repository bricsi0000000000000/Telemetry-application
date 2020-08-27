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
    /// This represents one single group attribute
    /// </summary>
    public partial class GroupSettingsAttribute : UserControl
    {
        public string Attribute { get; set; }

        private readonly string group_name;

        public GroupSettingsAttribute(string attribute, string group_name)
        {
            InitializeComponent();

            Attribute = attribute;
            this.group_name = group_name;
            attribute_lbl.Content = attribute;
        }

        private void deleteAttribute_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.GetGroup(group_name).RemoveAttribute(Attribute);
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).ActiveAttribute = GroupManager.Groups.First().Attributes.First();
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).InitGroups();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).ActiveAttribute = Attribute;
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).InitAttributes();
        }

        public void ChangeColorMode(bool change)
        {
            if (change)
            {
                colorZone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Inverted;
                attribute_lbl.Foreground = Brushes.Black;
            }
            else
            {
                colorZone.Mode = MaterialDesignThemes.Wpf.ColorZoneMode.Dark;
                attribute_lbl.Foreground = Brushes.White;
            }
        }
    }
}
