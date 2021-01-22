using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Errors;
using Telemetry_presentation_layer.Menus.Driverless;

namespace Telemetry_presentation_layer.Menus.Settings.Groups
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
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageRelativePath = "Images/daisy.png";
                string imagePath = Path.Combine(baseDirectory, imageRelativePath);

                logo.UriSource = new Uri(imagePath);
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageRelativePath = "Images/art_banner.png";
                string imagePath = Path.Combine(baseDirectory, imageRelativePath);

                logo.UriSource = new Uri(imagePath);
            }

            logo.EndInit();

            GroupTypeImage.Source = logo;
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
            BackgroundColor.Background = change ? (Brush)converter.ConvertFromString("#3c3c3c") : Brushes.White;
            GroupLbl.Foreground = !change ? (Brush)converter.ConvertFromString("#3c3c3c") : Brushes.White;
        }

        private void DeleteGroupBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteGroupBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary500));
        }

        private void DeleteGroupBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteGroupBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
        }

        private void DeleteGroupBtn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeleteGroupBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary300));
        }

        private void DeleteGroupBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DeleteGroupBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary500));

            GroupManager.RemoveGroup(GroupName);
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveGroupName = GroupManager.Groups[0].Name;
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();

            GroupManager.SaveGroups();

            // ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).InitTabs();
        }

        private void ChangeGroupItemType_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeGroupItemType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary200));
        }

        private void ChangeGroupItemType_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeGroupItemType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
        }

        private void ChangeGroupItemType_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeGroupItemType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary400));
        }

        private void ChangeGroupItemType_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChangeGroupItemType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary200));

            var group = GroupManager.GetGroup(GroupName);
            if (group == null)
            {
                ShowError.ShowErrorMessage($"Group '{GroupName}' is empty!");
                return;
            }

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

            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateAfterReadFile();
        }
    }
}
