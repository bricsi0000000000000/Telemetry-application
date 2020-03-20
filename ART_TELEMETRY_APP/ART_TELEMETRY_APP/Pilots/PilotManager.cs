using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Pilots
{
    public static class PilotManager
    {
        static List<Pilot> pilots = new List<Pilot>();

        public static void AddPilot(Pilot pilot)
        {
            pilots.Add(pilot);
        }

        public static Pilot GetPilot(string name)
        {
            return pilots.Find(n => n.Name == name);
        }

        public static List<Pilot> Pilots
        {
            get
            {
                return pilots;
            }
        }

        public static bool SettingsIsOpen = false;
    }
}
