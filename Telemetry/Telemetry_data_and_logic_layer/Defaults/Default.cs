namespace DataLayer.Defaults
{
    public class Default
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Default(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
