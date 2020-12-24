using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Datas.Classes
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
