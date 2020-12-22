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
            ChangeColorBtn.Background = new SolidColorBrush(color);
            InitImportantChannelsComboBox();
        }

        private void ChangeColorBtn_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickColor = new PickColor();
            if (pickColor.ShowDialog() == true)
            {
                var pickedColor = pickColor.ColorPicker.Color;

                //TODO stadnarddal is
                DriverlessInputFileManager.Instance.GetInputFile(inputFileName).GetChannel(ChannelName).Color = pickedColor;
                ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).InitInputFileSettingsItems();

                foreach (var group in GroupManager.Groups)
                {
                    var channel = group.GetAttribute(ChannelName);
                    if (channel != null)
                    {
                        channel.Color = pickedColor;
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

            foreach (var item in ImportantChannels.DriverlessImportantChannels)
            {
                comboBoxItem = new ComboBoxItem() { Content = item, IsSelected = item.Equals(ChannelName) };
                comboBoxItem.PreviewMouseLeftButtonDown += ChooseInputFileCombobox_PreviewMouseRightButtonUp;
                ImportantChannelsComboBox.Items.Add(comboBoxItem);
            }
        }

        private void ChooseInputFileCombobox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
