using System.Collections.Generic;

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
        /// The channel names that this <see cref="Group"/> uses.
        /// </summary>
        public List<string> Attributes { get; } = new List<string>();

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
        /// Add <paramref name="attribute"/> to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="attribute">Attribute name you want to add.</param>
        public void AddAttribute(string attribute)
        {
            if (attribute.Equals(string.Empty))
            {
                return;
            }
            Attributes.Add(attribute);
        }

        /// <summary>
        /// Removes <paramref name="attribute"/> from <see cref="Attributes"/>.
        /// </summary>
        /// <param name="attribute">Attribute name you want to delete.</param>
        public void RemoveAttribute(string attribute) => Attributes.Remove(attribute);
    }
}
