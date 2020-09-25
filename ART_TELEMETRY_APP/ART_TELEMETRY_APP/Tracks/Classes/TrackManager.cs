using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ART_TELEMETRY_APP
{
    static public class TrackManager
    {
        public static List<Track> Tracks { get; } = new List<Track>();

        public static void LoadTracks(ref Snackbar errorSnackbar)
        {
            ReadTracks(ref errorSnackbar);
        }

        private static void ReadTracks(ref Snackbar errorSnackbar)
        {
            if (File.Exists(TextManager.TracksFileName))
            {
                TrackReader();
            }
            else
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, string.Format("Couldn't load tracks, because '{0}' file not found!", TextManager.TracksFileName), 3);
            }
        }

        private static void TrackReader()
        {
            Tracks.Clear();

            using var reader = new StreamReader(TextManager.TracksFileName);
            while (!reader.EndOfStream)
            {
                string[] row = reader.ReadLine().Split(';');

                if (row.Length != 4)
                    continue;

                Tracks.Add(new Track(row.First(), row.Last()));
                if (row[1].Equals(string.Empty) && row[2].Equals(string.Empty))
                {
                    GetTrack(row[0], row.Last()).StarPoint = new Point(-1, -1);
                }
                else
                {
                    GetTrack(row[0], row.Last()).StarPoint = new Point(double.Parse(row[1]), double.Parse(row[2]));
                }
            }
        }

        public static List<Tuple<string, string>> TrackNamesAndDesctiptions
        {
            get
            {
                var namesAndYears = new List<Tuple<string, string>>();
                foreach (var track in Tracks)
                {
                    namesAndYears.Add(new Tuple<string, string>(track.Name, track.Description));
                }
                return namesAndYears;
            }
        }

        public static void SaveTracks()
        {
            using var writer = new StreamWriter(TextManager.TracksFileName);
            foreach (Track track in Tracks)
            {
                writer.WriteLine("{0};{1};{2};{3}", track.Name, track.StarPoint.X, track.StarPoint.Y, track.Description);
            }
        }

        public static void AddTrack(string name, string description)
        {
            Tracks.Add(new Track(name, description));
            SaveTracks();
        }

        public static void DeleteTrack(Track track)
        {
            Tracks.Remove(Tracks.Find(n => n.Name.Equals(track.Name) && n.Description.Equals(track.Description)));
            SaveTracks();
        }

        public static Track GetTrack(string trackName, string description) =>
              Tracks.Find(x => x.Name.Equals(trackName) &&
                          x.Description.Equals(description));

        public static Track GetTrack(string trackName, string description, string fileName, string driverName) => 
               Tracks.Find(x => x.Name.Equals(trackName) && 
                           x.Description.Equals(description) &&
                           x.InputFileFileName.Equals(InputFileManager.GetInputFile(fileName, driverName)));

        public static Track GetTrack(Track track) => 
               Tracks.Find(x => x.Name.Equals(track.Name) && 
                           x.Description.Equals(track.Description) &&
                           x.InputFileFileName.Equals(track.InputFileFileName));

        public static void ChangeTrack(Track track, string name, string description)
        {
            GetTrack(track).Description = description;
            GetTrack(track).Name = name;
            SaveTracks();
        }

        public static void ChangeTrackStartPoint(Track track, Point starPoint)
        {
            GetTrack(track).StarPoint = starPoint;
            SaveTracks();
        }
    }
}
