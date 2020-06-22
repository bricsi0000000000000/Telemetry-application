using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Interaction logic for GroupSettingsAttribute.xaml
    /// </summary>
    public partial class GroupSettingsAttribute : UserControl
    {
        string attribute;
        string group_name;

        public GroupSettingsAttribute(string attribute, string group_name)
        {
            InitializeComponent();

            this.attribute = attribute;
            this.group_name = group_name;
            attribute_lbl.Content = attribute;
        }

        private void deleteAttribute_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.GetGroup(group_name).RemoveAttribute(attribute);
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).ActiveAttribute = GroupManager.Groups.First().Attributes.First();
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).InitGroups();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).ActiveAttribute = attribute;
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

        public string Attribute
        {
            get
            {
                return attribute;
            }
            set
            {
                attribute = value;
            }
        }
    }
}
