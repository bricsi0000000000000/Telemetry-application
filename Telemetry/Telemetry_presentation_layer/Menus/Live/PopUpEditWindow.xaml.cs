using System.Windows;
using System.Windows.Input;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.Menus.Settings;
using Telemetry_presentation_layer.Menus.Settings.InputFiles;
using Telemetry_presentation_layer.Menus.Settings.Live;
using Telemetry_presentation_layer.Menus.Settings.Units;
using Telemetry_presentation_layer.ValidationRules;

namespace Telemetry_presentation_layer.Menus.Live
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

        public PopUpEditWindow(string title, EditType editType, dynamic data = null)
        {
            InitializeComponent();

            if (data.lineWidth != null)
            {
                fieldsViewModel.ChangeLineWidth = data.lineWidth;
            }
            else
            {
            }
            DataContext = fieldsViewModel;

            this.editType = editType;
            this.data = data;

            TitleTextBlock.Text = title;

            ChaneNameTextBox.Focus();
        }

        private void OkCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void OkCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            switch (editType)
            {
                case EditType.ChangeSectionName:
                    ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).ChangeName(change: true, ChaneNameTextBox.Text);
                    break;
                case EditType.ChangeLineWidth:
                    ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeLineWidth(newLineWidth: ChaneNameTextBox.Text,
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

            switch (editType)
            {
                case EditType.ChangeSectionName:
                    ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).ChangeName(change: false);
                    break;
                case EditType.ChangeLineWidth:
                    ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).ChangeLineWidth(newLineWidth: ChaneNameTextBox.Text,
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
