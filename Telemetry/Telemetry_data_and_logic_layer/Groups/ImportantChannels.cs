using System.Collections.Generic;

namespace DataLayer.Groups
{
    public static class ImportantChannels
    {
        public static List<string> DriverlessImportantChannelNames => new List<string>()
        {
            "yawrate"
        };

        public static List<string> StandardImportantChannelNames => new List<string>()
        {
            "time"
        };
    }
}
