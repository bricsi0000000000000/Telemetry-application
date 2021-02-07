using System;
using System.Collections.Generic;
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

namespace Telemetry_presentation_layer.Menus.Settings.Units
{
    /// <summary>
    /// Interaction logic for UnitsMenu.xaml
    /// </summary>
    public partial class UnitsMenu : UserControl
    {
        private Unit activeUnit;

        public UnitsMenu()
        {
            InitializeComponent();

            if (UnitOfMeasureManager.UnitOfMeasures.Count > 0)
            {
                activeUnit = UnitOfMeasureManager.UnitOfMeasures[0];
                InitializeUnits();
            }
        }

        private void InitializeUnits()
        {
            UnitsStackPanel.Children.Clear();

            foreach (var unit in UnitOfMeasureManager.UnitOfMeasures)
            {
                UnitsStackPanel.Children.Add(new UnitItem(unit));
            }

            SelectUnit(activeUnit);
        }

        public void UpdateUnit(bool update)
        {
            if (update)
            {

            }
        }

        public void DeleteUnit(bool delete)
        {
            if (delete)
            {

            }
        }

        public void SelectUnit(Unit unit)
        {
            if (activeUnit.ID != unit.ID)
            {
                activeUnit = unit;

                UpdateSelectedUnitUI();

                foreach (var item in UnitsStackPanel.Children)
                {
                    ((UnitItem)item).ChangeColor(((UnitItem)item).Unit.ID == unit.ID);
                }
            }
        }

        private void UpdateSelectedUnitUI()
        {
            //TODO edit unit
        }

        public void ChangeUnitName(string newName)
        {
            UnitOfMeasureManager.ChangeUnitOfMeasureName(activeUnit.ID, newName);
            InitializeUnits();
        }

        private void DeleteSectionCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary700);
        }

        private void DeleteSectionCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);

            var deleteSectionWindow = new PopUpWindow($"You are about to delete {activeUnit.Name}\n" +
                                                      $"Are you sure about that?",
                                                      PopUpWindow.PopUpType.DeleteUnit);
            deleteSectionWindow.ShowDialog();
        }

        private void DeleteSectionCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DeleteSectionCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DeleteSectionCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
            Mouse.OverrideCursor = null;
        }

        private void ChangeSectionNameCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeUnitNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void ChangeSectionNameCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeUnitNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            var changeNameWindow = new PopUpEditWindow("Change unit", PopUpEditWindow.EditType.ChangeUnitName);
            changeNameWindow.ShowDialog();
        }

        private void ChangeSectionNameCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeUnitNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSectionNameCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChangeUnitNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }
    }
}
