using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Menus.Driverless;
using Telemetry_presentation_layer.Menus.Settings.InputFiles;

namespace Telemetry_presentation_layer.Menus.Settings.Groups
{
    /// <summary>
    /// Represents one <see cref="Group"/> <see cref="Attribute"/> int <see cref="GroupSettings"/>.
    /// </summary>
    public partial class GroupSettingsAttribute : UserControl
    {
        /// <summary>
        /// <see cref="Attribute"/>s name.
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// <see cref="Group"/>s name.
        /// </summary>
        private readonly string groupName;

        /// <summary>
        /// Hex code of the channels color.
        /// </summary>
        private readonly string colorCode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="channelName"><see cref="Channel"/>s name.</param>
        /// <param name="groupName"><see cref="Group"/>s name.</param>
        /// <param name="color"><see cref="Attribute"/>s color.</param>
        public GroupSettingsAttribute(string channelName, string groupName, string color)
        {
            InitializeComponent();

            AttributeName = channelName;
            this.groupName = groupName;
            colorCode = color;
            AttributeLbl.Content = channelName;
            ChangeColorBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        /// <summary>
        /// Changes the color of the selected <see cref="Attribute"/> in all <see cref="Group"/>s and <see cref="InputFile"/>s.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeColorBtn_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickColor = new PickColor(colorCode);
            if (pickColor.ShowDialog() == true)
            {
                var pickedColor = pickColor.ColorPicker.Color;
                GroupManager.GetGroup(groupName).GetAttribute(AttributeName).Color = pickedColor.ToString();

                foreach (var inputFile in InputFileManager.InputFiles)
                {
                    var channel = inputFile.GetChannel(AttributeName);
                    if (channel != null)
                    {
                        channel.Color = pickedColor.ToString();
                    }
                }

                foreach (var group in GroupManager.Groups)
                {
                    var channel = group.GetAttribute(AttributeName);
                    if (channel != null)
                    {
                        channel.Color = pickedColor.ToString();
                    }
                }

                GroupManager.SaveGroups();
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitAttributes();
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateCharts();
                ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).InitInputFileSettingsItems();
            }
        }

        private void DeleteAttributeBtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DeleteAttributeBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary500));
        }

        private void DeleteAttributeBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DeleteAttributeBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
        }

        private void DeleteAttributeBtn_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DeleteAttributeBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary300));
        }

        private void DeleteAttributeBtn_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DeleteAttributeBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary500));

            GroupManager.GetGroup(groupName).RemoveAttribute(AttributeName);
            if (GroupManager.Groups.Count > 0)
            {
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveAttribute = GroupManager.Groups.First().Attributes.First();
            }
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitActiveChannelSelectableAttributes();
            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateCharts();
            GroupManager.SaveGroups();
        }
    }
}
