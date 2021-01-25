﻿using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Models;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.Menus.Settings;
using Telemetry_presentation_layer.Menus.Settings.Live;

namespace Telemetry_presentation_layer.Menus.Live
{
    /// <summary>
    /// Interaction logic for LiveSectionItem.xaml
    /// </summary>
    public partial class LiveSectionItem : UserControl
    {
        public int SectionID { get; private set; }

        private bool isActive = false;
        private bool status = false;

        public LiveSectionItem(Section section)
        {
            InitializeComponent();

            SectionID = section.ID;

            DateLabel.Content = section.DateString;
            NameLabel.Content = section.Name;

            IsLiveIcon.Kind = section.IsLive ? MaterialDesignThemes.Wpf.PackIconKind.Wifi : MaterialDesignThemes.Wpf.PackIconKind.WifiOff;

            IsLiveIcon.Foreground = section.IsLive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                     ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
        }

        public void ChangeColor(bool isActive)
        {
            this.isActive = isActive;

            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            StatusCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                               ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            DateLabel.Foreground = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50) :
                                              ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900);
            NameLabel.Foreground = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50) :
                                              ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900);

            if (isActive)
            {
                IsLiveIcon.Foreground = status ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50) :
                                                 ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
            }
            else
            {
                IsLiveIcon.Foreground = status ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                 ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
            }
        }

        public void ChangeStatus(bool status)
        {
            this.status = status;
            IsLiveIcon.Kind = status ? MaterialDesignThemes.Wpf.PackIconKind.Wifi : MaterialDesignThemes.Wpf.PackIconKind.WifiOff;

            if (isActive)
            {
                IsLiveIcon.Foreground = status ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50) :
                                                 ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
            }
            else
            {
                IsLiveIcon.Foreground = status ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                 ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
            }
        }

        private void BackgroundCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary700) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void BackgroundCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary800) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).SelectSection(SectionID);
        }

        private void BackgroundCard_MouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary800) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
        }

        private void BackgroundCard_MouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                                   ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
        }

        private void StatusCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StatusCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary700) :
                                               ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void StatusCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StatusCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary800) :
                                               ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            ((LiveSettings)((LiveMenu)MenuManager.GetTab(TextManager.LiveMenuName).Content).GetTab(TextManager.SettingsMenuName).Content).ChangeStatus(SectionID);
        }

        private void StatusCard_MouseEnter(object sender, MouseEventArgs e)
        {
            StatusCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary800) :
                                               ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
        }

        private void StatusCard_MouseLeave(object sender, MouseEventArgs e)
        {
            StatusCard.Background = isActive ? ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary900) :
                                               ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
        }
    }
}
