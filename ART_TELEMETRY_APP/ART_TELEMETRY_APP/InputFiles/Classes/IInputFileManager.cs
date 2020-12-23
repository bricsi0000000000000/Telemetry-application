using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    /// <summary>
    /// Contains the default things for an <see cref="InputFile"/> manager.
    /// </summary>
    /// <typeparam name="T">Can be any child of <see cref="InputFile"/>.</typeparam>
    public interface IInputFileManager <T>
    {
        /// <summary>
        /// List of <see cref="InputFile"/>s.
        /// </summary>
        List<T> InputFiles { get; }

        /// <summary>
        /// Adds an <see cref="InputFile"/>.
        /// </summary>
        /// <param name="inputFile"><see cref="InputFile"/> you want to add.</param>
        void AddInputFile(T inputFile);

        /// <summary>
        /// Removes an <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.
        /// </summary>
        /// <param name="inputFileName">Removable <see cref="InputFile"/>s name.</param>
        void RemoveInputFile(string inputFileName);

        /// <summary>
        /// Finds an <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.
        /// </summary>
        /// <param name="inputFileName">Findable <see cref="InputFile"/>s name.</param>
        /// <returns>Any <see cref="InputFile"/>.</returns>
        T GetInputFile(string inputFileName);

        /// <summary>
        /// Active <see cref="InputFile"/>s name.
        /// </summary>
        string ActiveInputFileName { get; set; }

        /// <summary>
        /// Returns the active <see cref="InputFile"/>.
        /// </summary>
        T GetActiveInputFile { get; }
    }
}
