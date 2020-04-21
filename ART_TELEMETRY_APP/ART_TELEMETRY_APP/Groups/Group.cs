using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;

namespace ART_TELEMETRY_APP
{
    public class Group
    {
        string name;
        List<Pilot> pilots = new List<Pilot>();
        List<Data> selected_channels = new List<Data>();
        List<Tuple<string, List<bool>>> selected_pilots_and_laps = new List<Tuple<string, List<bool>>>();
        //TODO chartsettings class

        public Group(string name)
        {
            this.name = name;
        }

        public void CalculateMultiplier()
        {
            double max = 0;
            string max_attribute = "";
            foreach (Data attribute in selected_channels)
            {
                ChartValues<double> data = DataManager.GetData().GetData(attribute.Attribute).Datas;
                double act_max = data.Max();
                if (act_max > max)
                {
                    max = act_max;
                    max_attribute = attribute.Attribute;
                }
            }

            foreach (Data attribute in selected_channels)
            {
                if (!attribute.Equals(max_attribute))
                {
                    ChartValues<double> data = DataManager.GetData().GetData(attribute.Attribute).Datas;
                    double multiplier = max / data.Max();

                    for (int i = 0; i < data.Count; i++)
                    {
                        data[i] *= multiplier;
                    }
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                name = value;
            }
        }

        public List<Data> SelectedChannels
        {
            get
            {
                return this.selected_channels;
            }
            set
            {
                selected_channels = value;
            }
        }

        public List<Pilot> Pilots
        {
            get
            {
                return this.pilots;
            }
            set
            {
                pilots = value;
            }
        }

        public List<Tuple<string, List<bool>>> SelectedPilotsAndLaps
        {
            get
            {
                return this.selected_pilots_and_laps;
            }
            set
            {
                selected_pilots_and_laps = value;
            }
        }

        public void SetLap(string pilots_name, int lap_index)
        {
            foreach (Tuple<string, List<bool>> item in selected_pilots_and_laps)
            {
                if (item.Item1 == pilots_name)
                {
                    item.Item2[lap_index] = !item.Item2[lap_index];
                }
            }
        }

        public void ClearSelectedPilotsAndLaps()
        {
            foreach (var item in selected_pilots_and_laps)
            {
                for (int i = 0; i < item.Item2.Count; i++)
                {
                    item.Item2[i] = false;
                }
            }
        }
    }
}
