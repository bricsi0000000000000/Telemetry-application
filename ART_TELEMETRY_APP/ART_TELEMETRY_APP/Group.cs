using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class Group
    {
        string name;
        List<string> attributes = new List<string>();

        public Group(string name)
        {
            this.name = name;
        }

        public void AddAttribute(string attribute)
        {
            attributes.Add(attribute);
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public List<string> Attributes
        {
            get
            {
                return this.attributes;
            }
        }
    }
}
