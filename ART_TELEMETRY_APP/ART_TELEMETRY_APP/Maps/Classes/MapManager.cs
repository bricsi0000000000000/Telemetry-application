using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ART_TELEMETRY_APP.Maps.Classes
{
    static public class MapManager
    {
        static List<Map> maps = new List<Map>();

        public static List<Map> Maps
        {
            get
            {
                return maps;
            }
        }

        public static void LoadMaps()
        {
            maps.Clear();
            StreamReader sr = new StreamReader("maps.csv");
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] row = sr.ReadLine().Split(';');
                maps.Add(new Map(row.First(), row.Last()));
                if (row[1].Equals("") && row[2].Equals(""))
                {
                    GetMap(row[0]).StarPoint = new Point(-1, -1);
                }
                else
                {
                    GetMap(row[0]).StarPoint = new Point(double.Parse(row[1]), double.Parse(row[2]));
                }
            }
            sr.Close();
        }

        private static void saveMaps()
        {
            StreamWriter sw = new StreamWriter("maps.csv");
            sw.WriteLine("map_name;start_point_x;start_point_y;year");
            foreach (Map map in maps)
            {
                sw.WriteLine("{0};{1};{2};{3}", map.Name, map.StarPoint.X, map.StarPoint.Y, map.Year);
            }
            sw.Close();
        }

        public static void AddMap(string name, string year)
        {
            maps.Add(new Map(name, year));
            saveMaps();
        }

        public static void DeleteMap(string name)
        {
            maps.Remove(maps.Find(n => n.Name == name));
            saveMaps();
        }

        public static Map GetMap(string name)
        {
            return maps.Find(n => n.Name == name);
        }

        public static void ChangeMapsStartPoint(string map_name, Point star_point)
        {
            GetMap(map_name).StarPoint = star_point;
            saveMaps();
        }
    }
}
