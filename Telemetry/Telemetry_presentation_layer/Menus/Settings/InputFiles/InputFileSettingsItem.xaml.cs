using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Menus.Driverless;

namespace Telemetry_presentation_layer.Menus.Settings.InputFiles
{
    /// <summary>
    /// Interaction logic for InputFileSettingsItem.xaml
    /// </summary>
    public partial class InputFileSettingsItem : UserControl
    {
        public int ID { get; set; }

        private bool driverless;

        public InputFileSettingsItem(InputFile inputFile)
        {
            InitializeComponent();

            ID = inputFile.ID;
            driverless = inputFile.Driverless;

            InputFileNameLbl.Content = inputFile.Name;

            ChangeTypeImage();
        }

        private void ChangeTypeImage()
        {
            var logo = new BitmapImage();
            logo.BeginInit();

            if (driverless)
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageRelativePath = "Images/daisy.png";
                string imagePath = Path.Combine(baseDirectory, imageRelativePath);

                logo.UriSource = new Uri(imagePath);
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageRelativePath = "Images/art_banner.png";
                string imagePath = Path.Combine(baseDirectory, imageRelativePath);

                logo.UriSource = new Uri(imagePath);
            }

            logo.EndInit();

            InputFileTypeImage.Source = logo;
        }

        public void ChangeColorMode(bool selected)
        {
            var converter = new BrushConverter();
            BackgroundCard.Background =    selected ? (Brush)converter.ConvertFromString("#3c3c3c") : Brushes.White;
            InputFileNameLbl.Foreground = !selected ? (Brush)converter.ConvertFromString("#3c3c3c") : Brushes.White;
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeActiveInputFileSettingsItem(ID);
            ChangeColorMode(selected: true);
        }

        private void ChangeGroupItemType_Click(object sender, RoutedEventArgs e)
        {
            var inputFile = InputFileManager.GetInputFile(ID);
            if (inputFile != null)
            {
                if (inputFile is DriverlessInputFile)
                {
                    InputFileManager.RemoveInputFile(ID);
                    InputFileManager.AddInputFile(new StandardInputFile(inputFile));
                    driverless = false;
                }
                else
                {
                    InputFileManager.RemoveInputFile(ID);
                    InputFileManager.AddInputFile(new DriverlessInputFile(inputFile));
                    driverless = true;
                }
            }

            ChangeTypeImage();
            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateAfterReadFile();
            ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).UpdateRequiredChannels();
        }

        private void ChangeGroupItemType_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeGroupItemType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary300));
        }

        private void ChangeGroupItemType_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeGroupItemType.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
        }
    }
}
