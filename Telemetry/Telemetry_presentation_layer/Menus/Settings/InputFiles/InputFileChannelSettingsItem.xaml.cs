using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Menus.Driverless;
using Telemetry_presentation_layer.Menus.Settings.Groups;

namespace Telemetry_presentation_layer.Menus.Settings.InputFiles
{
    /// <summary>
    /// Interaction logic for InputFileChannelSettingsItem.xaml
    /// </summary>
    public partial class InputFileChannelSettingsItem : UserControl
    {
        public string ChannelName { get; set; }

        private readonly string inputFileName;

        private readonly string colorCode;

        public InputFileChannelSettingsItem(string channelName, string inputFileName, string color)
        {
            InitializeComponent();

            ChannelName = channelName;
            this.inputFileName = inputFileName;
            colorCode = color;
            AttributeLbl.Content = channelName;
            ChangeColor((Color)ColorConverter.ConvertFromString(color));
            // InitImportantChannelsComboBox();
        }

        private void ChangeColor(Color color)
        {
            ChangeColorBtn.Background = new SolidColorBrush(color);
        }

        private void ChangeColorBtn_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickColor = new PickColor(colorCode);
            if (pickColor.ShowDialog() == true)
            {
                var pickedColor = pickColor.ColorPicker.Color;
                ChangeColor(pickedColor);
                /* var driverlessInputFile = DriverlessInputFileManager.Instance.GetInputFile(inputFileName);
                 if (driverlessInputFile != null)
                 {
                     driverlessInputFile.GetChannel(ChannelName).Color = pickedColor;
                 }
                 else
                 {
                     var standardInputFile = StandardInputFileManager.Instance.GetInputFile(inputFileName);
                     if (standardInputFile != null)
                     {
                         standardInputFile.GetChannel(ChannelName).Color = pickedColor;
                     }
                 }*/

                //((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).InitInputFileSettingsItems();

                foreach (var group in GroupManager.Groups)
                {
                    var channel = group.GetAttribute(ChannelName);
                    if (channel != null)
                    {
                        channel.Color = pickedColor.ToString();
                    }
                }

                foreach (var inputFile in InputFileManager.InputFiles)
                {
                    foreach (var channel in inputFile.Channels)
                    {
                        if (channel.Name.Equals(ChannelName))
                        {
                            channel.Color = pickedColor.ToString();
                        }
                    }
                }

                GroupManager.SaveGroups();
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitAttributes();

                //TODO if driverless, a driverlesseset updatelje ha nem akkor meg a másikat
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateCharts();
            }
        }

        private void InitImportantChannelsComboBox()
        {
            ImportantChannelsComboBox.Items.Clear();

            var comboBoxItem = new ComboBoxItem() { Content = "", IsSelected = true };
            comboBoxItem.PreviewMouseLeftButtonDown += ChooseInputFileComboBox_PreviewMouseRightButtonUp;
            ImportantChannelsComboBox.Items.Add(comboBoxItem);

            var inputFile = InputFileManager.GetInputFile(inputFileName);

            foreach (var item in ImportantChannels.DriverlessImportantChannelNames)
            {
                bool found = item.Equals(ChannelName);

                inputFile.ChangeRequiredChannelSatisfaction(item, found);

                comboBoxItem = new ComboBoxItem() { Content = item, IsSelected = found };
                comboBoxItem.PreviewMouseLeftButtonDown += ChooseInputFileComboBox_PreviewMouseRightButtonUp;
                ImportantChannelsComboBox.Items.Add(comboBoxItem);
            }
        }

        private void ChooseInputFileComboBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
