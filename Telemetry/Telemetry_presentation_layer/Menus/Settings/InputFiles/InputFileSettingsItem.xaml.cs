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

        private void DeleteInputFile_Click(object sender, RoutedEventArgs e)
        {
            InputFileManager.RemoveInputFile(InputFileName);

            ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).RemoveSingleInputFileSettingsItem(InputFileName);
        }

        public void ChangeColorMode(bool selected)
        {
            var converter = new BrushConverter();
            BackgroundCard.Background =    selected ? (Brush)converter.ConvertFromString("#3c3c3c") : Brushes.White;
            InputFileNameLbl.Foreground = !selected ? (Brush)converter.ConvertFromString("#3c3c3c") : Brushes.White;
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeActiveInputFileSettingsItem(InputFileName);
            ChangeColorMode(selected: true);
        }

        private void ChangeGroupItemType_Click(object sender, RoutedEventArgs e)
        {
            var inputFile = InputFileManager.GetInputFile(InputFileName);
            if (inputFile != null)
            {
                if (inputFile is DriverlessInputFile)
                {
                    InputFileManager.RemoveInputFile(InputFileName);
                    InputFileManager.AddInputFile(new StandardInputFile(inputFile)
                    {
                        Driverless = false
                    });
                    driverless = false;
                }
                else
                {
                    InputFileManager.RemoveInputFile(InputFileName);
                    InputFileManager.AddInputFile(new StandardInputFile(inputFile)
                    {
                        Driverless = true
                    });
                    driverless = true;
                    //TODO Biztos jo ez igy? mert mindketto helyen standardet hoz letre
                }
            }

            ChangeTypeImage();
            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateAfterReadFile();
            ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).UpdateRequiredChannels();
        }

        private void DeleteGroupBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteGroupBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary400));
        }

        private void DeleteGroupBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteGroupBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
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
