using LogicLayer.Extensions;
using System.Windows.Media;

namespace LogicLayer.Colors
{
    /// <summary>
    /// Stores colors.
    /// The colors are represented in strings.
    /// </summary>
    public static class ColorManager
    {
        #region chart colors
        /// <summary>
        /// Stores the default chart colors.
        /// </summary>
        private static Color[] ChartColors => new Color[]{
            "#fc0505".ConvertColor(),
            "#fc7c05".ConvertColor(),
            "#fce705".ConvertColor(),
            "#80fc05".ConvertColor(),
            "#05fcf4".ConvertColor(),
            "#8005fc".ConvertColor(),
            "#e305fc".ConvertColor(),
        };

        private static int chartColorIndex = 0;
        /// <summary>
        /// Actual index of <seealso cref="ChartColors"/>.
        /// </summary>
        private static int ChartColorIndex
        {
            get
            {
                return chartColorIndex;
            }
            set
            {
                if (value >= ChartColors.Length - 1)
                {
                    value = 0;
                }
                chartColorIndex = value;
            }
        }

        /// <summary>
        /// Returns the next chart color in Color.
        /// </summary>
        public static Color GetChartColor => ChartColors[ChartColorIndex++];
        #endregion

        #region input file list element colors
        /// <summary>
        /// Default color of an input file list element.
        /// </summary>
        public static Color InputFileListElementCasualColor { get; private set; } = "#FF303030".ConvertColor();

        /// <summary>
        /// "Bad" color of an input file list element.
        /// </summary>
        public static Color InputFileListElementBadColor { get; private set; } = "#FFE21B1B".ConvertColor();
        #endregion

        public static Color ApprovedColor { get; private set; } = "#52fc3f".ConvertColor();
        public static Color DeniedColor { get; private set; } = "#fc3f3f".ConvertColor();

        #region Primary colors
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary50 { get; private set; } = "#ffe7e6".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary100 { get; private set; } = "#ffc7b8".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary200 { get; private set; } = "#ffa28a".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary300 { get; private set; } = "#ff795b".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary400 { get; private set; } = "#ff5436".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary500 { get; private set; } = "#ff200c".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary600 { get; private set; } = "#ff1507".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary700 { get; private set; } = "#ff0000".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary800 { get; private set; } = "#f10000".ConvertColor();
        /// <summary>
        /// Red
        /// </summary>
        public static Color Primary900 { get; private set; } = "#da0000".ConvertColor();
        #endregion

        #region Secondary colors
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary50 { get; private set; } = "#ffffff".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary100 { get; private set; } = "#fafafa".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary200 { get; private set; } = "#f5f5f5".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary300 { get; private set; } = "#f0f0f0".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary400 { get; private set; } = "#dedede".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary500 { get; private set; } = "#c2c2c2".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary600 { get; private set; } = "#979797".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary700 { get; private set; } = "#818181".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary800 { get; private set; } = "#606060".ConvertColor();
        /// <summary>
        /// Gray
        /// </summary>
        public static Color Secondary900 { get; private set; } = "#3c3c3c".ConvertColor();
        #endregion
    }
}
