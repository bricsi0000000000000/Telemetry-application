using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DataLayer.InputFiles;
using LogicLayer.Colors;
using PresentationLayer.InputFiles;
using PresentationLayer.Texts;
using PresentationLayer.Menus.Driverless;
using LogicLayer.Extensions;

namespace LogicLayer.Menus.Settings.InputFiles
{
    /// <summary>
    /// Interaction logic for InputFileSettingsItem.xaml
    /// </summary>
    public partial class InputFileSettingsItem : UserControl
    {
        public int ID { get; set; }

        private bool isDriverless;

        private string inputFileName;
        public string InputFileName
        {
            get
            {
                return inputFileName;
            }
            set
            {
                inputFileName = value;
                InputFileNameLabel.Content = inputFileName;
            }
        }

        private bool isSelected = false;

        public InputFileSettingsItem(InputFile inputFile)
        {
            InitializeComponent();

            ID = inputFile.ID;
            isDriverless = inputFile.InputFileType == InputFileTypes.driverless;

            InputFileName = inputFile.Name;

            ChangeTypeImage();
        }

        private void ChangeTypeImage()
        {
            var logo = new BitmapImage();
            logo.BeginInit();

            if (isDriverless)
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageRelativePath = @"..\..\..\..\..\default_files\images\daisy.png";
                string imagePath = Path.Combine(baseDirectory, imageRelativePath);

                logo.UriSource = new Uri(imagePath);
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imageRelativePath = @"..\..\..\..\..\default_files\images\art_banner.png";
                string imagePath = Path.Combine(baseDirectory, imageRelativePath);

                logo.UriSource = new Uri(imagePath);
            }

            logo.EndInit();

            InputFileTypeImage.Source = logo;
        }

        public void ChangeColorMode(bool selected)
        {
            isSelected = selected;

            BackgroundCard.Background = isSelected ? ColorManager.Secondary900.ConvertBrush() :
                                                     ColorManager.Secondary50.ConvertBrush();

            InputFileNameLabel.Foreground = isSelected ? ColorManager.Secondary50.ConvertBrush() :
                                                         ColorManager.Secondary900.ConvertBrush();
        }

        private void ChangeGroupItemType_Click(object sender, RoutedEventArgs e)
        {
            var inputFile = InputFileManager.Get(ID);
            if (inputFile != null)
            {
                InputFileManager.Remove(ID);

                if (inputFile is DriverlessInputFile)
                {
                    InputFileManager.Add(new StandardInputFile(inputFile));
                    isDriverless = false;
                }
                else
                {
                    InputFileManager.Add(new DriverlessInputFile(inputFile));
                    isDriverless = true;
                }
            }

            ChangeTypeImage();
            ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).UpdateAfterFileTypeChanges();
        }

        private void ChangeGroupItemType_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeGroupItemType.Background = ColorManager.Secondary300.ConvertBrush();
        }

        private void ChangeGroupItemType_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeGroupItemType.Background = ColorManager.Secondary50.ConvertBrush();
        }

        private void BackgroundCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isSelected ? ColorManager.Secondary700.ConvertBrush() :
                                                     ColorManager.Secondary200.ConvertBrush();
        }

        private void BackgroundCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            BackgroundCard.Background = isSelected ? ColorManager.Secondary800.ConvertBrush() :
                                                     ColorManager.Secondary100.ConvertBrush();

            ((InputFilesSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeActiveInputFileSettingsItem(ID);

            Mouse.OverrideCursor = null;
        }

        private void BackgroundCard_MouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isSelected ? ColorManager.Secondary800.ConvertBrush() :
                                                     ColorManager.Secondary100.ConvertBrush();

            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void BackgroundCard_MouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isSelected ? ColorManager.Secondary900.ConvertBrush() :
                                                     ColorManager.Secondary50.ConvertBrush();

            Mouse.OverrideCursor = null;
        }
    }
}
