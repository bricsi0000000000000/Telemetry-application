using System.Collections.Generic;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Groups.Classes
{
    /// <summary>
    /// Represents a <see cref="Group"/>.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// <see cref="Group"/>s name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// If true, you can customizable.
        /// If false, you can't customize and it doesn't show up in the settings.
        /// </summary>
        public bool Customizable { get; set; }

        /// <summary>
        /// If true, this <see cref="Group"/> is a driverless groups.
        /// </summary>
        public bool Driverless { get; set; }

        /// <summary>
        /// <see cref="Attribute"/>s that this <see cref="Group"/> uses.
        /// </summary>
        public List<Attribute> Attributes { get; } = new List<Attribute>();

        /// <summary>
        /// Constructor for <see cref="Group"/>.
        /// </summary>
        /// <param name="name"><see cref="Group"/>s name.</param>
        public Group(string name)
        {
            Name = name;
            Customizable = true;
        }

        /// <summary>
        /// Add <paramref name="name"/> to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="name">Attribute name you want to add.</param>
        public void AddAttribute(string name, Color color)
        {
            if (name.Equals(string.Empty))
            {
                return;
            }
            Attributes.Add(new Attribute(name, color));
        }

        /// <summary>
        /// Removes <paramref name="name"/> from <see cref="Attributes"/>.
        /// </summary>
        /// <param name="name">Attribute name you want to delete.</param>
        public void RemoveAttribute(string name) => Attributes.Remove(GetAttribute(name));

        public Attribute GetAttribute(string name) => Attributes.Find(x => x.Name.Equals(name));
    }
}
