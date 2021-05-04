using System.Windows;
using System.Windows.Input;
using LogicLayer.Colors;
using PresentationLayer.Texts;
using LogicLayer.Extensions;
using LogicLayer.Menus.Settings;
using LogicLayer.Menus.Settings.Groups;
using LogicLayer.Menus.Settings.InputFiles;
using LogicLayer.Menus.Settings.Live;
using LogicLayer.Menus.Settings.Units;

namespace LogicLayer.Menus.Live
{
    /// <summary>
    /// Interaction logic for ChangeLiveStatusWindow.xaml
    /// </summary>
    public partial class PopUpWindow : Window
    {
        public enum PopUpType { ChangeLiveStatus, DeleteSession, DeleteUnit, DeleteGroup, DeleteGroupAttribute, DeleteInputFile }

        private readonly PopUpType popUpType;

        public PopUpWindow(string title, PopUpType popUpType)
        {
            InitializeComponent();

            this.popUpType = popUpType;
            TitleTextBox.Text = title;
        }

        private void OkCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void OkCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            switch (popUpType)
            {
                case PopUpType.ChangeLiveStatus:
                    MenuManager.LiveSettings.ChangeStatusResultAsync(change: true);
                    break;
                case PopUpType.DeleteSession:
                    ((LiveSettings)((LiveMenu)MenuManager.GetMenuTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).DeleteSeciton(delete: true);
                    break;
                case PopUpType.DeleteUnit:
                    ((UnitsMenu)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).DeleteUnit(delete: true);
                    break;
                case PopUpType.DeleteGroup:
                    ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).DeleteGroup(delete: true);
                    break;
                case PopUpType.DeleteGroupAttribute:
                    ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).DeleteAttribute(delete: true);
                    break;
                case PopUpType.DeleteInputFile:
                    ((InputFilesSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).DeleteInputFile(delete: true);
                    break;
            }

            Close();
        }

        private void OkCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            OkCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void OkCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            OkCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }

        private void CancelCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CancelCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void CancelCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CancelCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            switch (popUpType)
            {
                case PopUpType.ChangeLiveStatus:
                    ((LiveSettings)((LiveMenu)MenuManager.GetMenuTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).ChangeStatusResultAsync(change: false);
                    break;
                case PopUpType.DeleteSession:
                    ((LiveSettings)((LiveMenu)MenuManager.GetMenuTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).DeleteSeciton(delete: false);
                    break;
                case PopUpType.DeleteUnit:
                    ((UnitsMenu)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.UnitsSettingsName).Content).DeleteUnit(delete: false);
                    break;
                case PopUpType.DeleteGroup:
                    ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).DeleteGroup(delete: false);
                    break;
                case PopUpType.DeleteGroupAttribute:
                    ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).DeleteAttribute(delete: false);
                    break;
                case PopUpType.DeleteInputFile:
                    ((InputFilesSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).DeleteInputFile(delete: false);
                    break;
            }
            Close();
        }

        private void CancelCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CancelCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void CancelCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CancelCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }
    }
}
