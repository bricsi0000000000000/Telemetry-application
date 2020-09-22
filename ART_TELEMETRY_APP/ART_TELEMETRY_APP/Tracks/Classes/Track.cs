using System.Windows;

namespace ART_TELEMETRY_APP
{
    public class Track
    {
        public Track(string name, string description)
        {
            Name = name;
            Description = description;
            Processed = false;
        }

        public string Name { get; set; }
        public string Description{ get; set; }
        public Point StarPoint { get; set; }
        public bool Processed { get; set; }

        private bool Equals(Track track) => track.Name.Equals(Name) && track.Description.Equals(Description);

        public override bool Equals(object obj) => Equals(obj as Track);

        public override string ToString() => base.ToString();

        public override int GetHashCode() => base.GetHashCode();
    }
}
