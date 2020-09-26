using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
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

        public void AddChart(ref Chart chart)
        {
            Charts.Add(chart);
        }

        public Chart GetChart(string name)
        {
            return Charts.Find(x => x.Name.Equals(name));
        }

        public void ChangeAllLapsActive(bool active)
        {
            /* for (int i = 0; i < lap_list_elements.Count; i++)
             {
                 if (lap_list_elements[i].Active != active)
                 {
                     lap_list_elements[i].Active = active;
                 }
             }
             InitLapListElements();*/
        }

        public void InitLapListElements()
        {
            /* if (active_input_file != null)
             {
                 LapsStackPanel.Children.Clear();
                 for (int i = 0; i < lap_list_elements.Count; i++)
                 {
                     LapsStackPanel.Children.Add(lap_list_elements[i]);
                    // active_input_file.ActiveLaps[i] = lap_list_elements[i].Active;
                 }
                 LapsStackPanel.Children.Insert(0, all_lap_list_element);
             }*/
        }

        public void ChangeAllSelectedChannels(List<string> selected_channels)
        {
            /*  all_selected_channels.Clear();
              foreach (string attribute in selected_channels)
              {
                  all_selected_channels.Add(attribute);
              }
              for (int i = 0; i < active_input_file.Laps.Count; i++)
              {
                  active_input_file.Laps[i].SelectedChannels.Clear();
                  foreach (string attribute in selected_channels)
                  {
                      active_input_file.Laps[i].SelectedChannels.Add(attribute);
                  }
              }*/
        }

        public void BuildCharts()
        {
            /*ChartBuilder.Build(ref charts_grid,
                               ActiveLaps,
                               group == null ? SelectedChannels : group.Attributes,
                               active_input_file,
                               distance_as_time,
                               filter,
                               group == null ? TextManager.DiagramCustomTabName : group.Name
                               );*/
            /*  StreamWriter sw = new StreamWriter("gps_adatok.csv");
              foreach (var item in active_input_file.MapPoints)
              {
                  sw.WriteLine("{0};{1}", item.X, item.Y);
              }
              sw.Close();*/
        }

        private List<Lap> ActiveLaps
        {
            get
            {
                /*List<Lap> activeLaps = new List<Lap>();
                for (ushort i = 0; i < active_input_file.ActiveLaps.Count; i++)
                {
                    if (active_input_file.ActiveLaps[i])
                    {
                        activeLaps.Add(active_input_file.Laps[i]);
                    }
                }*/

                return null;
            }
        }

        private void updateLaps()
        {
            /* lap_list_elements.Clear();
             laps_stackpanel.Children.Clear();

             List<string> channels = new List<string>();
             foreach (Data data in active_input_file.AllData)
             {
                 channels.Add(data.Attribute);
             }

             TimeSpan all_time = new TimeSpan();

             ushort worst_index = 1;
             ushort best_index = 1;
             TimeSpan worst_time = active_input_file.Laps[1].Time;
             TimeSpan best_time = active_input_file.Laps[1].Time;
             for (ushort i = 2; i < active_input_file.Laps.Count - 1; i++)
             {
                 if (active_input_file.Laps[i].Time > worst_time)
                 {
                     worst_time = active_input_file.Laps[i].Time;
                     worst_index = i;
                 }
             }

             for (ushort i = 2; i < active_input_file.Laps.Count - 1; i++)
             {
                 if (active_input_file.Laps[i].Time < best_time)
                 {
                     best_time = active_input_file.Laps[i].Time;
                     best_index = i;
                 }
             }

             for (ushort i = 0; i < active_input_file.Laps.Count; i++)
             {
                 LapListElement lap_list_element;
                 if (i + 1 >= active_input_file.Laps.Count)
                 {
                     lap_list_element = new LapListElement(active_input_file.Laps[i],
                                                           driver.Name,
                                                           active_input_file.ActiveLaps[active_input_file.Laps[i].Index],
                                                           channels,
                                                           group == null ? SelectedChannels : group.Attributes,
                                                           i > 0 && i < active_input_file.Laps.Count - 1 ? i == best_index ? 1 : i == worst_index ? 0 : 2 : 2,
                                                           group == null ? TextManager.DiagramCustomTabName : group.Name,
                                                           true
                                                           );
                 }
                 else
                 {
                     lap_list_element = new LapListElement(active_input_file.Laps[i],
                                                           driver.Name,
                                                           active_input_file.ActiveLaps[active_input_file.Laps[i].Index],
                                                           channels,
                                                           group == null ? SelectedChannels : group.Attributes,
                                                           i > 0 && i < active_input_file.Laps.Count - 1 ? i == best_index ? 1 : i == worst_index ? 0 : 2 : 2,
                                                           group == null ? TextManager.DiagramCustomTabName : group.Name
                                                           );
                 }

                 lap_list_elements.Add(lap_list_element);
                 laps_stackpanel.Children.Add(lap_list_element);

                 all_time += active_input_file.Laps[i].Time;
             }

             all_lap_list_element = new AllLapListElement(channels, all_time, driver.Name, all_selected_channels);
             laps_stackpanel.Children.Insert(0, all_lap_list_element);*/
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
             foreach (var child in ChartsGrid.Children)
             {
                 if (child is Chart)
                 {
                     ((Chart)child).ResetZoom();
                 }
             }
        }
    }
}
