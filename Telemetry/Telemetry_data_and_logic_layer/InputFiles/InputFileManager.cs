using System.Collections.Generic;

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

        /// <summary>
        /// Active <see cref="InputFile"/>s name.
        /// </summary>
        public static string ActiveInputFileName { get; set; }

        /// <summary>
        /// Returns the active <see cref="InputFile"/>.
        /// </summary>
        public static InputFile GetActiveInputFile => GetInputFile(ActiveInputFileName);
    }
}
