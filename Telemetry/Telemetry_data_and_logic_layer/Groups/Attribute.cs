namespace DataLayer.Groups
{
    /// <summary>
    /// Represents an <see cref="Attribute"/> in a <see cref="Group"/>.
    /// </summary>
    public class Attribute
    {
        public int ID { get; private set; }
        /// <summary>
        /// <see cref="Attribute"/>s name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <see cref="Attribute"/>s color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// <see cref="Attribute"/>s line width.
        /// </summary>
        public int LineWidth { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"><see cref="Attribute"/>s name.</param>
        /// <param name="color"><see cref="Attribute"/>s color.</param>
        public Attribute(int id, string name, string color, int lineWidth)
        {
            ID = id;
            Name = name.Trim();
            Color = color.Trim();
            LineWidth = lineWidth;
            IsActive = true;
        }
    }
}
