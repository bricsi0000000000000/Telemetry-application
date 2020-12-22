using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    public class DriverlessInputFileManager : IInputFileManager<DriverlessInputFile>
    {
        #region instance
        private static DriverlessInputFileManager instance = null;
        private DriverlessInputFileManager() { }
        public static DriverlessInputFileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DriverlessInputFileManager();
                }
                return instance;
            }
        }
        #endregion

        public List<DriverlessInputFile> InputFiles { get; private set; } = new List<DriverlessInputFile>();

        public void AddInputFile(DriverlessInputFile inputFile) => InputFiles.Add(inputFile);

        public DriverlessInputFile GetInputFile(string inputFileName) => InputFiles.Find(x => x.Name.Equals(inputFileName));

        public void RemoveInputFile(string inputFileName) => InputFiles.Remove(GetInputFile(inputFileName));
    }
}
