using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Groups.Classes
{
    public class Group
    {
        string name;
        List<string> attributes = new List<string>();

        public Group(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public void AddAttribute(string attribute)
        {
            attributes.Add(attribute);
        }

        public void RemoveAttribute(string attribute)
        {
            attributes.Remove(attribute);
        }

        public List<string> Attributes
        {
            get
            {
                return attributes;
            }
        }
    }
}
