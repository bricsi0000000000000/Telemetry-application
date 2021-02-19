using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Defaults;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.ValidationRules;

namespace Telemetry_presentation_layer.Menus.Settings.Default
{
    /// <summary>
    /// Interaction logic for DefaultSettings.xaml
    /// </summary>
    public partial class DefaultSettings : UserControl
    {
        FieldsViewModel fieldsViewModel = new FieldsViewModel();

        public DefaultSettings()
        {
            InitializeComponent();

            DefaultsManager.LoadDefaults(TextManager.DefaultFileName);
            InitializeDefaults();

            DataContext = fieldsViewModel;
        }

        private void InitializeDefaults()
        {
            fieldsViewModel.DriverlessHorizontalAxis = DefaultsManager.GetDefault(TextManager.DriverlessHorizontalAxis).Value;
            fieldsViewModel.DriverlessC0refChannel = DefaultsManager.GetDefault(TextManager.DriverlessC0refChannel).Value;
            fieldsViewModel.DriverlessYChannel = DefaultsManager.GetDefault(TextManager.DriverlessYChannel).Value;
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

        private void DriverlessHorizontalAxisCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DriverlessHorizontalAxisCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void DriverlessHorizontalAxisCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DriverlessHorizontalAxisCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            try
            {
                DefaultsManager.GetDefault(TextManager.DriverlessHorizontalAxis).Value = DriverlessHorizontalAxisTextBox.Text;
                DefaultsManager.SaveDefaults();
                ShowErrorMessage("Updated successfully", error: false);
            }
            catch (Exception)
            {
                ShowErrorMessage("There was an error while updating the 'driverless horizontal axis' settings", error: false, time: 5);
            }

        }

        private void DriverlessHorizontalAxisCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            DriverlessHorizontalAxisCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DriverlessHorizontalAxisCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DriverlessHorizontalAxisCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void DriverlessC0refCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DriverlessC0refCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void DriverlessC0refCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DriverlessC0refCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            try
            {
                DefaultsManager.GetDefault(TextManager.DriverlessC0refChannel).Value = DriverlessC0refTextBox.Text;
                DefaultsManager.SaveDefaults();
                ShowErrorMessage("Updated successfully", error: false);
            }
            catch (Exception)
            {
                ShowErrorMessage("There was an error while updating the 'driverless c0ref channel' settings", error: false, time: 5);
            }
        }

        private void DriverlessC0refCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            DriverlessC0refCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DriverlessC0refCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DriverlessC0refCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void DriverlessYChannelCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DriverlessYChannelCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void DriverlessYChannelCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DriverlessYChannelCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            try
            {
                DefaultsManager.GetDefault(TextManager.DriverlessYChannel).Value = DriverlessYChannelTextBox.Text;
                DefaultsManager.SaveDefaults();
                ShowErrorMessage("Updated successfully", error: false);
            }
            catch (Exception)
            {
                ShowErrorMessage("There was an error while updating the 'driverless y channel' settings", error: false, time: 5);
            }
        }

        private void DriverlessYChannelCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            DriverlessYChannelCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DriverlessYChannelCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DriverlessYChannelCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }
    }
}
