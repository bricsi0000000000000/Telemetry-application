using DataLayer;
using DataLayer.Tracks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PresentationLayer.Tracks
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
        public static void LoadTracks(string folder)
        {
            foreach (var fileName in Directory.GetFiles(folder))
            {
                AddTrack(LoadTrack(fileName));
            }
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
        public static DriverlessTrack LoadTrack(string fileName)
        {
            var track = new DriverlessTrack();

            if (File.Exists(fileName))
            {
                if (new FileInfo(fileName).Length == 0)
                {
                    throw new Exception($"{fileName} is empty");
                }

                using var reader = new StreamReader(fileName);

                dynamic trackJSON = new object();

                try
                {
                    trackJSON = JsonConvert.DeserializeObject(reader.ReadToEnd());
                }
                catch (JsonReaderException)
                {
                    throw new Exception($"Can't deserialize {fileName}");
                }

                if (trackJSON.track == null)
                {
                    throw new Exception("Track is null");
                }

                if (trackJSON.track.name == null)
                {
                    throw new Exception("Track name is null");
                }

                if (trackJSON.track.width == null)
                {
                    throw new Exception("Track width is null");
                }

                if (trackJSON.track.length == null)
                {
                    throw new Exception("Track length is null");
                }

                if (trackJSON.track.leftSide == null)
                {
                    throw new Exception($"In track '{track.Name}' left side is empty!");
                }

                if (trackJSON.track.centerSide == null)
                {
                    throw new Exception($"In track '{track.Name}' center is empty!");
                }

                if (trackJSON.track.rightSide == null)
                {
                    throw new Exception($"In track '{track.Name}' right side is empty!");
                }

                if (trackJSON.track.name.ToString().Equals(string.Empty))
                {
                    throw new Exception("Track name is empty");
                }

                if (!float.TryParse(trackJSON.track.width.ToString(), out float width))
                {
                    throw new Exception($"In track '{track.Name}' couldn't convert '{trackJSON.track.width}' to a number");
                }

                if (!float.TryParse(trackJSON.track.length.ToString(), out float length))
                {
                    throw new Exception($"In track '{track.Name}' couldn't convert '{trackJSON.track.length}' to a number");
                }

                if (trackJSON.track.rightSide.Count == 0)
                {
                    throw new Exception("Track right side is empty");
                }

                if (trackJSON.track.leftSide.Count == 0)
                {
                    throw new Exception("Track left side is empty");
                }

                if (trackJSON.track.centerSide.Count == 0)
                {
                    throw new Exception("Track center is empty");
                }

                track.Name = trackJSON.track.name;
                track.Width = width;
                track.Length = length;

                for (int i = 0; i < trackJSON.track.rightSide.Count; i++)
                {
                    if (trackJSON.track.rightSide[i].x == null)
                    {
                        throw new Exception("Track right side x value is empty");
                    }

                    if (trackJSON.track.rightSide[i].y == null)
                    {
                        throw new Exception("Track right side y value is empty");
                    }

                    track.RightSide.Add(ParsePoint(trackJSON.track.rightSide[i].x.ToString(), trackJSON.track.rightSide[i].y.ToString(), ref track));
                }

                for (int i = 0; i < trackJSON.track.leftSide.Count; i++)
                {
                    if (trackJSON.track.leftSide[i].x == null)
                    {
                        throw new Exception("Track left side x value is empty");
                    }

                    if (trackJSON.track.leftSide[i].y == null)
                    {
                        throw new Exception("Track left side y value is empty");
                    }

                    track.LeftSide.Add(ParsePoint(trackJSON.track.leftSide[i].x.ToString(), trackJSON.track.leftSide[i].y.ToString(), ref track));
                }

                for (int i = 0; i < trackJSON.track.centerSide.Count; i++)
                {
                    if (trackJSON.track.leftSide[i].x == null)
                    {
                        throw new Exception("Track center x value is empty");
                    }

                    if (trackJSON.track.leftSide[i].y == null)
                    {
                        throw new Exception("Track center y value is empty");
                    }

                    track.Center.Add(ParsePoint(trackJSON.track.centerSide[i].x.ToString(), trackJSON.track.centerSide[i].y.ToString(), ref track));
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
        private static Point ParsePoint(string coordinateX, string coordinateY, ref DriverlessTrack track)
        {
            var point = new Point();
            if (double.TryParse(coordinateX, out double x))
            {
                point.X = x;
            }
            else
            {
                throw new Exception($"In track \"{track.Name}\" couldn't convert x coordinate \"{coordinateX}\" to a number");
            }

            if (double.TryParse(coordinateY, out double y))
            {
                point.Y = y;
            }
            else
            {
                throw new Exception($"In track \"{track.Name}\" couldn't convert y coordinate \"{coordinateY}\" to a number");
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
