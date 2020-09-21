using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    public static class InputFileManager
    {
        public static List<InputFile> InputFiles { get; private set; } = new List<InputFile>();

        public static void AddInputFile(InputFile inputFile) => InputFiles.Add(inputFile);

        public static void RemoveInputFile(InputFile inputFile) => InputFiles.Remove(inputFile);

        public static InputFile GetInputFile(string fileName) => InputFiles.Find(x => x.FileName.Equals(fileName));
    }
}
