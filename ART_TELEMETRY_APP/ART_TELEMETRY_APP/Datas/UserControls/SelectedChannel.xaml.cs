using ART_TELEMETRY_APP.Datas.Classes;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Datas.UserControls
{
    /// <summary>
    /// Interaction logic for SelectedChannel.xaml
    /// </summary>
    public partial class SelectedChannel : UserControl
    {
        bool isSelected = false;
        string channelName;
        private readonly BrushConverter converter = new BrushConverter();

        public SelectedChannel(string channelName)
        {
            InitializeComponent();

            this.channelName = channelName;
            ChannelNameLbl.Content = channelName;

            ChangeState();
        }

        private void ChangeState()
        {
            SelectChannelIcon.Kind = isSelected ? PackIconKind.CheckboxMarked : PackIconKind.CheckboxBlankOutline;
            SelectChannelIcon.Foreground = isSelected ? (Brush)converter.ConvertFromString("#FFE21B1B") : Brushes.White;
        }

        private void SelectChannel_Click(object sender, RoutedEventArgs e)
        {
            isSelected = !isSelected;
            ChangeState();
            if (isSelected)
            {
                ChartsSelectedData.SelectedChannels.Add(channelName);
            }
            else
            {
                if (ChartsSelectedData.SelectedChannels.Contains(channelName))
                {
                    ChartsSelectedData.SelectedChannels.Remove(channelName);
                }
            }
        }
    }
}
