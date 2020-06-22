using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Pilots
{
    /// <summary>
    /// Interaction logic for PilotContent.xaml
    /// </summary>
    public partial class LapsContent : UserControl
    {
        Pilot pilot;
        InputFile active_input_file;
        List<LapListElement> lap_list_elements = new List<LapListElement>();
        AllLapListElement all_lap_list_element;
        List<string> all_selected_channels = new List<string>();
        bool dist_as_time = false;
        public enum Filter
        {
            kalman, nothing, both
        }
        Filter filter = Filter.kalman;

        public LapsContent(Pilot pilot)
        {
            InitializeComponent();

            this.pilot = pilot;

            InitInputFileCmbbox();
            InitLapListElements();
        }

        public void InitInputFileCmbbox()
        {
            input_files_cmbbox.Items.Clear();

            int index = 0;
            foreach (InputFile input_file in pilot.InputFiles)
            {
                Console.WriteLine(input_file.FileName);
                ComboBoxItem combo_box_item = new ComboBoxItem();
                combo_box_item.Content = input_file.FileName;
                combo_box_item.Name = string.Format("cmbboxitem{0}", index++);
                input_files_cmbbox.Items.Add(combo_box_item);
            }
        }

        private void inputFilesCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)((ComboBox)sender).SelectedItem) != null)
            {
                string file_name = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
                active_input_file = pilot.GetInputFile(file_name);
                avg_lap_svg.Data = Geometry.Parse(active_input_file.AvgLapSVG);

                updateLaps();
                BuildCharts();
                //((GGDiagram_UC)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilot.Name).Content).GetTab("Traction").Content).InitScatterPlot(active_input_file);
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
            all_selected_channels.Clear();
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
            }
        }

        public void BuildCharts()
        {
            ChartBuilder.Build(charts_grid, activeLaps, active_input_file, dist_as_time, filter);
        }

        private List<Lap> activeLaps
        {
            get
            {
                List<Lap> active_laps = new List<Lap>();
                for (int i = 0; i < active_input_file.ActiveLaps.Count; i++)
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
            foreach (Data data in active_input_file.Datas)
            {
                channels.Add(data.Attribute);
            }

            TimeSpan all_time = new TimeSpan();

            int worst_index = 1;
            int best_index = 1;
            TimeSpan worst_time = active_input_file.Laps[1].Time;
            TimeSpan best_time = active_input_file.Laps[1].Time;
            for (int i = 2; i < active_input_file.Laps.Count - 1; i++)
            {
                if (active_input_file.Laps[i].Time > worst_time)
                {
                    worst_time = active_input_file.Laps[i].Time;
                    worst_index = i;
                }
            }

            for (int i = 2; i < active_input_file.Laps.Count - 1; i++)
            {
                if (active_input_file.Laps[i].Time < best_time)
                {
                    best_time = active_input_file.Laps[i].Time;
                    best_index = i;
                }
            }

            for (int i = 0; i < active_input_file.Laps.Count; i++)
            {
                LapListElement lap_list_element;
                if (i + 1 >= active_input_file.Laps.Count)
                {
                    lap_list_element = new LapListElement(active_input_file.Laps[i], pilot.Name,
                                                          active_input_file.ActiveLaps[active_input_file.Laps[i].Index],
                                                          channels, i > 0 && i < active_input_file.Laps.Count - 1 ? i == best_index ? 1 : i == worst_index ? 0 : 2 : 2,
                                                          true
                                                          );
                }
                else
                {
                    lap_list_element = new LapListElement(active_input_file.Laps[i], pilot.Name,
                                                          active_input_file.ActiveLaps[active_input_file.Laps[i].Index],
                                                          channels, i > 0 && i < active_input_file.Laps.Count - 1 ? i == best_index ? 1 : i == worst_index ? 0 : 2 : 2
                                                          );
                }

                lap_list_elements.Add(lap_list_element);
                laps_stackpanel.Children.Add(lap_list_element);

                all_time += active_input_file.Laps[i].Time;
            }

            all_lap_list_element = new AllLapListElement(channels, all_time, pilot.Name, all_selected_channels);
            laps_stackpanel.Children.Insert(0, all_lap_list_element);
        }

        private void metricCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (active_input_file != null)
            {
                dist_as_time = !dist_as_time;
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
                        if(((ComboBoxItem)item).Content.Equals("Kalman"))
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

        public void InitFirstInputFilesContent()
        {
            input_files_cmbbox.SelectedItem = input_files_cmbbox.Items[0];
        }

        public LapListElement GetLapListElement(int lap_index)
        {
            return lap_list_elements[lap_index];
        }

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
