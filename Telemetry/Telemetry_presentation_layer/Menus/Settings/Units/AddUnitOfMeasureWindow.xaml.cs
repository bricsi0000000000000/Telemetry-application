using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_data_and_logic_layer.Units;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.ValidationRules;

namespace Telemetry_presentation_layer.Menus.Settings.Units
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
