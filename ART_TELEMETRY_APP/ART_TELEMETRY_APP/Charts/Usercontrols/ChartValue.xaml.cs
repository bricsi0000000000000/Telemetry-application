using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Charts.Usercontrols
{
    /// <summary>
    /// Interaction logic for ChartValue.xaml
    /// </summary>
    public partial class ChartValue : UserControl
    {
        /// <summary>
        /// Represents a <see cref="Chart"/> value next to it.
        /// </summary>
        /// <param name="color"><see cref="Color"/> of the channel.</param>
        /// <param name="channelName">Channel name.</param>
        /// <param name="channelValue">Channel value.</param>
        public ChartValue(Color color, string channelName, double channelValue)
        {
            InitializeComponent();

            ValueColorCard.Background = new SolidColorBrush(color);
            ChannelName = channelName;
            SetChannelValue(channelValue);
        }

        private string channelName;
        public string ChannelName
        {
            get
            {
                return channelName;
            }
            set
            {
                channelName = value;
                SetChannelName(channelName);
            }
        }

        private void SetChannelName(string channelName)
        {
            ChannelNameLabel.Content = channelName;
        }

        public void SetChannelValue(double channelValue)
        {
            ChannelValueLabel.Content = $"{channelValue:f3}";
        }
    }
}
