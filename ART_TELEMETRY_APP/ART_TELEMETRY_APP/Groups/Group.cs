using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings;

namespace ART_TELEMETRY_APP
{
    public class Group
    {
        string name;
        List<Pilot> pilots = new List<Pilot>();
        List<string> selected_channels = new List<string>();
        ZoomingOptions zoom;
        
        //TODO chartsettings class

        public Group(string name)
        {
            this.name = name;
            zoom = ZoomingOptions.X;
        }

        public void CalculateMultiplier()
        {
            double max = 0;
            string max_attribute = "";
            foreach (string attribute in selected_channels)
            {
                ChartValues<double> data = DataManager.GetData().GetSingleData(attribute).Datas;
                double act_max = data.Max();
                if(act_max > max)
                {
                    max = act_max;
                    max_attribute = attribute;
                }
            }

            foreach (string attribute in selected_channels)
            {
                if (!attribute.Equals(max_attribute))
                {
                    ChartValues<double> data = DataManager.GetData().GetSingleData(attribute).Datas;
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

        public List<string> SelectedChannels
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

        public ZoomingOptions Zooming
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;
            }
        }
    }
}
