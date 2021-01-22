using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;

namespace Telemetry_presentation_layer.Errors
{
    /// <summary>
    /// Interaction logic for ErrorMessagePopUp.xaml
    /// </summary>
    public partial class ErrorMessagePopUp : Window
    {
        public ErrorMessagePopUp(string message)
        {
            InitializeComponent();

            TitleLabel.Text = message;
        }

        private void OkButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OkButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary700));
        }

        private void OkButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OkButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800));

            Close();
        }

        private void OkButton_MouseEnter(object sender, MouseEventArgs e)
        {
            OkButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800));
        }

        private void OkButton_MouseLeave(object sender, MouseEventArgs e)
        {
            OkButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));
        }
    }
}
