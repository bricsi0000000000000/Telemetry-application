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
    /// This class represents one input files data.
    /// </summary>
    public class InputFile
    {
        /// <summary>
        /// File name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <see cref="Channel"/>s those are in this <see cref="InputFile"/>.
        /// </summary>
        public List<Channel> Channels { get; } = new List<Channel>();

        public InputFile(string name, List<Channel> channels)
        {
            Name = name;
            Channels = channels;
        }
    }
}
