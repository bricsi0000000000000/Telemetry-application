using System.Windows.Controls;
using System.Windows.Input;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_data_and_logic_layer.Units;
using Telemetry_presentation_layer.Converters;

namespace Telemetry_presentation_layer.Menus.Settings.Units
{
    /// <summary>
    /// Interaction logic for UnitItem.xaml
    /// </summary>
    public partial class UnitItem : UserControl
    {
        public Unit Unit { get; private set; }

        private bool isActive = false;

        public UnitItem(Unit unit)
        {
            InitializeComponent();

            Unit = unit;

            UnitNameLabel.Content = unit.Name;
            UnitOfMeasureFormulaControl.Formula = unit.UnitOfMeasure;
        }

        public void ChangeColor(bool isActive)
        {
            this.isActive = isActive;

            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);

            UnitNameLabel.Foreground = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50) :
                                                  ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900);

            UnitOfMeasureFormulaControl.Formula = isActive ? @"\color[HTML]{ffffff}{" + Unit.UnitOfMeasure + "}" :
                                                             @"\color[HTML]{3c3c3c}{" + Unit.UnitOfMeasure + "}";
        }

        private void BackgroundCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary700) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void BackgroundCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary800) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            ((UnitsMenu)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).SelectUnit(Unit);
        }

        private void BackgroundCard_MouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary800) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
        }

        private void BackgroundCard_MouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
        }
    }
}
