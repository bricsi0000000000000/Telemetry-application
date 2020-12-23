using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Driverless.UserControls;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Groups.UserControls;
using ART_TELEMETRY_APP.Groups.Windows;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Settings.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.InputFiles.UserControls
{
    /// <summary>
    /// Interaction logic for InputFileChannelSettingsItem.xaml
    /// </summary>
    public partial class InputFileChannelSettingsItem : UserControl
    {
        public string ChannelName { get; set; }

        private readonly string inputFileName;

        public InputFileChannelSettingsItem(string channelName, string inputFileName, Color color)
        {
            InitializeComponent();

            ChannelName = channelName;
            this.inputFileName = inputFileName;
            AttributeLbl.Content = channelName;
            ChangeColor(color);
           // InitImportantChannelsComboBox();
        }

        private void ChangeColor(Color color)
        {
            ChangeColorBtn.Background = new SolidColorBrush(color);
        }

        private void ChangeColorBtn_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickColor = new PickColor();
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
                        channel.Color = pickedColor;
                    }
                }

                foreach (var inputFile in DriverlessInputFileManager.Instance.InputFiles)
                {
                    foreach (var channel in inputFile.Channels)
                    {
                        if (channel.Name.Equals(ChannelName))
                        {
                            channel.Color = pickedColor;
                        }
                    }
                }

                foreach (var inputFile in StandardInputFileManager.Instance.InputFiles)
                {
                    foreach (var channel in inputFile.Channels)
                    {
                        if (channel.Name.Equals(ChannelName))
                        {
                            channel.Color = pickedColor;
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
            comboBoxItem.PreviewMouseLeftButtonDown += ChooseInputFileCombobox_PreviewMouseRightButtonUp;
            ImportantChannelsComboBox.Items.Add(comboBoxItem);

            var inputFile = new InputFile();
            inputFile = DriverlessInputFileManager.Instance.GetInputFile(inputFileName);
            if (inputFile == null)
            {
                inputFile = StandardInputFileManager.Instance.GetInputFile(inputFileName);
            }

            foreach (var item in ImportantChannels.DriverlessImportantChannelNames)
            {
                bool found = item.Equals(ChannelName);

                inputFile.ChangeRequiredChannelSatisfaction(item, found);

                comboBoxItem = new ComboBoxItem() { Content = item, IsSelected = found };
                comboBoxItem.PreviewMouseLeftButtonDown += ChooseInputFileCombobox_PreviewMouseRightButtonUp;
                ImportantChannelsComboBox.Items.Add(comboBoxItem);
            }
        }

        private void ChooseInputFileCombobox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
