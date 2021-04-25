using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataLayer.Groups;
using LogicLayer.Colors;
using LogicLayer.Extensions;
using PresentationLayer.Texts;

namespace LogicLayer.Menus.Settings.Groups
{
    /// <summary>
    /// Represents a <see cref="Group"/> settings item in <see cref="GroupSettings"/>.
    /// </summary>
    public partial class GroupSettingsItem : UserControl
    {
        private string groupName;
        /// <summary>
        /// The <see cref="Group"/>s name that is represented in this <see cref="GroupSettingsItem"/>.
        /// </summary>
        public string GroupName
        {
            get
            {
                return groupName;
            }
            set
            {
                groupName = value;
                GroupLabel.Content = groupName;
            }
        }

        private bool isSelected = false;

        public int ID { get; private set; }

        /// <summary>
        /// Constructor for <see cref="GroupSettingsItem"/>.
        /// </summary>
        /// <param name="groupName"><see cref="Group"/>s name which is represented by this <see cref="GroupSettingsItem"/>.</param>
        /// If true, this <see cref="GroupSettingsItem"/> is driverless.
        /// Otherwise it's not.
        /// </param>
        public GroupSettingsItem(Group group)
        {
            InitializeComponent();

            ID = group.ID;
            GroupName = group.Name;
        }

        /// <summary>
        /// Changes the active <see cref="GroupSettingsItem"/> to the clicked one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).GroupSettingsItemClicked(GroupName);
        }

        /// <summary>
        /// Changes color mode in this <see cref="GroupSettingsItem"/>.
        /// </summary>
        /// <param name="change"></param>
        public void ChangeColorMode(bool change)
        {
            isSelected = change;

            BackgroundColor.Background = isSelected ? ColorManager.Secondary900.ConvertBrush() :
                                                      ColorManager.Secondary50.ConvertBrush();

            GroupLabel.Foreground = isSelected ? ColorManager.Secondary50.ConvertBrush() :
                                                 ColorManager.Secondary900.ConvertBrush();
        }

        private void BackgroundColor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundColor.Background = isSelected ? ColorManager.Secondary700.ConvertBrush() :
                                                      ColorManager.Secondary200.ConvertBrush();
        }

        private void BackgroundColor_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BackgroundColor.Background = isSelected ? ColorManager.Secondary800.ConvertBrush() :
                                                      ColorManager.Secondary100.ConvertBrush();

            ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ChangeActiveGroupItem(ID);
            ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).SelectInputFile();
        }

        private void BackgroundColor_MouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundColor.Background = isSelected ? ColorManager.Secondary800.ConvertBrush() :
                                                      ColorManager.Secondary100.ConvertBrush();

            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void BackgroundColor_MouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundColor.Background = isSelected ? ColorManager.Secondary900.ConvertBrush() :
                                                      ColorManager.Secondary50.ConvertBrush();

            Mouse.OverrideCursor = null;
        }
    }
}
