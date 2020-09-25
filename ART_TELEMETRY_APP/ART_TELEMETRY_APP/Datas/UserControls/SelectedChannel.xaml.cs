using ART_TELEMETRY_APP.Datas.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            SelectChannelIcon.Kind = isSelected ? MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarked : MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline;
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

            foreach (var item in ChartsSelectedData.SelectedChannels)
            {
                Console.Write(item + "," );
            }
            Console.WriteLine();
        }
    }
}
