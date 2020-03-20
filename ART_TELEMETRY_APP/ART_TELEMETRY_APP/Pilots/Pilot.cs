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
        List<InputFile> input_files = new List<InputFile>();

        public string Name
        {
            get
            {
                return name;
            }
        }

        public void AddInputFile(InputFile file)
        {
            input_files.Add(file);
        }

        public InputFile GetInputFile(string file_name)
        {
            return input_files.Find(name => name.FileName == file_name);
        }

        public Pilot(string name)
        {
            this.name = name;
        }

        public List<InputFile> InputFiles
        {
            get
            {
                return input_files;
            }
        }
    }
}
