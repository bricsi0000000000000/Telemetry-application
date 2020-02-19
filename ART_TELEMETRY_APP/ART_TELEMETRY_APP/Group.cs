using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;

namespace ART_TELEMETRY_APP
{
    class Group
    {
        string name;
        List<string> attributes = new List<string>();
        // CartesianChart chart;
       // bool chart_disable_animations;
       // bool chart_hoverable;
        ZoomingOptions zoom;

        public Group(string name)
        {
            this.name = name;
         //   chart_disable_animations = true;
          //  chart_hoverable = false;

            //  chart = new CartesianChart();

            //chart.DataTooltip = null;
            //chart.Name = name;
            zoom = ZoomingOptions.X;
            //chart.Hoverable = chart_hoverable;
            //chart.DisableAnimations = chart_disable_animations;

            /*
            Axis a = new Axis();
            a.Title = name;
            a.Position = AxisPosition.LeftBottom;

            chart.AxisX.Add(a);
           */
        }

        public void AddAttribute(string attribute)
        {
            attributes.Add(attribute);
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public List<string> Attributes
        {
            get
            {
                return this.attributes;
            }
        }

        /*public CartesianChart Chart
        {
            get
            {
                return chart;
            }
            set
            {
                chart = value;
            }
        }*/

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
