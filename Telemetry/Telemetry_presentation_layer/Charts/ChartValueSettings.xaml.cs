using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.Menus;
using Telemetry_presentation_layer.Menus.Driverless;
using Telemetry_presentation_layer.Menus.Settings;
using Telemetry_presentation_layer.Menus.Settings.Groups;
using Telemetry_presentation_layer.Menus.Settings.InputFiles;

namespace Telemetry_presentation_layer.Charts
{
    /// <summary>
    /// Interaction logic for ChartValueSettings.xaml
    /// </summary>
    public partial class ChartValueSettings : UserControl
    {
        private int inputFileID;
        private string colorCode;
        private string groupName;
        private bool isActive = false;

        public ChartValueSettings(string channelName, string groupName, int lineWidth = 0, string color = "#ffffff", int inputFileID = -1, bool isActive = false)
        {
            InitializeComponent();

            this.inputFileID = inputFileID;
            this.groupName = groupName;
            colorCode = color;
            ChannelName = channelName;
            ColorCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(color);
            LineWidthLabel.Content = $"{lineWidth} pt";
            this.isActive = isActive;
            IsVisibleCheckBox.IsChecked = isActive;

            ChannelNameLabel.Opacity = isActive ? 1 : .5f;
            LineWidthLabel.Opacity = isActive ? 1 : .5f;
        }

        private string channelName;
        public string ChannelName
        {
            get
            {
                return channelName;
            }
            set
            {
                channelName = value;
                SetChannelName(channelName);
            }
        }

        /// <summary>
        /// Sets the <see cref="ChannelNameLabel"/>s text to <paramref name="channelName"/>.
        /// </summary>
        /// <param name="channelName"><see cref="Channel"/>s name.</param>
        private void SetChannelName(string channelName)
        {
            if (inputFileID != -1)
            {
                ChannelNameLabel.Content = $"{channelName}_{inputFileID}";
            }
            else
            {
                ChannelNameLabel.Content = channelName;
            }
        }

        public void SetUp(string color, int inputFileID)
        {
            /*ChannelNameLabel.Opacity = 1;
            ChannelValueLabel.Opacity = 1;
            UnitOfMeasureFormulaControl.Opacity = 1;

            this.inputFileID = inputFileID;
            colorCode = color;
            ColorCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(color);

            SetChannelName(channelName);*/
        }

        private void ColorCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (inputFileID != -1)
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void ColorCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (inputFileID != -1)
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ColorCard_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (inputFileID != -1)
            {
                PickColor pickColor = new PickColor(colorCode);
                if (pickColor.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var pickedColor = pickColor.ColorPicker.Color;
                    ChangeColor(pickedColor);

                    var group = GroupManager.GetGroup(groupName);

                    if (group != null)
                    {
                        foreach (var actGroup in GroupManager.Groups)
                        {
                            if (actGroup.Name.Equals(groupName))
                            {
                                var channel = actGroup.GetAttribute(ChannelName);
                                if (channel != null)
                                {
                                    channel.Color = pickedColor.ToString();
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var inputFile in InputFileManager.InputFiles)
                        {
                            if (inputFile.ID == inputFileID)
                            {
                                foreach (var channel in inputFile.Channels)
                                {
                                    if (channel.Name.Equals(ChannelName))
                                    {
                                        channel.Color = pickedColor.ToString();
                                    }
                                }
                            }
                        }
                    }

                    GroupManager.SaveGroups();
                    ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitAttributes();
                    ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).InitChannelItems();

                    //TODO if driverless, a driverlesseset updatelje ha nem akkor meg a másikat
                    ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).BuildCharts();

                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void ChangeColor(Color color)
        {
            ColorCard.Background = new SolidColorBrush(color);
        }

        private void LineWidthLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (inputFileID != -1)
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void LineWidthLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (inputFileID != -1)
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void IsVisibleCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            bool newIsActive = (bool)IsVisibleCheckBox.IsChecked;
            if (newIsActive != isActive)
            {
                isActive = newIsActive;
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).SetChannelActivity(channelName, inputFileID, isActive);
            }
        }
    }
}
