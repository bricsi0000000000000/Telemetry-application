using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Groups.Classes
{
    public class Attribute
    {
        public string Name { get; private set; }
        public Color Color { get; set; }

        public Attribute(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
