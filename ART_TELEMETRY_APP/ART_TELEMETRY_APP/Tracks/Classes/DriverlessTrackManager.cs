using ART_TELEMETRY_APP.Errors.Classes;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace ART_TELEMETRY_APP.Tracks.Classes
{
    /// <summary>
    /// Manages the driverless tracks
    /// </summary>
    static class DriverlessTrackManager
    {
        /// <summary>
        /// Stores the <see cref="DriverlessTrack"/>-s.
        /// </summary>
        public static List<DriverlessTrack> DriverlessTracks { get; private set; } = new List<DriverlessTrack>();


        /// <summary>
        /// Loads all tracks from files
        /// </summary>
        /// <param name="errorSnackbar"><see cref="Snackbar"/> that shows erro message.</param>
        public static void LoadTracks(ref Snackbar errorSnackbar)
        {
            AddTrack(LoadTrack("straight_track.xml", ref errorSnackbar), ref errorSnackbar);
        }

        /// <summary>
        /// Add <see cref="DriverlessTrack"/> to <see cref="DriverlessTracks"/>.
        /// </summary>
        /// <param name="track">Loaded <see cref="DriverlessTracks"/>.</param>
        /// <param name="errorSnackbar"><see cref="Snackbar"/> that shows erro message.</param>
        private static void AddTrack(DriverlessTrack track, ref Snackbar errorSnackbar)
        {
            if (CheckTrack(ref track))
            {
                DriverlessTracks.Add(track);
            }
            else
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, "Straight track can't be loaded because of something is missing.", time: 6);
            }
        }

        /// <summary>
        /// Load a <see cref="DriverlessTrack"/> from file.
        /// </summary>
        /// <param name="fileName">The xml file name that contains the track data.</param>
        /// <param name="errorSnackbar"><see cref="Snackbar"/> that shows erro message.</param>
        /// <returns>Readed <see cref="DriverlessTrack"/>.</returns>
        private static DriverlessTrack LoadTrack(string fileName, ref Snackbar errorSnackbar)
        {
            DriverlessTrack track = new DriverlessTrack();

            using var reader = XmlReader.Create(fileName);

            bool parsingError = false;

            while (reader.Read() && !parsingError)
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "name":
                            string inputName = reader.ReadElementContentAsString();
                            if (!inputName.Equals(string.Empty))
                            {
                                track.Name = inputName;
                            }
                            else
                            {
                                parsingError = true;
                                ShowError.ShowErrorMessage(ref errorSnackbar, $"Track name is empty", time: 6);
                            }
                            break;
                        case "width":
                            float width;
                            string inputWidth = reader.ReadElementContentAsString();
                            if (float.TryParse(inputWidth, out width))
                            {
                                track.Width = width;
                            }
                            else
                            {
                                parsingError = true;
                                ShowError.ShowErrorMessage(ref errorSnackbar, $"In track \"{track.Name}\" couldn't convert \"{inputWidth}\" to a number.", time: 6);
                            }
                            break;
                        case "length":
                            float length;
                            string inputLength = reader.ReadElementContentAsString();
                            if (float.TryParse(inputLength, out length))
                            {
                                track.Length = length;
                            }
                            else
                            {
                                parsingError = true;
                                ShowError.ShowErrorMessage(ref errorSnackbar, $"In track \"{track.Name}\" couldn't convert \"{inputLength}\" to a number.", time: 6);
                            }
                            break;
                        case "leftSide":
                            XmlReader leftSideReader = reader.ReadSubtree();
                            while (leftSideReader.Read())
                            {
                                track.LeftSide.Add(ParsePoint(leftSideReader.Value.Split(';'), ref parsingError, ref track, ref errorSnackbar));
                            }
                            break;
                        case "rightSide":
                            XmlReader rightSideReader = reader.ReadSubtree();
                            while (rightSideReader.Read())
                            {
                                track.RightSide.Add(ParsePoint(rightSideReader.Value.Split(';'), ref parsingError, ref track, ref errorSnackbar));
                            }
                            break;
                        case "center":
                            XmlReader centerSideReader = reader.ReadSubtree();
                            while (centerSideReader.Read())
                            {
                                track.Center.Add(ParsePoint(centerSideReader.Value.Split(';'), ref parsingError, ref track, ref errorSnackbar));
                            }
                            break;
                    }
                }
            }

            return track;
        }

        /// <summary>
        /// Checks if the <paramref name="track"/> is valid, so every property is readed.
        /// </summary>
        /// <param name="track">The <see cref="DriverlessTrack"/> that you want to check.</param>
        /// <returns>True if every parameter is readed and false if at least one of them is missing.</returns>
        private static bool CheckTrack(ref DriverlessTrack track)
        {
            return !track.Name.Equals(string.Empty) &&
                    track.Width > 0 &&
                    track.Length > 0 &&
                    track.RightSide.Count > 0 &&
                    track.LeftSide.Count > 0 &&
                    track.Center.Count > 0;
        }

        /// <summary>
        /// Parses a <see cref="Point"/> from the readed point row <i><point>0;1</point></i>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="parsingError"></param>
        /// <param name="track"></param>
        /// <param name="errorSnackbar"></param>
        /// <returns></returns>
        private static Point ParsePoint(string[] row, ref bool parsingError, ref DriverlessTrack track, ref Snackbar errorSnackbar)
        {
            Point point = new Point();
            if (row.Length >= 2)
            {
                if (double.TryParse(row[0], out double x))
                {
                    point.X = x;
                }
                else
                {
                    ShowError.ShowErrorMessage(ref errorSnackbar, $"In track \"{track.Name}\" couldn't convert x coordinate \"{row[0]}\" to a number.", time: 6);
                    parsingError = true;
                }

                double y;
                if (double.TryParse(row[1], out y))
                {
                    point.Y = y;
                }
                else
                {
                    ShowError.ShowErrorMessage(ref errorSnackbar, $"In track \"{track.Name}\" couldn't convert y coordinate \"{row[1]}\" to a number.", time: 6);
                    parsingError = true;
                }
            }

            return point;
        }
    }
}
