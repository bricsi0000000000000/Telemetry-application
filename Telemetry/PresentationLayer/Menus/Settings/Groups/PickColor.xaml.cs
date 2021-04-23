using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using LogicLayer.Colors;
using LogicLayer.Errors;
using LogicLayer.Extensions;

namespace LogicLayer.Menus.Settings.Groups
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
            ChooseButton.Background = ColorManager.Secondary700.ConvertBrush();
        }

        private void ChooseButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChooseButton.Background = ColorManager.Secondary800.ConvertBrush();

            DialogResult = true;
        }

        private void ChooseButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChooseButton.Background = ColorManager.Secondary800.ConvertBrush();
        }

        private void ChooseButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ChooseButton.Background = ColorManager.Secondary900.ConvertBrush();
        }

        private void CancelButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CancelButton.Background = ColorManager.Primary700.ConvertBrush();
        }

        private void CancelButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CancelButton.Background = ColorManager.Primary800.ConvertBrush();

            DialogResult = false;
        }

        private void CancelButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CancelButton.Background = ColorManager.Primary800.ConvertBrush();
        }

        private void CancelButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CancelButton.Background = ColorManager.Primary900.ConvertBrush();
        }

        private void HexColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string hexColor = HexColorTextBox.Text;
            if (hexColor.Length == 4 || hexColor.Length == 7 || hexColor.Length == 9)
            {
                Regex regex = new Regex(@"#([a-fA-F0-9]{6}|[a-fA-F0-9]{3}|[a-fA-F0-9]{8})");
                if (regex.IsMatch(hexColor))
                {
                    HexColorTextBox.Foreground = ColorManager.Secondary900.ConvertBrush();
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
                    HexColorTextBox.Foreground = ColorManager.Primary900.ConvertBrush();
                }
            }
            else
            {
                HexColorTextBox.Foreground = ColorManager.Primary900.ConvertBrush();
            }
        }

        private void ColorPicker_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HexColorTextBox.Text = ColorPicker.Color.ToString();
        }
    }
}
