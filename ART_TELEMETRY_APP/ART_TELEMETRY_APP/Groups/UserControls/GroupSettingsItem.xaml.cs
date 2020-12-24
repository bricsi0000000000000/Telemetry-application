using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Represents a <see cref="Group"/> settings item in <see cref="GroupSettings"/>.
    /// </summary>
    public partial class GroupSettingsItem : UserControl
    {
        /// <summary>
        /// The <see cref="Group"/>s name that is represented in this <see cref="GroupSettingsItem"/>.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Decides that the <see cref="Group"/> is driverless or not.
        /// </summary>
        private bool driverless;

        /// <summary>
        /// Constructor for <see cref="GroupSettingsItem"/>.
        /// </summary>
        /// <param name="groupName"><see cref="Group"/>s name which is represented by this <see cref="GroupSettingsItem"/>.</param>
        /// <param name="driverless">
        /// If true, this <see cref="GroupSettingsItem"/> is driverless.
        /// Otherwise it's not.
        /// </param>
        public GroupSettingsItem(string groupName, bool driverless = false)
        {
            InitializeComponent();

            GroupName = groupName;
            GroupLbl.Content = groupName;
            this.driverless = driverless;
            ChangeTypeImage();
        }

        /// <summary>
        /// Changes the <see cref="GroupTypeImage"/>es image based on <see cref="driverless"/>.
        /// </summary>
        private void ChangeTypeImage()
        {
            var logo = new BitmapImage();
            logo.BeginInit();

            if (driverless)
            {
                logo.UriSource = new Uri("pack://application:,,,/ART_TELEMETRY_APP;component/Images/daisy.png");
            }
            else
            {
                logo.UriSource = new Uri("pack://application:,,,/ART_TELEMETRY_APP;component/Images/art_banner.png");
            }

            logo.EndInit();

            GroupTypeImage.Source = logo;
        }

        /// <summary>
        /// Deletes this <see cref="GroupSettingsItem"/> than updates ActiveGroupName and initializes groups in <see cref="GroupSettings"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.RemoveGroup(GroupName);
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveGroupName = GroupManager.Groups.First().Name;
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();

            GroupManager.SaveGroups();

           // ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).InitTabs();
        }

        /// <summary>
        /// Changes the active <see cref="GroupSettingsItem"/> to the clicked one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).GroupSettingsItemClicked(GroupName);
        }

        /// <summary>
        /// Changes color mode in this <see cref="GroupSettingsItem"/>.
        /// </summary>
        /// <param name="change"></param>
        public void ChangeColorMode(bool change)
        {
            var converter = new BrushConverter();
            ColorZone.BorderBrush = change ? Brushes.White : (Brush)converter.ConvertFromString("#FF303030");
        }

        /// <summary>
        /// Changes that this <see cref="GroupSettingsItem"/> is driverless or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeGroupItemType_Click(object sender, RoutedEventArgs e)
        {
            var group = GroupManager.GetGroup(GroupName);
            group.Driverless = !group.Driverless;
            driverless = group.Driverless;
            GroupManager.SaveGroups();
           // ChangeTypeImage();
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();

            if (group.Driverless)
            {
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitActiveChannelSelectableAttributes();
            }
            else
            {
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).DestroyAllActiveChannelSelectableAttributes();
            }

            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitInputFilesComboBox();
        }
    }
}
