using System.Collections.Generic;

namespace ART_TELEMETRY_APP.Drivers.Classes
{
    public static class DriverManager
    {
        public static List<Driver> Drivers { get; private set; } = new List<Driver>();

        public static void AddDriver(Driver driver) => Drivers.Add(driver);

        public static Driver GetDriver(string name) => Drivers.Find(x => x.Name.Equals(name));

        public static void RemoveDriver(string name) => Drivers.Remove(GetDriver(name));
    }
}
