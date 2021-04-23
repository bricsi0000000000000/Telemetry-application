using LogicLayer.Colors;
using LogicLayer.Extensions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LogicLayer.Errors
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
            OkButton.Background = ColorManager.Secondary700.ConvertBrush();
        }

        private void OkButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OkButton.Background = ColorManager.Secondary800.ConvertBrush();

            Close();
        }

        private void OkButton_MouseEnter(object sender, MouseEventArgs e)
        {
            OkButton.Background = ColorManager.Secondary800.ConvertBrush();
        }

        private void OkButton_MouseLeave(object sender, MouseEventArgs e)
        {
            OkButton.Background = ColorManager.Secondary900.ConvertBrush();
        }
    }
}
