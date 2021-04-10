using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataLayer.Units;
using LocigLayer.Colors;
using LocigLayer.Units;
using PresentationLayer.Converters;
using PresentationLayer.Menus.Live;
using PresentationLayer.ValidationRules;

namespace PresentationLayer.Menus.Settings.Units
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
            DeleteUnitCardButton.Background = isUnitSelected ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900) :
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
                InitializeUnits();
                activeUnit = UnitOfMeasureManager.UnitOfMeasures.Last();
                SelectUnit(activeUnit);
            }

            SetLoadingGrid(visibility: false);
        }

        public void DeleteUnit(bool delete)
        {
            if (delete)
            {
                UnitOfMeasureManager.RemoveUnitOfMeasure(activeUnit.ID);
                UnitOfMeasureManager.Save();
                InitializeUnits();
                if (UnitOfMeasureManager.UnitOfMeasures.Count > 0)
                {
                    activeUnit = UnitOfMeasureManager.UnitOfMeasures.Last();
                    SelectUnit(activeUnit);
                }
            }

            SetLoadingGrid(visibility: false);
        }

        public void SelectUnit(Unit unit)
        {
            activeUnit = unit;

            isUnitSelected = true;

            UpdateSelectedUnitUI();

            foreach (UnitItem item in UnitsStackPanel.Children)
            {
                item.ChangeColor(item.Unit.ID == unit.ID);
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

        private void DeleteUnitCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isUnitSelected)
            {
                DeleteUnitCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary700);
            }
        }

        private void DeleteUnitCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isUnitSelected)
            {
                DeleteUnitCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);

                SetLoadingGrid(visibility: true);
                var deleteSectionWindow = new PopUpWindow($"You are about to delete '{activeUnit.Name}'\n" +
                                                          $"Are you sure about that?",
                                                          PopUpWindow.PopUpType.DeleteUnit);
                deleteSectionWindow.ShowDialog();
            }
        }

        private void DeleteUnitCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isUnitSelected)
            {
                DeleteUnitCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void DeleteUnitCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isUnitSelected)
            {
                DeleteUnitCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
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

        private void Grid_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                expanderHeight = grid.RowDefinitions[2].Height;
                grid.RowDefinitions[2].Height = GridLength.Auto;
            }
        }

        private void Grid_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                grid.RowDefinitions[2].Height = expanderHeight;
            }
        }

        private void AddUnitCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddUnitCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void AddUnitCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddUnitCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            SetLoadingGrid(visibility: true);
            var addUnitOfMeasureWindow = new AddUnitOfMeasureWindow();
            addUnitOfMeasureWindow.ShowDialog();
        }

        private void AddUnitCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AddUnitCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void AddUnitCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AddUnitCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }
    }
}
