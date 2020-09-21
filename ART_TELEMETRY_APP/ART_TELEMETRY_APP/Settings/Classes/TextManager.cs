namespace ART_TELEMETRY_APP.Settings.Classes
{
    public static class TextManager
    {
        public static readonly string SettingsMenuName = "Settings";
        public static readonly string DiagramsMenuName = "Diagrams";
        public static readonly string DriversMenuName = "Drivers";

        public static string TracksSettingsName { get; private set; } = "Tracks";
        public static readonly string GroupsSettingsName = "Groups";

        public static readonly string DiagramCustomTabName = "Custom";

        public static readonly string SectorsSettingsName = "Sectors";

        public static string DriversCSV { get; private set; } = "drivers.csv";
        public static readonly string TracksCSV = "tracks.csv";
    }
}
