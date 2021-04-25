using System;
using System.Collections.Generic;
using System.Text;
using DataLayer.Groups;

namespace DataLayer.InputFiles
{
    /// <summary>
    /// Represents an <see cref="InputFile"/>.
    /// </summary>
    public abstract class InputFile
    {
        public int ID { get; set; }

        /// <summary>
        /// <see cref="InputFile"/>s name.
        /// </summary>
        public string Name { get; set; }
        public string OriginalName { get; }

        public InputFileTypes InputFileType { get; set; } = InputFileTypes.standard;

        /// <summary>
        /// <see cref="Channel"/>s which are in this <see cref="InputFile"/>.
        /// </summary>
        public List<Channel> Channels { get; } = new List<Channel>();

        #region constructors
        public InputFile() { }

        public InputFile(int id, string name, List<Channel> channels)
        {
            ID = id;
            Name = name;
            OriginalName = name;
            Channels = channels;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driverlessInputFile">An <see cref="InputFile"/> which will be created the <see cref="InputFile"/>.</param>
        public InputFile(InputFile inputFile)
        {
            ID = inputFile.ID;
            Name = inputFile.Name;
            OriginalName = inputFile.Name;
            Channels = inputFile.Channels;
        }
        #endregion

        /// <summary>
        /// Finds a <see cref="Channel"/> whose name is <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="Channel"/>s name.</param>
        /// <returns>Returns a <see cref="Channel"/> whose name is <paramref name="name"/>.</returns>
        public Channel GetChannel(string name) => Channels.Find(x => x.Name.Equals(name));
        public Channel GetChannel(int id) => Channels.Find(x => x.ID == id);

        /// <summary>
        /// Contains the required <see cref="Channel"/>s for this <see cref="InputFile"/>.
        /// </summary>
        public Dictionary<string, bool> RequiredChannels { get; protected set; }
    }
}

