using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataLayer.Groups;
using LogicLayer.Colors;
using PresentationLayer.Groups;
using PresentationLayer.InputFiles;
using PresentationLayer.Texts;
using PresentationLayer.Units;
using PresentationLayer.Menus.Driverless;
using LogicLayer.Menus.Settings.Groups;
using LogicLayer.Extensions;

namespace LogicLayer.Menus.Settings.InputFiles
{
    /// <summary>
    /// Interaction logic for InputFileChannelSettingsItem.xaml
    /// </summary>
    public partial class InputFileChannelSettingsItem : UserControl
    {
        public int ID { get; set; }

        public int ChannelID;

        private string colorCode;

        private int lineWidth;
        public int LineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
                LineWidthLabel.Content = $"{lineWidth} pt";
            }
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
                AttributeLabel.Content = channelName;
            }
        }

        private bool isSelected = false;

        private string unitOfMeasureFormula;

        public InputFileChannelSettingsItem(Channel channel, int inputFileID)
        {
            InitializeComponent();

            channelName = channel.Name;
            ChannelID = channel.ID;
            ID = inputFileID;
            colorCode = channel.Color;
            ChangeColor((Color)ColorConverter.ConvertFromString(colorCode));
            ChannelName = channel.Name;

            var unitOfMeasure = UnitOfMeasureManager.GetUnitOfMeasure(channel.Name);
            if (unitOfMeasure != null)
            {
                unitOfMeasureFormula = unitOfMeasure.UnitOfMeasure;
                var formula = @"\color[HTML]{" + ColorManager.Secondary900.ToString()[1..] + "}{" + unitOfMeasure.UnitOfMeasure + "}";
                UnitOfMeasureFormulaControl.Formula = formula;
            }

            LineWidth = channel.LineWidth;
        }

        public void ChangeColor(Color color)
        {
            ChangeColorBtn.Background = new SolidColorBrush(color);
        }

        private void ChangeColorBtn_Click(object sender, RoutedEventArgs e)
        {
            PickColor pickColor = new PickColor(colorCode);
            if (pickColor.ShowDialog() == true)
            {
                var pickedColor = pickColor.ColorPicker.Color;
                colorCode = pickedColor.ToString();
                ChangeColor(pickedColor);

                /*foreach (var group in GroupManager.Groups)
                {
                    var channel = group.GetAttribute(channelName);
                    if (channel != null)
                    {
                        channel.Color = pickedColor.ToString();
                    }
                }*/

                foreach (var inputFile in InputFileManager.InputFiles)
                {
                    foreach (var channel in inputFile.Channels)
                    {
                        if (channel.Name.Equals(channelName))
                        {
                            channel.Color = pickedColor.ToString();
                        }
                    }
                }

                GroupManager.SaveGroups();
                ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitAttributes();
                ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).SelectInputFile();

                //TODO if driverless, a driverlesseset updatelje ha nem akkor meg a másikat
                ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).BuildCharts();
                MenuManager.LiveTelemetry.BuildCharts();
            }
        }

        private void ChangeColorBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeColorBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        public void ChangeColorMode(bool selected)
        {
            isSelected = selected;

            BackgroundCard.Background = isSelected ? ColorManager.Secondary900.ConvertBrush() :
                                                     ColorManager.Secondary50.ConvertBrush();

            AttributeLabel.Foreground = isSelected ? ColorManager.Secondary50.ConvertBrush() :
                                                         ColorManager.Secondary900.ConvertBrush();

            LineWidthLabel.Foreground = isSelected ? ColorManager.Secondary50.ConvertBrush() :
                                                         ColorManager.Secondary900.ConvertBrush();

            var formula = @"\color[HTML]{" + (isSelected ? ColorManager.Secondary50.ToString()[1..] : ColorManager.Secondary900.ToString()[1..]) + "}{" + unitOfMeasureFormula + "}";
            UnitOfMeasureFormulaControl.Formula = formula;
        }

        private void BackgroundCard_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isSelected ? ColorManager.Secondary700.ConvertBrush() :
                                                     ColorManager.Secondary200.ConvertBrush();
        }

        private void BackgroundCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            BackgroundCard.Background = isSelected ? ColorManager.Secondary800.ConvertBrush() :
                                                     ColorManager.Secondary100.ConvertBrush();

            ((InputFilesSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeActiveChannelSettingsItem(ChannelID);

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
