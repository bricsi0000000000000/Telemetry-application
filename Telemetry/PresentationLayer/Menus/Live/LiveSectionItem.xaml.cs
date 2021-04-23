using System.Windows.Controls;
using System.Windows.Input;
using DataLayer.Models;
using LocigLayer.Colors;
using PresentationLayer.Extensions;

namespace LogicLayer.Menus.Live
{
    /// <summary>
    /// Interaction logic for LiveSectionItem.xaml
    /// </summary>
    public partial class LiveSectionItem : UserControl
    {
        public int SectionID => section.ID;

        private readonly Section section;
        private bool isActive = false;

        public LiveSectionItem(Section section)
        {
            InitializeComponent();

            this.section = section;

            DateLabel.Content = section.DateString;
            NameLabel.Content = section.Name;
            ChangeStausIcon(section.IsLive);

            IsLiveIcon.Foreground = section.IsLive ? ColorManager.Secondary900.ConvertBrush() :
                                                     ColorManager.Primary900.ConvertBrush();
        }

        private void ChangeStausIcon(bool isLive)
        {
            IsLiveIcon.Kind = isLive ? MaterialDesignThemes.Wpf.PackIconKind.AccessPoint : MaterialDesignThemes.Wpf.PackIconKind.AccessPointOff;
        }

        public void ChangeColor(bool isActive)
        {
            this.isActive = isActive;

            BackgroundCard.Background = isActive ? ColorManager.Secondary900.ConvertBrush() :
                                                   ColorManager.Secondary50.ConvertBrush();
            StatusCard.Background = isActive ? ColorManager.Secondary900.ConvertBrush() :
                                               ColorManager.Secondary50.ConvertBrush();
            DateLabel.Foreground = isActive ? ColorManager.Secondary50.ConvertBrush() :
                                              ColorManager.Secondary900.ConvertBrush();
            NameLabel.Foreground = isActive ? ColorManager.Secondary50.ConvertBrush() :
                                              ColorManager.Secondary900.ConvertBrush();

            if (isActive)
            {
                IsLiveIcon.Foreground = section.IsLive ? ColorManager.Secondary50.ConvertBrush() :
                                                         ColorManager.Primary900.ConvertBrush();
            }
            else
            {
                IsLiveIcon.Foreground = section.IsLive ? ColorManager.Secondary900.ConvertBrush() :
                                                         ColorManager.Primary900.ConvertBrush();
            }
        }

        public void ChangeStatus(bool status)
        {
            ChangeStausIcon(status);

            if (isActive)
            {
                IsLiveIcon.Foreground = status ? ColorManager.Secondary50.ConvertBrush() :
                                                 ColorManager.Primary900.ConvertBrush();
            }
            else
            {
                IsLiveIcon.Foreground = status ? ColorManager.Secondary900.ConvertBrush() :
                                                 ColorManager.Primary900.ConvertBrush();
            }
        }

        private void BackgroundCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isActive ? ColorManager.Secondary700.ConvertBrush() :
                                                   ColorManager.Secondary200.ConvertBrush();

            StatusCard.Background = isActive ? ColorManager.Secondary700.ConvertBrush() :
                                               ColorManager.Secondary200.ConvertBrush();
        }

        private void BackgroundCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BackgroundCard.Background = isActive ? ColorManager.Secondary800.ConvertBrush() :
                                                   ColorManager.Secondary100.ConvertBrush();

            StatusCard.Background = isActive ? ColorManager.Secondary800.ConvertBrush() :
                                               ColorManager.Secondary100.ConvertBrush();

            MenuManager.LiveSettings.SelectSection(SectionID);
        }

        private void BackgroundCard_MouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isActive ? ColorManager.Secondary800.ConvertBrush() :
                                                   ColorManager.Secondary100.ConvertBrush();

            StatusCard.Background = isActive ? ColorManager.Secondary800.ConvertBrush() :
                                               ColorManager.Secondary100.ConvertBrush();

            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void BackgroundCard_MouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundCard.Background = isActive ? ColorManager.Secondary900.ConvertBrush() :
                                                   ColorManager.Secondary50.ConvertBrush();

            StatusCard.Background = isActive ? ColorManager.Secondary900.ConvertBrush() :
                                               ColorManager.Secondary50.ConvertBrush();

            Mouse.OverrideCursor = null;
        }
    }
}
