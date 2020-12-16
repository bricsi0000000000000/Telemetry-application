namespace ART_TELEMETRY_APP.Drivers.Classes
{
    public class Driver
    {
        public string Name { get; }
        public bool IsSelected { get; set; }

        public Driver(string name)
        {
            Name = name;
            IsSelected = false;
        }
    }
}
