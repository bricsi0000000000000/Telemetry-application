using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Telemetry_data_and_logic_layer.Tracks
{
    /// <summary>
    /// Manages driverless tracks.
    /// </summary>
    public static class DriverlessTrackManager
    {
        /// <summary>
        /// Stores <see cref="DriverlessTrack"/>s.
        /// </summary>
        public static List<DriverlessTrack> DriverlessTracks { get; private set; } = new List<DriverlessTrack>();

        /// <summary>
        /// Loads all tracks from files.
        /// Should be called in <see cref="MainWindow"/>.
        /// </summary>
        /// <param name="errorSnackbar"><see cref="Snackbar"/> that shows erro message.</param>
        public static void LoadTracks()
        {
            AddTrack(LoadTrack("straight_track.json"));
        }

        /// <summary>
        /// Add <see cref="DriverlessTrack"/> to <see cref="DriverlessTracks"/>.
        /// </summary>
        /// <param name="track">Loaded <see cref="DriverlessTracks"/>.</param>
        /// <param name="errorSnackbar"><see cref="Snackbar"/> that shows erro message.</param>
        private static void AddTrack(DriverlessTrack track)
        {
            if (CheckTrack(ref track))
            {
                DriverlessTracks.Add(track);
            }
            else
            {
                throw new Exception("Straight track can't be loaded because something is missing.");
            }
        }

        /// <summary>
        /// Loads a <see cref="DriverlessTrack"/> from file.
        /// </summary>
        /// <param name="fileName">The JSON file name that contains the track data.</param>
        /// <param name="errorSnackbar"><see cref="Snackbar"/> that shows erro message.</param>
        /// <returns>Readed <see cref="DriverlessTrack"/>.</returns>
        private static DriverlessTrack LoadTrack(string fileName)
        {
            var track = new DriverlessTrack();

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
                    throw new Exception("Track name is empty");
                }

                if (float.TryParse(trackJSON.track.width.ToString(), out float width))
                {
                    track.Width = width;
                }
                else
                {
                    parsingError = true;
                    throw new Exception($"In track '{track.Name}' couldn't convert '{trackJSON.track.width}' to a number.");
                }

                if (float.TryParse(trackJSON.track.length.ToString(), out float length))
                {
                    track.Length = length;
                }
                else
                {
                    parsingError = true;
                    throw new Exception($"In track '{track.Name}' couldn't convert '{trackJSON.track.length}' to a number.");
                }

                for (int i = 0; i < trackJSON.track.rightSide.Count; i++)
                {
                    track.RightSide.Add(ParsePoint(trackJSON.track.rightSide[i].x.ToString(), trackJSON.track.rightSide[i].y.ToString(), ref parsingError, ref track));
                }

                for (int i = 0; i < trackJSON.track.leftSide.Count; i++)
                {
                    track.LeftSide.Add(ParsePoint(trackJSON.track.leftSide[i].x.ToString(), trackJSON.track.leftSide[i].y.ToString(), ref parsingError, ref track));
                }

                for (int i = 0; i < trackJSON.track.centerSide.Count; i++)
                {
                    track.Center.Add(ParsePoint(trackJSON.track.centerSide[i].x.ToString(), trackJSON.track.centerSide[i].y.ToString(), ref parsingError, ref track));
                }
            }
            else
            {
                throw new Exception($"Can't find '{fileName}'");
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
        private static Point ParsePoint(string coordinateX, string coordinateY, ref bool parsingError, ref DriverlessTrack track)
        {
            Point point = new Point();
            if (double.TryParse(coordinateX, out double x))
            {
                point.X = x;
            }
            else
            {
                parsingError = true;
                throw new Exception($"In track \"{track.Name}\" couldn't convert x coordinate \"{coordinateX}\" to a number.");
            }

            if (double.TryParse(coordinateY, out double y))
            {
                point.Y = y;
            }
            else
            {
                parsingError = true;
                throw new Exception($"In track \"{track.Name}\" couldn't convert y coordinate \"{coordinateY}\" to a number.");
            }

            return point;
        }

        /// <summary>
        /// Finds a <see cref="DriverlessTrack"/> in <see cref="DriverlessTracks"/> based on <paramref name="trackName"/>.
        /// </summary>
        /// <param name="trackName">The name of the track to be found.</param>
        /// <returns>A <see cref="DriverlessTrack"/>.</returns>
        public static DriverlessTrack GetTrack(string trackName) => DriverlessTracks.Find(x => x.Name.Equals(trackName));
    }
}
