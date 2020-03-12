using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Pilots
{
    public class Pilot
    {
        string name;
        InputFile input_file;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public InputFile Inputfile
        {
            get
            {
                return input_file;
            }
            set
            {
                input_file = value;
            }
        }

        public Pilot(string name)
        {
            this.name = name;
        }
    }
}
