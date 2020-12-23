using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    /// <summary>
    /// Manages <see cref="StandardInputFile"/>s.
    /// </summary>
    public class StandardInputFileManager : IInputFileManager<StandardInputFile>
    {
        #region instance
        private static StandardInputFileManager instance = null;
        private StandardInputFileManager() { }
        public static StandardInputFileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StandardInputFileManager();
                }
                return instance;
            }
        }
        #endregion

        /// <summary>
        /// List of <see cref="StandardInputFile"/>s.
        /// </summary>
        public List<StandardInputFile> InputFiles { get; private set; } = new List<StandardInputFile>();

        /// <summary>
        /// Adds a <see cref="StandardInputFile"/> to <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFile"></param>
        public void AddInputFile(StandardInputFile inputFile) => InputFiles.Add(inputFile);

        /// <summary>
        /// Finds a <see cref="StandardInputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileName">Name ofthe findable <see cref="StandardInputFile"/>.</param>
        /// <returns>A <see cref="StandardInputFile"/> from <see cref="InputFiles"/> whose name is <paramref name="inputFileName"/>.</returns>
        public StandardInputFile GetInputFile(string inputFileName) => InputFiles.Find(x => x.Name.Equals(inputFileName));

        /// <summary>
        /// Removes a <see cref="StandardInputFile"/> from <see cref="InputFiles"/> whose name is <paramref name="inputFileName"/>.
        /// </summary>
        /// <param name="inputFileName">Removabel <see cref="StandardInputFile"/>s name.</param>
        public void RemoveInputFile(string inputFileName) => InputFiles.Remove(GetInputFile(inputFileName));

        /// <summary>
        /// Active <see cref="StandardInputFile"/>s name.
        /// </summary>
        public string ActiveInputFileName { get; set; }

        /// <summary>
        /// Returns the active <see cref="StandardInputFile"/>.
        /// </summary>
        public StandardInputFile GetActiveInputFile => GetInputFile(ActiveInputFileName);

    }
}
