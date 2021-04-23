using System.Windows.Controls;
using System.Windows.Input;
using DataLayer.Units;
using LogicLayer.Colors;
using PresentationLayer.Texts;
using LogicLayer.Extensions;

namespace LogicLayer.Menus.Settings.Units
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

            BackgroundCard.Background = isActive ? ColorManager.Secondary900.ConvertBrush() :
                                                   ColorManager.Secondary50.ConvertBrush();

            UnitNameLabel.Foreground = isActive ? ColorManager.Secondary50.ConvertBrush() :
                                                  ColorManager.Secondary900.ConvertBrush();

            UnitOfMeasureFormulaControl.Formula = isActive ? @"\color[HTML]{ffffff}{" + Unit.UnitOfMeasure + "}" :
                                                             @"\color[HTML]{3c3c3c}{" + Unit.UnitOfMeasure + "}";
        }

        private void BackgroundCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isActive ? ColorManager.Secondary700.ConvertBrush() :
                                                   ColorManager.Secondary200.ConvertBrush();
        }

        private void BackgroundCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isActive ? ColorManager.Secondary800.ConvertBrush() :
                                                   ColorManager.Secondary100.ConvertBrush();

            ((UnitsMenu)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).SelectUnit(Unit);
        }

        private void BackgroundCard_MouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isActive ? ColorManager.Secondary800.ConvertBrush() :
                                                   ColorManager.Secondary100.ConvertBrush();
        }

        private void BackgroundCard_MouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isActive ? ColorManager.Secondary900.ConvertBrush() :
                                                   ColorManager.Secondary50.ConvertBrush();
        }
    }
}
