using System;
using System.Windows;
using System.Windows.Input;
using LogicLayer.Colors;
using LogicLayer.Extensions;

namespace LogicLayer.Menus.Live
{
    /// <summary>
    /// Interaction logic for PopUpEditDateWindow.xaml
    /// </summary>
    public partial class PopUpEditDateWindow : Window
    {
        public PopUpEditDateWindow(string title)
        {
            InitializeComponent();

            TitleTextBlock.Text = title;

            PickDateDatePicker.SelectedDate = DateTime.Now;
            PickTimeTimePicker.SelectedTime = DateTime.Now;
        }

        private void OkCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void OkCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OkCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            var date = (DateTime)PickDateDatePicker.SelectedDate;
            var time = (DateTime)PickTimeTimePicker.SelectedTime;
            var newDate = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);

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
