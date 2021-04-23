using DataLayer.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresentationLayer.Drivers
{
    public static class DriverManager
    {
        public static List<Driver> Drivers { get; private set; } = new List<Driver>();

        public static void AddDriver(Driver driver) => Drivers.Add(driver);

        public static Driver GetDriver(string name) => Drivers.Find(x => x.Name.Equals(name));

        public static void RemoveDriver(string name) => Drivers.Remove(GetDriver(name));

        public static ushort SelectedDriversCount => (ushort)Drivers.Select(x => x.IsSelected).Count();
    }
}
