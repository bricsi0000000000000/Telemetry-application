using ART_TELEMETRY_APP.Driverless.UserControls;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Groups.Windows;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ART_TELEMETRY_APP.Settings.UserControls;

namespace ART_TELEMETRY_APP.Groups.UserControls
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
        /// Constructor
        /// </summary>
        /// <param name="channelName"><see cref="Channel"/>s name.</param>
        /// <param name="groupName"><see cref="Group"/>s name.</param>
        /// <param name="color"><see cref="Attribute"/>s color.</param>
        public GroupSettingsAttribute(string channelName, string groupName, Color color)
        {
            InitializeComponent();

            AttributeName = channelName;
            this.groupName = groupName;
            AttributeLbl.Content = channelName;
            ChangeColorBtn.Background = new SolidColorBrush(color);
        }

        /// <summary>
        /// Deletes an <see cref="Attribute"/> based on <see cref="AttributeName"/> from <see cref="Group"/> thats name is <see cref="groupName"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteAttribute_Click(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// Changes the color of the selected <see cref="Attribute"/> in all <see cref="Group"/>s and <see cref="InputFile"/>s.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeColorBtn_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickColor = new PickColor();
            if (pickColor.ShowDialog() == true)
            {
                var pickedColor = pickColor.ColorPicker.Color;
                GroupManager.GetGroup(groupName).GetAttribute(AttributeName).Color = pickedColor;

                foreach (var inputFile in InputFileManager.InputFiles)
                {
                    var channel = inputFile.GetChannel(AttributeName);
                    if (channel != null)
                    {
                        channel.Color = pickedColor;
                    }
                }

                foreach (var group in GroupManager.Groups)
                {
                    var channel = group.GetAttribute(AttributeName);
                    if (channel != null)
                    {
                        channel.Color = pickedColor;
                    }
                }

                GroupManager.SaveGroups();
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitAttributes();
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateCharts();
                ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).InitInputFileSettingsItems();
            }
        }
    }
}
