using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Settings.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Pilots
{
    /// <summary>
    /// Interaction logic for PilotContent.xaml
    /// </summary>
    public partial class LapsContent : UserControl
    {
        private readonly Driver driver;
        private readonly List<LapListElement> lap_list_elements = new List<LapListElement>();
        private readonly List<string> all_selected_channels = new List<string>();
        private AllLapListElement all_lap_list_element;
        private InputFile active_input_file;

        public List<string> SelectedChannels { get; set; } = new List<string>();

        //  private List<Tuple<ushort, List<string>>> laps_selected_channels = new List<Tuple<ushort, List<string>>>();

        private bool distance_as_time = false;
        private Filter filter = Filter.kalman;

        private readonly Group group;

        public LapsContent(Driver driver, Group group)
        {
            InitializeComponent();

            this.driver = driver;
            this.group = group;

            //if (group != null)
            //{
            //    foreach (string attribute in group.Attributes)
            //    {
            //        all_selected_channels.Add(attribute);
            //    }
            //}

            InitInputFileCmbbox();
            InitLapListElements();
        }

        public void InitInputFileCmbbox()
        {
            input_files_cmbbox.Items.Clear();

            ushort name_index = 0;
            foreach (InputFile input_file in driver.InputFiles)
            {
                ComboBoxItem combo_box_item = new ComboBoxItem();
                combo_box_item.Content = input_file.FileName;
                combo_box_item.Name = string.Format("cmbboxitem{0}", name_index++);
                input_files_cmbbox.Items.Add(combo_box_item);
            }

            ComboBoxItem nothing_combo_box_item = new ComboBoxItem();
            nothing_combo_box_item.Content = "Choose file";
            nothing_combo_box_item.Name = string.Format("cmbboxitem{0}", name_index++);
            input_files_cmbbox.Items.Add(nothing_combo_box_item);
        }

        private void inputFilesCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (((ComboBoxItem)((ComboBox)sender).SelectedItem) != null)
            {
                string file_name = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
                active_input_file = pilot.GetInputFile(file_name);
                Console.WriteLine(active_input_file.FileName);
                avg_lap_svg.Data = Geometry.Parse(active_input_file.AvgLapSVG);

                updateLaps();
                BuildCharts();
                //((GGDiagram_UC)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilot.Name).Content).GetTab("Traction").Content).InitScatterPlot(active_input_file);
            }*/
            if (input_files_cmbbox.SelectedItem != null)
            {
                InputFilesCmbboxSelectionChange();
            }
        }

        public void InputFilesCmbboxSelectionChange()
        {
            string file_name = input_files_cmbbox.SelectedItem.ToString();
            file_name = file_name.Substring(38, file_name.Length - 38);
            if (!file_name.Equals("Choose file"))
            {
                active_input_file = driver.GetInputFile(file_name);

                if (active_input_file.Laps.Count > 0)
                {
                    avg_lap_svg.Data = Geometry.Parse(active_input_file.OneLapSVG(6));

                    updateLaps();
                    BuildCharts();
                }
                else
                {
                    error_snack_bar.MessageQueue.Enqueue(string.Format("No laps calculated. Change it in the map settings."),
                                                         null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            else
            {
                charts_grid.Children.Clear();
                charts_grid.RowDefinitions.Clear();
            }
        }

        public void ChangeAllLapsActive(bool active)
        {
            for (int i = 0; i < lap_list_elements.Count; i++)
            {
                if (lap_list_elements[i].Active != active)
                {
                    lap_list_elements[i].Active = active;
                }
            }
            InitLapListElements();
        }

        public void InitLapListElements()
        {
            if (active_input_file != null)
            {
                laps_stackpanel.Children.Clear();
                for (int i = 0; i < lap_list_elements.Count; i++)
                {
                    laps_stackpanel.Children.Add(lap_list_elements[i]);
                    active_input_file.ActiveLaps[i] = lap_list_elements[i].Active;
                }
                laps_stackpanel.Children.Insert(0, all_lap_list_element);
            }
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
            ChartBuilder.Build(ref charts_grid,
                               activeLaps,
                               group == null ? SelectedChannels : group.Attributes,
                               active_input_file,
                               distance_as_time,
                               filter,
                               group == null ? TextManager.DiagramCustomTabName : group.Name
                               );
            /*  StreamWriter sw = new StreamWriter("gps_adatok.csv");
              foreach (var item in active_input_file.MapPoints)
              {
                  sw.WriteLine("{0};{1}", item.X, item.Y);
              }
              sw.Close();*/
        }

        private List<Lap> activeLaps
        {
            get
            {
                List<Lap> active_laps = new List<Lap>();
                for (ushort i = 0; i < active_input_file.ActiveLaps.Count; i++)
                {
                    if (active_input_file.ActiveLaps[i])
                    {
                        active_laps.Add(active_input_file.Laps[i]);
                    }
                }

                return active_laps;
            }
        }

        private void updateLaps()
        {
            lap_list_elements.Clear();
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
            laps_stackpanel.Children.Insert(0, all_lap_list_element);
        }

        private void metricCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (active_input_file != null)
            {
                distance_as_time = !distance_as_time;
                BuildCharts();
            }
        }

        private void filterCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (active_input_file != null)
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
            }
        }

        public void InitFirstInputFilesContent() => input_files_cmbbox.SelectedItem = input_files_cmbbox.Items[0];

        public LapListElement GetLapListElement(int lap_index) => lap_list_elements[lap_index];

        private void resetZoom_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in charts_grid.Children)
            {
                if (child is Chart)
                {
                    ((Chart)child).ResetZoom();
                }
            }
        }
    }
}
