﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class Map
    {
        List<string> svg_pathes = new List<string>();
        string name;

        public Map(string name)
        {
            this.name = name;

            calculateLaps();

            for (int lap = 0; lap < Datas.Instance.GetData().Laps.Count; lap++)
            {
                string act_svg_path = string.Format("M{0} {1}", Datas.Instance.GetData().Laps[lap][0].Item1, Datas.Instance.GetData().Laps[lap][0].Item2);

                for (int i = 0; i < Datas.Instance.GetData().Laps[lap].Count; i++)
                {
                    act_svg_path += string.Format(" L{0} {1}", Datas.Instance.GetData().Laps[lap][i].Item1, Datas.Instance.GetData().Laps[lap][i].Item2);
                }

                svg_pathes.Add(act_svg_path);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public List<string> SvgPathes
        {
            get
            {
                return svg_pathes;
            }
        }

        void calculateLaps()
        {
            Datas.Instance.GetData().Laps.Clear();

            double latitude_min = double.MaxValue;
            double longitude_min = double.MaxValue;
            foreach (var item in Datas.Instance.GetData().Latitude)
            {
                if (item != 0)
                {
                    if (item < latitude_min)
                    {
                        latitude_min = item;
                    }
                }
            }
            foreach (var item in Datas.Instance.GetData().Longitude)
            {
                if (item != 0)
                {
                    if (item < longitude_min)
                    {
                        longitude_min = item;
                    }
                }
            }

            double scale = Math.Pow(10, 5);

            List<double> latitude = new List<double>();
            List<double> longitude = new List<double>();

            for (int i = 0; i < Datas.Instance.GetData().Latitude.Count; i++)
            {
                if (Datas.Instance.GetData().Latitude[i] != 0)
                {
                    latitude.Add(Math.Round((Datas.Instance.GetData().Latitude[i] - latitude_min) * scale));
                }
                if (Datas.Instance.GetData().Longitude[i] != 0)
                {
                    longitude.Add(Math.Round((Datas.Instance.GetData().Longitude[i] - longitude_min) * scale));
                }
            }

            int after = 500;
            int radius = 40;

            List<Tuple<double, double>> act_lap = new List<Tuple<double, double>>();

            bool last_in_circle = true;
            int last_circle_index = 0;

            for (int i = 0; i < latitude.Count; i++)
            {
                act_lap.Add(new Tuple<double, double>(latitude[i], longitude[i]));
                if (i >= last_circle_index + after)
                {
                    last_in_circle = false;
                }

                if (!last_in_circle)
                {
                    if (Math.Sqrt(Math.Pow(latitude[i] - latitude[0], 2) + Math.Pow(longitude[i] - longitude[0], 2)) < radius)
                    {
                        last_in_circle = true;
                        last_circle_index = i;

                        if (i + 1 < latitude.Count)
                        {
                            act_lap.Add(new Tuple<double, double>(latitude[i + 1], longitude[i + 1]));
                        }

                        List<Tuple<double, double>> add = new List<Tuple<double, double>>(act_lap);

                        Datas.Instance.GetData().Laps.Add(add);
                        act_lap.Clear();
                    }
                }
            }
        }
    }
}
