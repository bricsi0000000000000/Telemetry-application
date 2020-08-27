using System.Collections.Generic;

namespace ART_TELEMETRY_APP.Pilots
{
    public class Driver
    {
        public List<InputFile> InputFiles { get; } = new List<InputFile>();
        public string Name { get; }

        public Driver(string name)
        {
            Name = name;
        }

        public void AddInputFile(InputFile file) => InputFiles.Add(file);

        public InputFile GetInputFile(string file_name) => InputFiles.Find(name => name.FileName.Equals(file_name));

        public void RemoveInputFile(string file_name) => InputFiles.Remove(InputFiles.Find(name => name.FileName.Contains(file_name)));
    }
}
