using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            try
            {
                TrackReader();
            }
            catch (FileNotFoundException)
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, string.Format("Couldn't load tracks, because '{0}' file not found!", TextManager.TracksCSV), 3);
            }
        }

        private static void TrackReader()
        {
            Tracks.Clear();
            using var reader = new StreamReader(TextManager.TracksCSV);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string[] row = reader.ReadLine().Split(';');
                Tracks.Add(new Track(row.First(), int.Parse(row.Last())));
                if (row[1].Equals(string.Empty) && row[2].Equals(string.Empty))
                {
                    GetTrack(row[0], int.Parse(row.Last())).StarPoint = new Point(-1, -1);
                }
                else
                {
                    GetTrack(row[0], int.Parse(row.Last())).StarPoint = new Point(double.Parse(row[1]), double.Parse(row[2]));
                }
            }
        }

        public static List<Tuple<string, int>> TrackNamesAndYears
        {
            get
            {
                var namesAndYears = new List<Tuple<string, int>>();
                foreach (var track in Tracks)
                {
                    namesAndYears.Add(new Tuple<string, int>(track.Name, track.Year));
                }
                return namesAndYears;
            }
        }

        private static void saveMaps()
        {
            StreamWriter sw = new StreamWriter("maps.csv");
            sw.WriteLine("map_name;start_point_x;start_point_y;year");
            foreach (Track map in Tracks)
            {
                sw.WriteLine("{0};{1};{2};{3}", map.Name, map.StarPoint.X, map.StarPoint.Y, map.Year);
            }
            sw.Close();
        }

        public static void AddMap(string name, int year)
        {
            Tracks.Add(new Track(name, year));
            saveMaps();
        }

        public static void DeleteMap(Track map)
        {
            Tracks.Remove(Tracks.Find(n => n.Name.Equals(map.Name) && n.Year.Equals(map.Year)));
            saveMaps();
        }

        public static Track GetTrack(string name, int year) => Tracks.Find(x => x.Name.Equals(name) && x.Year.Equals(year));

        public static Track GetTrack(Track map) => Tracks.Find(n => n.Name.Equals(map.Name) && n.Year.Equals(map.Year));

        public static void ChangeMap(Track map, string name, int year)
        {
            GetTrack(map).Year = year;
            GetTrack(map).Name = name;
        }

        public static void ChangeMapsStartPoint(Track map, Point star_point)
        {
            GetTrack(map).StarPoint = star_point;
            saveMaps();
        }
    }
}
