using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataLayer.Groups;
using LocigLayer.Colors;
using LocigLayer.Texts;

namespace PresentationLayer.Menus.Settings.Groups
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
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).GroupSettingsItemClicked(GroupName);
        }

        /// <summary>
        /// Changes color mode in this <see cref="GroupSettingsItem"/>.
        /// </summary>
        /// <param name="change"></param>
        public void ChangeColorMode(bool change)
        {
            isSelected = change;

            BackgroundColor.Background = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900)) :
                                                      new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));

            GroupLabel.Foreground = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50)) :
                                                 new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));
        }

        private void BackgroundColor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundColor.Background = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary700)) :
                                                      new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary200));
        }

        private void BackgroundColor_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BackgroundColor.Background = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800)) :
                                                      new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));

            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ChangeActiveGroupItem(ID);
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).SelectInputFile();
        }

        private void BackgroundColor_MouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundColor.Background = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800)) :
                                                      new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));

            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void BackgroundColor_MouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundColor.Background = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900)) :
                                                      new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));

            Mouse.OverrideCursor = null;
        }
    }
}
