using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public int ID { get; private set; }
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

        private bool isSelected = false;

        private int lineWidth;

        public int LineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
                LineWidthLabel.Content = $"{lineWidth} pt";
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="channelName"><see cref="Channel"/>s name.</param>
        /// <param name="groupName"><see cref="Group"/>s name.</param>
        /// <param name="color"><see cref="Attribute"/>s color.</param>
        public GroupSettingsAttribute(string groupName, Attribute attribute)
        {
            InitializeComponent();

            ID = attribute.ID;
            AttributeName = attribute.Name;
            this.groupName = groupName;
            colorCode = attribute.Color;
            AttributeLabel.Content = AttributeName;
            LineWidth = attribute.LineWidth;
            ChangeColor(colorCode);
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


        /*  GroupManager.GetGroup(groupName).RemoveAttribute(AttributeName);
          if (GroupManager.Groups.Count > 0)
          {
              ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ActiveAttribute = GroupManager.Groups.First().Attributes.First();
          }
          ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();
          ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitActiveChannelSelectableAttributes();
          ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateCharts();
          GroupManager.SaveGroups();
        */

        public void ChangeColor(string colorCode)
        {
            ColorCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorCode));
        }

        public void ChangeColorMode(bool selected)
        {
            isSelected = selected;
            BackgroundColor.Background = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900)) :
                                                      new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));

            AttributeLabel.Foreground = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50)) :
                                                     new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));

            LineWidthLabel.Foreground = isSelected ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50)) :
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

            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ChangeActiveAttributeItem(ID);
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
