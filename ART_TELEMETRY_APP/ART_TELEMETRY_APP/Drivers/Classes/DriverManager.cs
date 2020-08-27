using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Pilots
{
    public static class DriverManager
    {
        public static List<Driver> Drivers { get; } = new List<Driver>();

        public static void AddDriver(Driver driver) => Drivers.Add(driver);

        public static Driver GetDriver(string name) => Drivers.Find(n => n.Name.Equals(name));

        public static void RemoveDriver(string name) => Drivers.Remove(GetDriver(name));
    }
}
