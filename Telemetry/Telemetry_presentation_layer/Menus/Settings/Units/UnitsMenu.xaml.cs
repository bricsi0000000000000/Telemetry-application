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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Units;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.Menus.Live;
using Telemetry_presentation_layer.ValidationRules;

namespace Telemetry_presentation_layer.Menus.Settings.Units
{
    /// <summary>
    /// Interaction logic for UnitsMenu.xaml
    /// </summary>
    public partial class UnitsMenu : UserControl
    {
        private Unit activeUnit;
        private bool isUnitSelected = false;
        private GridLength expanderHeight = GridLength.Auto;

        public UnitsMenu()
        {
            InitializeComponent();

            DataContext = new FieldsViewModel();

            if (UnitOfMeasureManager.UnitOfMeasures.Count > 0)
            {
                activeUnit = UnitOfMeasureManager.UnitOfMeasures.First();
                InitializeUnits();
            }

            UpdateSelectedSectionButtons();
        }

        private void UpdateSelectedSectionButtons()
        {
            DeleteSectionCardButton.Background = isUnitSelected ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900) :
                                                                  ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary200);

            NoUnitSelectedGrid.Visibility = isUnitSelected ? Visibility.Hidden : Visibility.Visible;
        }

        private void InitializeUnits()
        {
            UnitsStackPanel.Children.Clear();

            foreach (var unit in UnitOfMeasureManager.UnitOfMeasures)
            {
                UnitsStackPanel.Children.Add(new UnitItem(unit));
            }
        }

        public void AddUnit(Unit unit, bool add)
        {
            if (add)
            {
                UnitOfMeasureManager.AddUnitOfMeasure(unit);
                UnitOfMeasureManager.Save();
                activeUnit = UnitOfMeasureManager.UnitOfMeasures.Last();
                InitializeUnits();
            }

            SetLoadingGrid(visibility: false);
        }

        public void DeleteUnit(bool delete)
        {
            if (delete)
            {
                UnitOfMeasureManager.RemoveUnitOfMeasure(activeUnit.ID);
                UnitOfMeasureManager.Save();
                activeUnit = UnitOfMeasureManager.UnitOfMeasures.Last();
                InitializeUnits();
            }

            SetLoadingGrid(visibility: false);
        }

        public void SelectUnit(Unit unit)
        {
            if (activeUnit.ID != unit.ID || !isUnitSelected)
            {
                activeUnit = unit;

                isUnitSelected = true;

                UpdateSelectedUnitUI();

                foreach (var item in UnitsStackPanel.Children)
                {
                    ((UnitItem)item).ChangeColor(((UnitItem)item).Unit.ID == unit.ID);
                }
            }

            UpdateSelectedSectionButtons();
        }

        private void UpdateSelectedUnitUI()
        {
            SelectedUnitNameTextBox.Text = activeUnit.Name;
            SelectedUnitFormulaTextBox.Text = activeUnit.UnitOfMeasure;
            UnitOfMeasureFormulaControl.Formula = activeUnit.UnitOfMeasure;
        }

        public void ChangeUnitName(string newName)
        {
            if (newName.Equals(string.Empty))
            {
                ShowErrorMessage("Name can't be empty");
            }
            else
            {
                if (!activeUnit.Name.Equals(newName))
                {
                    UnitOfMeasureManager.ChangeUnitOfMeasureName(activeUnit.ID, newName);
                    UnitOfMeasureManager.Save();
                    InitializeUnits();
                    ShowErrorMessage("Saved", error: false);
                }
            }
        }

        public void ChangeUnitFormula(string newFormula)
        {
            if (newFormula.Equals(string.Empty))
            {
                ShowErrorMessage("Unit of measure can't be empty");
            }
            else
            {
                if (!activeUnit.UnitOfMeasure.Equals(newFormula))
                {
                    UnitOfMeasureManager.ChangeUnitOfMeasureFormula(activeUnit.ID, newFormula);
                    UnitOfMeasureManager.Save();
                    InitializeUnits();
                    ShowErrorMessage("Saved", error: false);
                }
            }
        }

        private void SetLoadingGrid(bool visibility)
        {
            LoadingGrid.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error">If true, it's an error message, if not, it's a regular one.</param>
        /// <param name="time"></param>
        private void ShowErrorMessage(string message, bool error = true, double time = 3)
        {
            ErrorSnackbar.Foreground = error ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary400)) :
                                               new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));

            ErrorSnackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }





        private void DeleteSectionCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isUnitSelected)
            {
                DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary700);
            }
        }

        private void DeleteSectionCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isUnitSelected)
            {
                DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);

                SetLoadingGrid(visibility: true);
                var deleteSectionWindow = new PopUpWindow($"You are about to delete '{activeUnit.Name}'\n" +
                                                          $"Are you sure about that?",
                                                          PopUpWindow.PopUpType.DeleteUnit);
                deleteSectionWindow.ShowDialog();
            }
        }

        private void DeleteSectionCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isUnitSelected)
            {
                DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void DeleteSectionCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isUnitSelected)
            {
                DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
                Mouse.OverrideCursor = null;
            }
        }

        private void ChangeNameCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void ChangeNameCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            ChangeUnitName(SelectedUnitNameTextBox.Text);
        }

        private void ChangeNameCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeNameCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void ChangeSectionFormulaCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeUnitFormulaCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void ChangeSectionFormulaCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeUnitFormulaCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            ChangeUnitFormula(SelectedUnitFormulaTextBox.Text);
        }

        private void ChangeSectionFormulaCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeUnitFormulaCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSectionFormulaCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeUnitFormulaCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void AddUnitButton_Click(object sender, RoutedEventArgs e)
        {
            SetLoadingGrid(visibility: true);
            var addUnitOfMeasureWindow = new AddUnitOfMeasureWindow();
            addUnitOfMeasureWindow.ShowDialog();
        }

        private void Grid_Collapsed(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                expanderHeight = grid.RowDefinitions[2].Height;
                grid.RowDefinitions[2].Height = GridLength.Auto;
            }
        }

        private void Grid_Expanded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                grid.RowDefinitions[2].Height = expanderHeight;
            }
        }
    }
}
