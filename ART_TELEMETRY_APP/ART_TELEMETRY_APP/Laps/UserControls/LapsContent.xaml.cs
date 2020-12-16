using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Laps.UserControls
{
    /// <summary>
    /// Interaction logic for PilotContent.xaml
    /// </summary>
    public partial class LapsContent : UserControl
    {
        public Group Group { get; set; }
        public List<DriverItem> DriverItems { get; } = new List<DriverItem>();
        public List<Chart> Charts { get; } = new List<Chart>();

        public LapsContent(Group group)
        {
            InitializeComponent();

            Group = group;
        }

        public void AddChart(ref Chart chart, string name)
        {
            Charts.Add(chart);
        }

        public Chart GetChart(string name)
        {
            return Charts.Find(x => x.ChartName.Equals(name));
        }

        public void UpdateCursorData()
        {
            for (int i = 0; i < ChartManager.CursorChannelNames.Count; i++)
            {
                CursorDataLbl.Content += string.Format("{0}: {1}\n", ChartManager.CursorChannelNames[i], ChartManager.CursorChannelData[i]);
            }
        }

        private void MetricCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*  if (active_input_file != null)
              {
                  distance_as_time = !distance_as_time;
                  BuildCharts();
              }*/
        }

        private void FilterCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* if (active_input_file != null)
             {
                 foreach (var item in ((ComboBox)sender).Items)
                 {
                     if (((ComboBoxItem)item).IsSelected)
                     {
                         if (((ComboBoxItem)item).Content.Equals("Kalman"))
                         {
                             filter = Filter.kalman;
                         }
                         else if (((ComboBoxItem)item).Content.Equals("Both"))
                         {
                             filter = Filter.both;
                         }
                         else
                         {
                             filter = Filter.nothing;
                         }
                     }
                 }
                 BuildCharts();
             }*/
        }

        //     public LapListElement GetLapListElement(int lap_index) => lap_list_elements[lap_index];

        private void resetZoom_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ChartsGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
    }
}
