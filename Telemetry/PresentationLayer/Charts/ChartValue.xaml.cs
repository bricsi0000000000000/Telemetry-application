using System.Windows.Controls;
using System.Windows.Media;
using DataLayer.Groups;
using LogicLayer.Colors;
using LogicLayer.Extensions;

namespace PresentationLayer.Charts
{
    /// <summary>
    /// Represents the a single value next to a <see cref="Chart"/>.
    /// </summary>
    public partial class ChartValue : UserControl
    {
        private string colorCode;
        private int inputFileID;
        private string groupName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="color"><see cref="Color"/> of the channel.</param>
        /// <param name="channelName">Channel name.</param>
        /// <param name="value">Channel value.</param>
        public ChartValue(string channelName, string unitOfMeasure, string groupName/*, string color = "#ffffff", int inputFileID = -1*/)
        {
            InitializeComponent();

            ChannelNameLabel.Opacity = .4f;
            ChannelValueLabel.Opacity = .4f;
            UnitOfMeasureFormulaControl.Opacity = .4f;

            //colorCode = color;
           // this.inputFileID = inputFileID;
            this.groupName = groupName;

            //ColorCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(color);
            ChannelName = channelName;
            SetChannelValue(0);
            var formula = @"\color[HTML]{" + ColorManager.Secondary900.ToString()[3..] + "}{" + unitOfMeasure + "}";
            UnitOfMeasureFormulaControl.Formula = formula;
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
            if (inputFileID != -1)
            {
                ChannelNameLabel.Content = $"{channelName} {inputFileID}";
            }
            else
            {
                ChannelNameLabel.Content = channelName;
            }
        }

        /// <summary>
        /// Sets the <see cref="ChannelValueLabel"/>s text to <paramref name="channelValue"/>.
        /// </summary>
        /// <param name="channelValue"><see cref="Channel"/>s value.</param>
        public void SetChannelValue(double channelValue)
        {
            if (inputFileID != -1)
            {
                ChannelValueLabel.Content = $"{channelValue:f3}";
            }
        }

        public void SetLiveChannelValue(double value)
        {
            ChannelValueLabel.Content = $"{value:f3}";
        }

        public void SetUp(string colorText, int inputFileID)
        {
            ChannelNameLabel.Opacity = 1;
            ChannelValueLabel.Opacity = 1;
            UnitOfMeasureFormulaControl.Opacity = 1;

            this.inputFileID = inputFileID;
            colorCode = colorText;
            ColorCard.Background = colorText.ConvertBrush();

            SetChannelName(channelName);
        }
    }
}
