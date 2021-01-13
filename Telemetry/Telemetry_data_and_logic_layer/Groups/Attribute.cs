namespace Telemetry_data_and_logic_layer.Groups
{
    /// <summary>
    /// Represents an <see cref="Attribute"/> in a <see cref="Group"/>.
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// <see cref="Attribute"/>s name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// <see cref="Attribute"/>s color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"><see cref="Attribute"/>s name.</param>
        /// <param name="color"><see cref="Attribute"/>s color.</param>
        public Attribute(string name, string color)
        {
            Name = name.Trim();
            Color = color.Trim();
        }
    }
}
