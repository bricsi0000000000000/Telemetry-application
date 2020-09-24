using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Groups.Classes
{
    /// <summary>
    /// Represents one group
    /// </summary>
    public class Group
    {
        public string Name { get; }
        public List<string> Attributes { get; } = new List<string>();

        public Group(string name)
        {
            Name = name;
        }

        public void AddAttribute(string attribute)
        {
            if (attribute.Equals(string.Empty))
            {
                return;
            }
            Attributes.Add(attribute);
        }

        public void RemoveAttribute(string attribute) => Attributes.Remove(attribute);
    }
}
