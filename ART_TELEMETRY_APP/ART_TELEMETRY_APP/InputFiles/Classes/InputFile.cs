using ART_TELEMETRY_APP.Charts.Classes;
using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    /// <summary>
    /// Represents an <see cref="InputFile"/>.
    /// </summary>
    public class InputFile
    {
        /// <summary>
        /// <see cref="InputFile"/>s name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Decides that this <see cref="InputFile"/> is driverless or not.
        /// </summary>
        public bool Driverless { get; set; } = true;

        /// <summary>
        /// <see cref="Channel"/>s which are in this <see cref="InputFile"/>.
        /// </summary>
        public List<Channel> Channels { get; } = new List<Channel>();

        #region constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public InputFile() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"><see cref="InputFile"/>s name.</param>
        /// <param name="channels"><see cref="InputFile"/>s channels</param>
        public InputFile(string name, List<Channel> channels)
        {
            Name = name;
            Channels = channels;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driverlessInputFile">An <see cref="InputFile"/> which will be created the <see cref="InputFile"/>.</param>
        public InputFile(InputFile inputFile)
        {
            Name = inputFile.Name;
            Channels = inputFile.Channels;
        }
        #endregion

        /// <summary>
        /// Finds a <see cref="Channel"/> whose name is <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="Channel"/>s name.</param>
        /// <returns>Returns a <see cref="Channel"/> whose name is <paramref name="name"/>.</returns>
        public Channel GetChannel(string name) => Channels.Find(x => x.Name.Equals(name));

        /// <summary>
        /// Contains the required <see cref="Channel"/>s for this <see cref="InputFile"/>.
        /// </summary>
        public Dictionary<string, bool> RequiredChannels { get; protected set; }

        /// <summary>
        /// Checks if the required <see cref="Channel"/> whose name is <paramref name="name"/> is satisfied.
        /// </summary>
        /// <param name="name">Name of the <see cref="Channel"/>.</param>
        /// <returns>True if the <see cref="Channel"/> is already satisfied and false if not.</returns>
        public bool IsRequiredChannelSatisfied(string name)
        {
            try
            {
                return RequiredChannels[name];
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Changes the value of the required channel based on <paramref name="satisfaction"/>.
        /// </summary>
        /// <param name="name"><see cref="Channel"/>s name.</param>
        /// <param name="satisfaction">True if satisfied and false if not.</param>
        public void ChangeRequiredChannelSatisfaction(string name, bool satisfaction)
        {
            RequiredChannels[name] = satisfaction;
        }
    }
}
