namespace ART_TELEMETRY_APP.Settings.Classes
{
    public static class TextManager
    {
        #region menu names
        public static string SettingsMenuName { get; private set; } = "Settings";
        public static string DiagramsMenuName { get; private set; } = "Diagrams";
        public static string DriversMenuName { get; private set; } = "Drivers";
        public static string TracksSettingsName { get; private set; } = "Tracks";
        public static string GroupsSettingsName { get; private set; } = "Groups";
        public static string DiagramCustomTabName { get; private set; } = "Custom";
        public static string SectorsSettingsName { get; private set; } = "Sectors";
        public static string GeneralSettingsName { get; private set; } = "General";
        #endregion

        #region input file names
        public static string DriversFileName { get; private set; } = "drivers.csv";
        public static string TracksFileName { get; private set; } = "tracks.csv";
        #endregion

        #region input file default channel names
        public static string DefaultSpeedChannelName { get; private set; } = "speed";
        public static string DefaultTimeChannelName { get; private set; } = "Time";
        public static string DefaultLongitudeChannelName { get; private set; } = "Longitude";
        public static string DefaultLatitudeChannelName { get; private set; } = "Latitude";
        #endregion
    }
}
