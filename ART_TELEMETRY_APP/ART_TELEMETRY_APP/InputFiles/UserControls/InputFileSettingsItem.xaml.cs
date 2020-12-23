using ART_TELEMETRY_APP.Driverless.UserControls;
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

        private bool driverless;

        public InputFileSettingsItem(string inputFileName, bool driverless = false)
        {
            InitializeComponent();

            InputFileName = inputFileName;
            this.driverless = driverless;

            InputFileNameLbl.Content = inputFileName;

            ChangeTypeImage();
        }

        private void ChangeTypeImage()
        {
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
            }
            else
            {
                StandardInputFileManager.Instance.RemoveInputFile(InputFileName);
            }

            ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).RemoveSingleInputFileSettingsItem(InputFileName);
        }

        public void ChangeColorMode(bool selected)
        {
            var converter = new BrushConverter();
            ColorZone.BorderBrush = selected ? Brushes.White : (Brush)converter.ConvertFromString("#FF303030");
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeActiveInputFileSettingsItem(InputFileName);
            ChangeColorMode(selected: true);
        }

        private void ChangeGroupItemType_Click(object sender, RoutedEventArgs e)
        {
            var driverlessInputFile = DriverlessInputFileManager.Instance.GetInputFile(InputFileName);
            if (driverlessInputFile != null)
            {
                DriverlessInputFileManager.Instance.RemoveInputFile(InputFileName);
                StandardInputFileManager.Instance.AddInputFile(new StandardInputFile(driverlessInputFile)
                {
                    Driverless = false
                });
                driverless = false;
            }
            else
            {
                var stadnardInputFile = StandardInputFileManager.Instance.GetInputFile(InputFileName);
                StandardInputFileManager.Instance.RemoveInputFile(InputFileName);
                DriverlessInputFileManager.Instance.AddInputFile(new DriverlessInputFile(stadnardInputFile)
                {
                    Driverless = true
                });
                driverless = true;
            }

            ChangeTypeImage();
            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateAfterReadFile();
            ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).UpdateReuqiredChannels();
        }
    }
}
