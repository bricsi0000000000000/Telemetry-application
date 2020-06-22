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
    /// Interaction logic for GroupSettingsGroup.xaml
    /// </summary>
    public partial class GroupSettingsGroup : UserControl
    {
        string group_name;

        public GroupSettingsGroup(string group_name)
        {
            InitializeComponent();

            this.group_name = group_name;
            group_lbl.Content = group_name;
        }

        private void deleteGroup_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.RemoveGroup(group_name);
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).ActiveGroupName = GroupManager.Groups.First().Name;
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).InitGroups();
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenuContent)TabManager.GetTab("Settings").Content).GetTab("Groups").Content).ActiveGroupName = group_name; 
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

        public string Attribute
        {
            get
            {
                return group_name;
            }
            set
            {
                group_name = value;
            }
        }
    }
}
