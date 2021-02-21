using System.Collections.Generic;
using System.Linq;

namespace Telemetry_data_and_logic_layer.InputFiles
{
    /// <summary>
    /// Manages <see cref="InputFile"/>s.
    /// </summary>
    public static class InputFileManager
    {
        /// <summary>
        /// List of <see cref="InputFile"/>s.
        /// </summary>
        public static List<InputFile> InputFiles { get; private set; } = new List<InputFile>();

        /// <summary>
        /// Adds a <see cref="InputFile"/> to <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFile"></param>
        public static void AddInputFile(InputFile inputFile) => InputFiles.Add(inputFile);

        /// <summary>
        /// Finds a <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileName">Name of the findable <see cref="InputFile"/>.</param>
        /// <returns>An <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.</returns>
        public static InputFile GetInputFile(string inputFileName) => InputFiles.Find(x => x.Name.Equals(inputFileName));

        /// <summary>
        /// Finds a driverless <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileName">Name of the findable <see cref="InputFile"/>.</param>
        /// <returns>An <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.</returns>
        public static InputFile GetDriverlessInputFile(string inputFileName) => InputFiles.Find(x => x.Name.Equals(inputFileName) && x is DriverlessInputFile);

        /// <summary>
        /// Finds a driverless <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileID">ID of the findable <see cref="InputFile"/>.</param>
        /// <returns>An <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.</returns>
        public static InputFile GetDriverlessInputFile(int inputFileID) => InputFiles.Find(x => x.ID == inputFileID && x is DriverlessInputFile);

        /// <summary>
        /// Finds the last driverless <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <returns>The last driverless <see cref="InputFile"/>.</returns>
        public static InputFile GetLastDriverlessInputFile => InputFiles.FindLast(x => x is DriverlessInputFile);

        /// <summary>
        /// Finds a stadnard <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileName">Name of the findable <see cref="InputFile"/>.</param>
        /// <returns>An <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.</returns>
        public static InputFile GetStandardInputFile(string inputFileName) => InputFiles.Find(x => x.Name.Equals(inputFileName) && x is StandardInputFile);

        /// <summary>
        /// Removes a <see cref="InputFile"/> from <see cref="InputFiles"/> whose name is <paramref name="inputFileName"/>.
        /// </summary>
        /// <param name="inputFileName">Removabel <see cref="InputFile"/>s name.</param>
        public static void RemoveInputFile(string inputFileName) => InputFiles.Remove(GetInputFile(inputFileName));
        public static void RemoveInputFile(int id) => InputFiles.Remove(GetInputFile(id));

        /// <summary>
        /// Active <see cref="InputFile"/>s name.
        /// </summary>
        public static string ActiveInputFileName { get; set; }

        /// <summary>
        /// Active driverless <see cref="InputFile"/>s name.
        /// </summary>
        public static int ActiveDriverlessInputFileID { get; set; }

        /// <summary>
        /// Returns the active <see cref="InputFile"/>.
        /// </summary>
        public static InputFile GetActiveInputFile => GetInputFile(ActiveInputFileName);

        public static int DriverlessInputFilesCount => InputFiles.FindAll(x => x is DriverlessInputFile).Count;

        public static int LastID => InputFiles.Count == 0 ? -1 : InputFiles.Last().ID;

        public static InputFile GetInputFile(int id) => InputFiles.Find(x => x.ID == id);
    }
}
