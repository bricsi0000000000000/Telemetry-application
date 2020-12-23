using ART_TELEMETRY_APP.Datas.Classes;
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
        /// If true, you can customize this <see cref="Group"/> in settings.
        /// If false, you can't customize and it doesn't show up in the settings.
        /// </summary>
        public bool Customizable { get; set; }

        /// <summary>
        /// If true, this <see cref="Group"/> is a driverless group.
        /// If false, this <see cref="Group"/> is a standard group.
        /// </summary>
        public bool Driverless { get; set; }

        /// <summary>
        /// <see cref="Attribute"/>s used by this <see cref="Group"/>.
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
        /// Creates a new <see cref="Attribute"/> and adds to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="name">Attribute name you want to add.</param>
        /// <param name="color">Attribute color you want to add.</param>
        public void AddAttribute(string name, Color color)
        {
            if (name.Equals(string.Empty))
            {
                return;
            }
            Attributes.Add(new Attribute(name, color));
        }

        /// <summary>
        /// Creates a new <see cref="Attribute"/> and adds to <see cref="Attributes"/>.
        /// </summary>
        /// <param name="channel">This channels name and color will be the new <see cref="Attribute"/>.</param>
        public void AddAttribute(Channel channel)
        {
            Attributes.Add(new Attribute(channel.Name, channel.Color));
        }

        /// <summary>
        /// Removes an <see cref="Attribute"/> whose name is <paramref name="name"/> from <see cref="Attributes"/>.
        /// </summary>
        /// <param name="name"><see cref="Attribute"/>s name you want to delete.</param>
        public void RemoveAttribute(string name) => Attributes.Remove(GetAttribute(name));

        /// <summary>
        /// Finds an <see cref="Attribute"/> based on <paramref name="name"/>.
        /// </summary>
        /// <param name="name"><see cref="Attribute"/>s name whose wants to find.</param>
        /// <returns>An <see cref="Attribute"/> from <see cref="Attributes"/> whose name is <paramref name="name"/>.</returns>
        public Attribute GetAttribute(string name) => Attributes.Find(x => x.Name.Equals(name));
    }
}
