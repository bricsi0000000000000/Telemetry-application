using System.Windows.Controls;
using System.Windows.Input;
using DataLayer.Models;
using LogicLayer.Colors;
using LogicLayer.Extensions;

namespace LogicLayer.Menus.Live
{
    /// <summary>
    /// Interaction logic for LiveSessionItem.xaml
    /// </summary>
    public partial class LiveSessionItem : UserControl
    {
        public int SessionID => session.ID;

        private readonly Session session;
        private bool isActive = false;

        public LiveSessionItem(Session session)
        {
            InitializeComponent();

            this.session = session;

            DateLabel.Content = session.DateString;
            NameLabel.Content = session.Name;
            ChangeStausIcon(session.IsLive);

            IsLiveIcon.Foreground = session.IsLive ? ColorManager.Secondary900.ConvertBrush() :
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
                IsLiveIcon.Foreground = session.IsLive ? ColorManager.Secondary50.ConvertBrush() :
                                                         ColorManager.Primary900.ConvertBrush();
            }
            else
            {
                IsLiveIcon.Foreground = session.IsLive ? ColorManager.Secondary900.ConvertBrush() :
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

            MenuManager.LiveSettings.SelectSession(SessionID);
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
