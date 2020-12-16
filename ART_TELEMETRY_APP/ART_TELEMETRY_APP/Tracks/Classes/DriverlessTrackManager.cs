using ART_TELEMETRY_APP.Errors.Classes;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

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
            AddTrack(LoadTrack("straight_track.json", ref errorSnackbar), ref errorSnackbar);
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
        /// <param name="fileName">The JSON file name that contains the track data.</param>
        /// <param name="errorSnackbar"><see cref="Snackbar"/> that shows erro message.</param>
        /// <returns>Readed <see cref="DriverlessTrack"/>.</returns>
        private static DriverlessTrack LoadTrack(string fileName, ref Snackbar errorSnackbar)
        {
            DriverlessTrack track = new DriverlessTrack();

            if (File.Exists(fileName))
            {
                using var reader = new StreamReader(fileName);

                dynamic trackJSON = JsonConvert.DeserializeObject(reader.ReadToEnd());

                bool parsingError = false;

                if (!trackJSON.track.name.Equals(string.Empty))
                {
                    track.Name = trackJSON.track.name;
                }
                else
                {
                    parsingError = true;
                    ShowError.ShowErrorMessage(ref errorSnackbar, $"Track name is empty", time: 6);
                }

                if (float.TryParse(trackJSON.track.width.ToString(), out float width))
                {
                    track.Width = width;
                }
                else
                {
                    parsingError = true;
                    ShowError.ShowErrorMessage(ref errorSnackbar, $"In track \"{track.Name}\" couldn't convert \"{trackJSON.track.width}\" to a number.", time: 6);
                }

                if (float.TryParse(trackJSON.track.length.ToString(), out float length))
                {
                    track.Length = length;
                }
                else
                {
                    parsingError = true;
                    ShowError.ShowErrorMessage(ref errorSnackbar, $"In track \"{track.Name}\" couldn't convert \"{trackJSON.track.length}\" to a number.", time: 6);
                }

                for (int i = 0; i < trackJSON.track.rightSide.Count; i++)
                {
                    track.RightSide.Add(ParsePoint(trackJSON.track.rightSide[i].x.ToString(), trackJSON.track.rightSide[i].y.ToString(), ref parsingError, ref track, ref errorSnackbar));
                }

                for (int i = 0; i < trackJSON.track.leftSide.Count; i++)
                {
                    track.LeftSide.Add(ParsePoint(trackJSON.track.leftSide[i].x.ToString(), trackJSON.track.leftSide[i].y.ToString(), ref parsingError, ref track, ref errorSnackbar));
                }

                for (int i = 0; i < trackJSON.track.centerSide.Count; i++)
                {
                    track.Center.Add(ParsePoint(trackJSON.track.centerSide[i].x.ToString(), trackJSON.track.centerSide[i].y.ToString(), ref parsingError, ref track, ref errorSnackbar));
                }
            }
            else
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, $"Can't find '{fileName}'", time: 6);
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
        /// Parses a <see cref="Point"/> from the readed <b>x</b> and <b>y</b> coordinate.
        /// </summary>
        /// <param name="coordinateX">Readed <b>x</b> coordinate in string</param>
        /// <param name="coordinateY">Readed <b>y</b> coordinate in string</param>
        /// <param name="parsingError"></param>
        /// <param name="track"></param>
        /// <param name="errorSnackbar"></param>
        /// <returns>A <see cref="Point"/>.</returns>
        private static Point ParsePoint(string coordinateX, string coordinateY, ref bool parsingError, ref DriverlessTrack track, ref Snackbar errorSnackbar)
        {
            Point point = new Point();
            if (double.TryParse(coordinateX, out double x))
            {
                point.X = x;
            }
            else
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, $"In track \"{track.Name}\" couldn't convert x coordinate \"{coordinateX}\" to a number.", time: 6);
                parsingError = true;
            }

            if (double.TryParse(coordinateY, out double y))
            {
                point.Y = y;
            }
            else
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, $"In track \"{track.Name}\" couldn't convert y coordinate \"{coordinateY}\" to a number.", time: 6);
                parsingError = true;
            }

            return point;
        }
    }
}
