using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    /// <summary>
    /// Manages <see cref="DriverlessInputFile"/>s.
    /// </summary>
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

        /// <summary>
        /// List of <see cref="DriverlessInputFile"/>s.
        /// </summary>
        public List<DriverlessInputFile> InputFiles { get; private set; } = new List<DriverlessInputFile>();

        /// <summary>
        /// Adds a <see cref="DriverlessInputFile"/> to <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFile"></param>
        public void AddInputFile(DriverlessInputFile inputFile) => InputFiles.Add(inputFile);

        /// <summary>
        /// Finds a <see cref="DriverlessInputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileName">Name ofthe findable <see cref="DriverlessInputFile"/>.</param>
        /// <returns>A <see cref="DriverlessInputFile"/> from <see cref="InputFiles"/> whose name is <paramref name="inputFileName"/>.</returns>
        public DriverlessInputFile GetInputFile(string inputFileName) => InputFiles.Find(x => x.Name.Equals(inputFileName));

        /// <summary>
        /// Removes a <see cref="DriverlessInputFile"/> from <see cref="InputFiles"/> whose name is <paramref name="inputFileName"/>.
        /// </summary>
        /// <param name="inputFileName">Removabel <see cref="DriverlessInputFile"/>s name.</param>
        public void RemoveInputFile(string inputFileName) => InputFiles.Remove(GetInputFile(inputFileName));

        /// <summary>
        /// Active <see cref="DriverlessInputFile"/>s name.
        /// </summary>
        public string ActiveInputFileName { get; set; }

        /// <summary>
        /// Returns the active <see cref="DriverlessInputFile"/>.
        /// </summary>
        public DriverlessInputFile GetActiveInputFile => GetInputFile(ActiveInputFileName);
    }
}
