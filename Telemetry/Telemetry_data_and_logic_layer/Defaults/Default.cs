namespace Telemetry_data_and_logic_layer.Defaults
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
