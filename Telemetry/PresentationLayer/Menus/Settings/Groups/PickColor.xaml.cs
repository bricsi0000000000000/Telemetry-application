using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using LocigLayer.Colors;
using PresentationLayer.Errors;

namespace PresentationLayer.Menus.Settings.Groups
{
    /// <summary>
    /// Interaction logic for PickColor.xaml
    /// </summary>
    public partial class PickColor : Window
    {
        public PickColor(string colorCode)
        {
            InitializeComponent();

            ColorPicker.Color = (Color)ColorConverter.ConvertFromString(colorCode);
            HexColorTextBox.Text = colorCode;
        }

        private void ChooseButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChooseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary700));
        }

        private void ChooseButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChooseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800));

            DialogResult = true;
        }

        private void ChooseButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChooseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800));
        }

        private void ChooseButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChooseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));
        }

        private void CancelButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CancelButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary700));
        }

        private void CancelButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CancelButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary800));

            DialogResult = false;
        }

        private void CancelButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CancelButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary800));
        }

        private void CancelButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CancelButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
        }

        private void HexColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string hexColor = HexColorTextBox.Text;
            if (hexColor.Length == 4 || hexColor.Length == 7 || hexColor.Length == 9)
            {
                Regex regex = new Regex(@"#([a-fA-F0-9]{6}|[a-fA-F0-9]{3}|[a-fA-F0-9]{8})");
                if (regex.IsMatch(hexColor))
                {
                    HexColorTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));
                    try
                    {
                        ColorPicker.Color = (Color)ColorConverter.ConvertFromString(hexColor);
                    }
                    catch (Exception exception)
                    {
                        ShowError.ShowErrorMessage(exception.Message, nameof(PickColor));
                    }
                }
                else
                {
                    HexColorTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
                }
            }
            else
            {
                HexColorTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
            }
        }

        private void ColorPicker_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HexColorTextBox.Text = ColorPicker.Color.ToString();
        }
    }
}
