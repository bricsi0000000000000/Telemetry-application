using System.Windows;

namespace ART_TELEMETRY_APP
{
    public class Track
    {
        public Track(string name, int year)
        {
            Name = name;
            Year = year;
            Processed = false;
        }

        public string Name { get; set; }
        public int Year { get; set; }
        public Point StarPoint { get; set; }
        public bool Processed { get; set; }

        private bool Equals(Track map) => map.Name.Equals(Name) && map.Year.Equals(Year);

        public override bool Equals(object obj) => Equals(obj as Track);

        public override string ToString() => base.ToString();

        public override int GetHashCode() => base.GetHashCode();
    }
}
