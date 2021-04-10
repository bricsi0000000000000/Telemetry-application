using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataLayer.Units;
using LocigLayer.Colors;
using LocigLayer.Texts;
using LocigLayer.Units;
using PresentationLayer.Converters;
using PresentationLayer.ValidationRules;

namespace PresentationLayer.Menus.Settings.Units
{
    /// <summary>
    /// Interaction logic for AddUnitOfMeasureWindow.xaml
    /// </summary>
    public partial class AddUnitOfMeasureWindow : Window
    {
        private GridLength expanderWidth = GridLength.Auto;

        public AddUnitOfMeasureWindow()
        {
            InitializeComponent();
            DataContext = new FieldsViewModel();
        }

        private void Grid_Collapsed(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                expanderWidth = grid.ColumnDefinitions[1].Width;
                grid.ColumnDefinitions[1].Width = GridLength.Auto;
            }
        }

        private void Grid_Expanded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                grid.ColumnDefinitions[1].Width = expanderWidth;
            }
        }

        private void OkCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void OkCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            if (!NameTextBox.Text.Equals(string.Empty) && !FormulaTextBox.Text.Equals(string.Empty))
            {
                var unit = new Unit(UnitOfMeasureManager.UnitOfMeasures.Last().ID + 1, NameTextBox.Text, DescriptionTextBox.Text, FormulaTextBox.Text);

                ((UnitsMenu)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).AddUnit(unit, add: true);

                Close();
            }
        }

        private void OkCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            OkCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void OkCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            OkCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void CancelCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CancelCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void CancelCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CancelCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            ((UnitsMenu)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).AddUnit(null, add: false);

            Close();
        }

        private void CancelCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CancelCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void CancelCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CancelCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }
    }
}
