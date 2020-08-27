using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ART_TELEMETRY_APP.Maps.Classes
{
    static public class MapManager
    {
        public static List<Map> Maps { get; } = new List<Map>();

        public static void LoadMaps()
        {
            Maps.Clear();
            StreamReader sr = new StreamReader("maps.csv");
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] row = sr.ReadLine().Split(';');
                Maps.Add(new Map(row.First(), int.Parse(row.Last())));
                if (row[1].Equals("") && row[2].Equals(""))
                {
                    GetMap(row[0], int.Parse(row.Last())).StarPoint = new Point(-1, -1);
                }
                else
                {
                    GetMap(row[0], int.Parse(row.Last())).StarPoint = new Point(double.Parse(row[1]), double.Parse(row[2]));
                }
            }
            sr.Close();
        }

        private static void saveMaps()
        {
            StreamWriter sw = new StreamWriter("maps.csv");
            sw.WriteLine("map_name;start_point_x;start_point_y;year");
            foreach (Map map in Maps)
            {
                sw.WriteLine("{0};{1};{2};{3}", map.Name, map.StarPoint.X, map.StarPoint.Y, map.Year);
            }
            sw.Close();
        }

        public static void AddMap(string name, int year)
        {
            Maps.Add(new Map(name, year));
            saveMaps();
        }

        public static void DeleteMap(Map map)
        {
            Maps.Remove(Maps.Find(n => n.Name.Equals(map.Name) && n.Year.Equals(map.Year)));
            saveMaps();
        }

        public static Map GetMap(string name, int year) => Maps.Find(n => n.Name.Equals(name) && n.Year.Equals(year));

        public static Map GetMap(Map map) => Maps.Find(n => n.Name.Equals(map.Name) && n.Year.Equals(map.Year));

        public static void ChangeMap(Map map, string name, int year)
        {
            GetMap(map).Year = year;
            GetMap(map).Name = name;
        }

        public static void ChangeMapsStartPoint(Map map, Point star_point)
        {
            GetMap(map).StarPoint = star_point;
            saveMaps();
        }
    }
}
