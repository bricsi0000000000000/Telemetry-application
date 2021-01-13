namespace Telemetry_data_and_logic_layer.Colors
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
        private static string[] ChartColors => new string[]{
            "#fc0505",
            "#fc7c05",
            "#fce705",
            "#80fc05",
            "#05fcf4",
            "#8005fc",
            "#e305fc",
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
        /// Returns the next chart color in string.
        /// </summary>
        public static string GetChartColor => ChartColors[ChartColorIndex++];
        #endregion

        #region input file list element colors
        /// <summary>
        /// Default color of an input file list element.
        /// </summary>
        public static string InputFileListElementCasualColor { get; private set; } = "#FF303030";

        /// <summary>
        /// "Bad" color of an input file list element.
        /// </summary>
        public static string InputFileListElementBadColor { get; private set; } = "#FFE21B1B";
        #endregion
    }
}
