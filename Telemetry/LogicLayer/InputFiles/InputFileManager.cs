using DataLayer.Groups;
using DataLayer.InputFiles;
using LogicLayer.Colors;
using System.Collections.Generic;
using System.Linq;

namespace PresentationLayer.InputFiles
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
        public static void Add(InputFile inputFile) => InputFiles.Add(inputFile);

        /// <summary>
        /// Finds a <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="fileName">Name of the findable <see cref="InputFile"/>.</param>
        /// <returns>An <see cref="InputFile"/> whose name is <paramref name="fileName"/>.</returns>
        public static InputFile Get(string fileName) => InputFiles.Find(x => x.Name.Equals(fileName));

        public static InputFile Get(int id) => InputFiles.Find(x => x.ID == id);

        /// <summary>
        /// Finds a driverless <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileName">Name of the findable <see cref="InputFile"/>.</param>
        /// <returns>An <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.</returns>
        public static InputFile GetDriverlessFile(string inputFileName) => InputFiles.Find(x => x.Name.Equals(inputFileName) && x is DriverlessInputFile);

        /// <summary>
        /// Finds a driverless <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileID">ID of the findable <see cref="InputFile"/>.</param>
        /// <returns>An <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.</returns>
        public static InputFile GetDriverlessFile(int inputFileID) => InputFiles.Find(x => x.ID == inputFileID && x is DriverlessInputFile);

        /// <summary>
        /// Finds the last driverless <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <returns>The last driverless <see cref="InputFile"/>.</returns>
        public static InputFile GetLastDriverlessFile => InputFiles.FindLast(x => x is DriverlessInputFile);

        /// <summary>
        /// Finds a stadnard <see cref="InputFile"/> in <see cref="InputFiles"/>.
        /// </summary>
        /// <param name="inputFileName">Name of the findable <see cref="InputFile"/>.</param>
        /// <returns>An <see cref="InputFile"/> whose name is <paramref name="inputFileName"/>.</returns>
        public static InputFile GetStandardFile(string inputFileName) => InputFiles.Find(x => x.Name.Equals(inputFileName) && x is StandardInputFile);

        /// <summary>
        /// Removes a <see cref="InputFile"/> from <see cref="InputFiles"/> whose name is <paramref name="inputFileName"/>.
        /// </summary>
        /// <param name="inputFileName">Removabel <see cref="InputFile"/>s name.</param>
        public static void Remove(string inputFileName) => InputFiles.Remove(Get(inputFileName));
        public static void Remove(int id) => InputFiles.Remove(Get(id));

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
        public static InputFile GetActiveInputFile => Get(ActiveInputFileName);

        public static bool IsAnyDriverlessFile => InputFiles.FindAll(x => x is DriverlessInputFile).Any();

        public static int LastID => InputFiles.Any() ? InputFiles.Last().ID : -1;

        public static bool HasInputFile(string originalName) => InputFiles.Find(x => x.OriginalName.Equals(originalName)) != null;

        /// <summary>
        /// Creates a live input file and saves it
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sensorNames"></param>
        public static void AddLive(string name, List<string> sensorNames)
        {
            if (Get(name) == null)
            {
                var channels = new List<Channel>();
                for (int i = 0; i < sensorNames.Count; i++)
                {
                    channels.Add(new Channel(i, sensorNames[i], ColorManager.GetChartColor.ToString()));
                }
                Add(new LiveInputFile(LastID + 1, name, channels));
            }
        }

        public static LiveInputFile GetLiveFile(string name) => (LiveInputFile)InputFiles.Find(x => x.Name.Equals(name) && x.InputFileType == InputFileTypes.live);
    }
}
