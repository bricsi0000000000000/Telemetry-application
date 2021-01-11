using System.Windows.Controls;
using System.Windows.Media;

namespace Telemetry_presentation_layer.Charts
{
    /// <summary>
    /// Represents the a single value next to a <see cref="Chart"/>.
    /// </summary>
    public partial class ChartValue : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="color"><see cref="Color"/> of the channel.</param>
        /// <param name="channelName">Channel name.</param>
        /// <param name="channelValue">Channel value.</param>
        public ChartValue(string color, string channelName, double channelValue)
        {
            InitializeComponent();

            ValueColorCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            ChannelName = channelName;
            SetChannelValue(channelValue);
        }

        /// <summary>
        /// <see cref="Channel"/>s name whose data is represented.
        /// </summary>
        private string channelName;

        /// <summary>
        /// <see cref="Channel"/>s name whose data is represented.
        /// </summary>
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

        /// <summary>
        /// Sets the <see cref="ChannelNameLabel"/>s text to <paramref name="channelName"/>.
        /// </summary>
        /// <param name="channelName"><see cref="Channel"/>s name.</param>
        private void SetChannelName(string channelName)
        {
            ChannelNameLabel.Content = channelName;
        }

        /// <summary>
        /// Sets the <see cref="ChannelValueLabel"/>s text to <paramref name="channelValue"/>.
        /// </summary>
        /// <param name="channelValue"><see cref="Channel"/>s value.</param>
        public void SetChannelValue(double channelValue)
        {
            ChannelValueLabel.Content = $"{channelValue:f3}";
        }
    }
}
