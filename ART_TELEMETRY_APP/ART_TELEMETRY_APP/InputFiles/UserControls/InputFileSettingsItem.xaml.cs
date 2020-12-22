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
    /// Interaction logic for InputFileSettingsItem.xaml
    /// </summary>
    public partial class InputFileSettingsItem : UserControl
    {
        public string InputFileName { get; set; }

        private readonly bool driverless;

        public InputFileSettingsItem(string inputFileName, bool driverless = false)
        {
            InitializeComponent();

            InputFileName = inputFileName;
            this.driverless = driverless;

            InputFileNameLbl.Content = inputFileName;

            var logo = new BitmapImage();
            logo.BeginInit();

            if (driverless)
            {
                logo.UriSource = new Uri("pack://application:,,,/ART_TELEMETRY_APP;component/Images/daisy.png");
            }
            else
            {
                logo.UriSource = new Uri("pack://application:,,,/ART_TELEMETRY_APP;component/Images/art_banner.png");
            }

            logo.EndInit();

            InputFileTypeImage.Source = logo;
        }

        private void DeleteInputFile_Click(object sender, RoutedEventArgs e)
        {
            if (driverless)
            {
                DriverlessInputFileManager.Instance.RemoveInputFile(InputFileName);
                ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).InitInputFileSettingsItems();
            }
            else
            {

            }
        }

        public void ChangeColorMode(bool change)
        {
            var converter = new BrushConverter();
            ColorZone.BorderBrush = change ? Brushes.White : (Brush)converter.ConvertFromString("#FF303030");
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).GroupSettingsItemClicked(GroupName);
        }

        private void ChangeGroupItemType_Click(object sender, RoutedEventArgs e)
        {
           /* var group = GroupManager.GetGroup(GroupName);
            group.Driverless = !group.Driverless;
            GroupManager.SaveGroups();
            ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();

            if (group.Driverless)
            {
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitActiveChannelSelectableAttributes();
            }
            else
            {
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).DestroyAllActiveChannelSelectableAttributes();
            }

         ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).ChangeGroupSettingsItemTypeTitle();*/
        }
    }
}
