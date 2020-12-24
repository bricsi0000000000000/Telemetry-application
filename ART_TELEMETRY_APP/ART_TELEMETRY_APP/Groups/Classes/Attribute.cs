using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Groups.Classes
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
        public Color Color { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"><see cref="Attribute"/>s name.</param>
        /// <param name="color"><see cref="Attribute"/>s color.</param>
        public Attribute(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
