namespace Telemetry_data_and_logic_layer.Texts
{
    /// <summary>
    /// Stores default values for texts.
    /// </summary>
    public static class TextManager
    {
        #region menu names

        #region main menu
        /// <summary>
        /// Name of the driverless menu.
        /// "Driverless"
        /// </summary>
        public static string DriverlessMenuName { get; private set; } = "Driverless";

        /// <summary>
        /// Name of the settings menu.
        /// "Settings"
        /// </summary>
        public static string SettingsMenuName { get; private set; } = "Settings";

        /// <summary>
        /// Name of the live menu.
        /// "Live"
        /// </summary>
        public static string LiveMenuName { get; private set; } = "Live";

        /// <summary>
        /// Name of the diagrams menu.
        /// "Diagrams"
        /// </summary>
        public static string DiagramsMenuName { get; private set; } = "Diagrams";

        /// <summary>
        /// Name of the diagrams settings menu.
        /// "Diagrams settings"
        /// </summary>
        public static string DiagramsSettingsMenuName { get; private set; } = "Diagrams settings";

        /// <summary>
        /// Name of the drivers menu.
        /// "Drivers"
        /// </summary>
        public static string DriversMenuName { get; private set; } = "Drivers";
        #endregion

        #region settings menu

        /// <summary>
        /// Name of the track settings menu.
        /// "Tracks"
        /// </summary>
        public static string TracksSettingsName { get; private set; } = "Tracks";

        /// <summary>
        /// Name of the group settings menu.
        /// "Groups"
        /// </summary>
        public static string GroupsSettingsName { get; private set; } = "Groups";

        /// <summary>
        /// Name of the file settings menu.
        /// "Files"
        /// </summary>
        public static string FilesSettingsName { get; private set; } = "Files";

        /// <summary>
        /// Name of the sector settings menu.
        /// "Sectors"
        /// </summary>
        public static string SectorsSettingsName { get; private set; } = "Sectors";

        /// <summary>
        /// Name of the general settings menu.
        /// "General"
        /// </summary>
        public static string GeneralSettingsName { get; private set; } = "General";
        #endregion

        #endregion

        #region tab names
        /// <summary>
        /// Name of the custom diagram tab.
        /// "Custom"
        /// </summary>
        public static string DiagramCustomTabName { get; private set; } = "Custom";

        /// <summary>
        /// Name of the traction tab.
        /// "Traction"
        /// </summary>
        public static string TractionTabName { get; private set; } = "Traction";

        /// <summary>
        /// Name of the lap report tab.
        /// "Lap report"
        /// </summary>
        public static string LapReportTabName { get; private set; } = "Lap report";
        #endregion

        #region input file names
        /// <summary>
        /// Name of the drivers file.
        /// "drivers.csv"
        /// </summary>
        public static string DriversFileName { get; private set; } = "drivers.csv";

        /// <summary>
        /// Name of the tracks file.
        /// "tracks.csv"
        /// </summary>
        public static string TracksFileName { get; private set; } = "tracks.csv";

        /// <summary>
        /// Name of the groups file.
        /// "groups.json"
        /// </summary>
        public static string GroupsFileName { get; private set; } = "groups.json";
        #endregion

        #region input file default channel names
        /// <summary>
        /// Default name of channel speed.
        /// "speed"
        /// </summary>
        public static string DefaultSpeedChannelName { get; private set; } = "speed";

        /// <summary>
        /// Default name of channel time.
        /// "Time"
        /// </summary>
        public static string DefaultTimeChannelName { get; private set; } = "Time";

        /// <summary>
        /// Default name of channel longitude.
        /// "Longitude"
        /// </summary>
        public static string DefaultLongitudeChannelName { get; private set; } = "Longitude";

        /// <summary>
        /// Default name of channel latitude.
        /// "Latitude"
        /// </summary>
        public static string DefaultLatitudeChannelName { get; private set; } = "Latitude";

        /// <summary>
        /// Default name of channel yaw angle.
        /// "yawangle"
        /// </summary>
        public static string DefaultYawAngleChannelName { get; private set; } = "yawangle";

        /// <summary>
        /// Default name of channel yaw rate.
        /// "yawrate"
        /// </summary>
        public static string DefaultYawRateChannelName { get; private set; } = "yawrate";
        #endregion
    }
}
