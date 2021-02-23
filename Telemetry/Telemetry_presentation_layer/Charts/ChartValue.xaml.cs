using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
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
    /// Represents the a single value next to a <see cref="Chart"/>.
    /// </summary>
    public partial class ChartValue : UserControl
    {
        private string colorCode;
        private int inputFileID;
        private string groupName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="color"><see cref="Color"/> of the channel.</param>
        /// <param name="channelName">Channel name.</param>
        /// <param name="value">Channel value.</param>
        public ChartValue(string color, string channelName, string unitOfMeasure, int inputFileID, string groupName)
        {
            InitializeComponent();

            ChannelNameLabel.Opacity = .4f;
            ChannelValueLabel.Opacity = .4f;
            UnitOfMeasureFormulaControl.Opacity = .4f;

            colorCode = color;
            this.inputFileID = inputFileID;
            this.groupName = groupName;

            ColorCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(color);
            ChannelName = channelName;
            SetChannelValue(0);
            var formula = @"\color[HTML]{" + ColorManager.Secondary900[1..] + "}{" + unitOfMeasure + "}";
            UnitOfMeasureFormulaControl.Formula = formula;
        }

        /// <summary>
        /// <see cref="Channel"/>s name whose data is represented.
        /// </summary>
        private string channelName;

        /// <summary>
        /// <see cref="Channel"/>s name whose data is represented.
        /// </summary>
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

        /// <summary>
        /// Sets the <see cref="ChannelValueLabel"/>s text to <paramref name="channelValue"/>.
        /// </summary>
        /// <param name="channelValue"><see cref="Channel"/>s value.</param>
        public void SetChannelValue(double channelValue)
        {
            if (inputFileID != -1)
            {
                ChannelValueLabel.Content = $"{channelValue:f3}";
            }
        }

        public void Set(string color, int inputFileID)
        {
            ChannelNameLabel.Opacity = 1;
            ChannelValueLabel.Opacity = 1;
            UnitOfMeasureFormulaControl.Opacity = 1;

            colorCode = color;
            this.inputFileID = inputFileID;

            ColorCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(color);
            ChannelName = channelName;
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
                    ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateCharts();

                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void ChangeColor(Color color)
        {
            ColorCard.Background = new SolidColorBrush(color);
        }
    }
}
