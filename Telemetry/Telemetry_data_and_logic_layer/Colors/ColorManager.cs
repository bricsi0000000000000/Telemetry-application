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

        public static string ApprovedColor { get; private set; } = "#52fc3f";
        public static string DeniedColor { get; private set; } = "#fc3f3f";

        #region Primary colors
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary50 { get; private set; } = "#ffe7e6";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary100 { get; private set; } = "#ffc7b8";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary200 { get; private set; } = "#ffa28a";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary300 { get; private set; } = "#ff795b";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary400 { get; private set; } = "#ff5436";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary500 { get; private set; } = "#ff200c";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary600 { get; private set; } = "#ff1507";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary700 { get; private set; } = "#ff0000";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary800 { get; private set; } = "#f10000";
        /// <summary>
        /// Red
        /// </summary>
        public static string Primary900 { get; private set; } = "#da0000";
        #endregion

        #region Secondary colors
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary50 { get; private set; } = "#ffffff";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary100 { get; private set; } = "#fafafa";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary200 { get; private set; } = "#f5f5f5";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary300 { get; private set; } = "#f0f0f0";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary400 { get; private set; } = "#dedede";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary500 { get; private set; } = "#c2c2c2";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary600 { get; private set; } = "#979797";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary700 { get; private set; } = "#818181";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary800 { get; private set; } = "#606060";
        /// <summary>
        /// Gray
        /// </summary>
        public static string Secondary900 { get; private set; } = "#3c3c3c";
        #endregion
    }
}
