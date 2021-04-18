using System.Windows;
using System.Windows.Input;
using LocigLayer.Colors;
using LocigLayer.Texts;
using PresentationLayer.Extensions;
using PresentationLayer.Menus.Settings;
using PresentationLayer.Menus.Settings.InputFiles;
using PresentationLayer.Menus.Settings.Live;
using PresentationLayer.ValidationRules;

namespace PresentationLayer.Menus.Live
{
    /// <summary>
    /// Interaction logic for PopUpEditWindow.xaml
    /// </summary>
    public partial class PopUpEditWindow : Window
    {
        public enum EditType { ChangeSectionName, ChangeLineWidth }
        private EditType editType;

        private readonly FieldsViewModel fieldsViewModel = new FieldsViewModel();

        private dynamic data;

        public PopUpEditWindow(string windowTitle, EditType editType, dynamic data = null)
        {
            InitializeComponent();

            if (data.lineWidth != null)
            {
                fieldsViewModel.ChangeLineWidth = data.lineWidth;
            }

            DataContext = fieldsViewModel;

            this.editType = editType;
            this.data = data;

            TitleTextBlock.Text = windowTitle;

            ChaneNameTextBox.Focus();
        }

        private void OkCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void OkCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            switch (editType)
            {
                case EditType.ChangeSectionName:
                    break;
                case EditType.ChangeLineWidth:
                    ((InputFilesSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeLineWidth(newLineWidth: ChaneNameTextBox.Text,
                                                                                                                                                                                 inputFileID: data.inputFileID,
                                                                                                                                                                                 channelName: data.channelName,
                                                                                                                                                                                 isGroup: data.isGroup,
                                                                                                                                                                                 change: true);
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

            switch (editType)
            {
                case EditType.ChangeSectionName:
                    break;
                case EditType.ChangeLineWidth:
                    ((InputFilesSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeLineWidth(newLineWidth: ChaneNameTextBox.Text,
                                                                                                                                                                                 inputFileID: data.inputFileID,
                                                                                                                                                                                 channelName: data.channelName,
                                                                                                                                                                                 isGroup: data.isGroup,
                                                                                                                                                                                 change: false);
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
