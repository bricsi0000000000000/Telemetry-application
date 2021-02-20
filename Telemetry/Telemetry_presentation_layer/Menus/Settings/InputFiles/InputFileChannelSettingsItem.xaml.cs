using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_data_and_logic_layer.Units;
using Telemetry_presentation_layer.Menus.Driverless;
using Telemetry_presentation_layer.Menus.Settings.Groups;

namespace Telemetry_presentation_layer.Menus.Settings.InputFiles
{
    /// <summary>
    /// Interaction logic for InputFileChannelSettingsItem.xaml
    /// </summary>
    public partial class InputFileChannelSettingsItem : UserControl
    {
        public string ActiveInputFileName { get; set; }


        private readonly string colorCode;

        public InputFileChannelSettingsItem(Channel channel, string activeInputFileName)
        {
            InitializeComponent();

            ActiveInputFileName = activeInputFileName;
            colorCode = channel.Color;
            ChangeColor((Color)ColorConverter.ConvertFromString(colorCode));
            AttributeLbl.Content = activeInputFileName;

            var unitOfMeasure = UnitOfMeasureManager.GetUnitOfMeasure(channel.Name);
            if (unitOfMeasure != null)
            {
                var formula = @"\color[HTML]{" + ColorManager.Secondary900[1..] + "}{" + unitOfMeasure.UnitOfMeasure + "}";
                UnitOfMeasureFormulaControl.Formula = formula;
            }
            LineWidthLabel.Content = $"{channel.LineWidth} pt";
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
                ChangeColor(pickedColor);

                foreach (var group in GroupManager.Groups)
                {
                    var channel = group.GetAttribute(ActiveInputFileName);
                    if (channel != null)
                    {
                        channel.Color = pickedColor.ToString();
                    }
                }

                foreach (var inputFile in InputFileManager.InputFiles)
                {
                    foreach (var channel in inputFile.Channels)
                    {
                        if (channel.Name.Equals(ActiveInputFileName))
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

        private void ChangeColorBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeColorBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }
    }
}
