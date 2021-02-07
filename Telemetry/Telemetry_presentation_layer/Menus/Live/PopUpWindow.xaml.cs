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
using System.Windows.Shapes;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.Menus.Settings;
using Telemetry_presentation_layer.Menus.Settings.Live;
using Telemetry_presentation_layer.Menus.Settings.Units;

namespace Telemetry_presentation_layer.Menus.Live
{
    /// <summary>
    /// Interaction logic for ChangeLiveStatusWindow.xaml
    /// </summary>
    public partial class PopUpWindow : Window
    {
        public enum PopUpType { ChangeLiveStatus, ChangeUnit, DeleteSection, DeleteUnit }

        private readonly PopUpType popUpType;

        public PopUpWindow(string title, PopUpType popUpType)
        {
            InitializeComponent();

            this.popUpType = popUpType;
            TitleTextBox.Text = title;
        }

        private void OkCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void OkCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            switch (popUpType)
            {
                case PopUpType.ChangeLiveStatus:
                    ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).ChangeStatusResultAsync(change: true);
                    break;
                case PopUpType.DeleteSection:
                    ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).DeleteSeciton(delete: true);
                    break;
                case PopUpType.ChangeUnit:
                    ((UnitsMenu)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).UpdateUnit(update: true);
                    break;
                case PopUpType.DeleteUnit:
                    ((UnitsMenu)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).DeleteUnit(delete: true);
                    break;
            }

            Close();
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

            switch (popUpType)
            {
                case PopUpType.ChangeLiveStatus:
                    ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).ChangeStatusResultAsync(change: false);
                    break;
                case PopUpType.DeleteSection:
                    ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).DeleteSeciton(delete: false);
                    break;
                case PopUpType.ChangeUnit:
                    ((UnitsMenu)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).UpdateUnit(update: true);
                    break;
                case PopUpType.DeleteUnit:
                    ((UnitsMenu)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).DeleteUnit(delete: true);
                    break;
            }
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
