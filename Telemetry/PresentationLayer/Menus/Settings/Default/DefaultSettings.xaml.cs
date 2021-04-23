using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LocigLayer.Colors;
using LocigLayer.Texts;
using PresentationLayer.Extensions;
using LogicLayer.ValidationRules;
using PresentationLayer.Defaults;

namespace LogicLayer.Menus.Settings.Default
{
    /// <summary>
    /// Interaction logic for DefaultSettings.xaml
    /// </summary>
    public partial class DefaultSettings : UserControl
    {
        private FieldsViewModel fieldsViewModel = new FieldsViewModel();

        public DefaultSettings()
        {
            InitializeComponent();

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
            DriverlessHorizontalAxisCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void DriverlessHorizontalAxisCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DriverlessHorizontalAxisCardButton.Background = ColorManager.Secondary100.ConvertBrush();

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
            DriverlessHorizontalAxisCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DriverlessHorizontalAxisCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DriverlessHorizontalAxisCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }

        private void DriverlessC0refCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DriverlessC0refCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void DriverlessC0refCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DriverlessC0refCardButton.Background = ColorManager.Secondary100.ConvertBrush();

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
            DriverlessC0refCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DriverlessC0refCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DriverlessC0refCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }

        private void DriverlessYChannelCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DriverlessYChannelCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void DriverlessYChannelCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DriverlessYChannelCardButton.Background = ColorManager.Secondary100.ConvertBrush();

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
            DriverlessYChannelCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DriverlessYChannelCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DriverlessYChannelCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }
    }
}
