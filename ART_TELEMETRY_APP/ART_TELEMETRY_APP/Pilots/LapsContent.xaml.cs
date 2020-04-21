using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using System;
using System.Collections.Generic;
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
                one_lap_svg.Data = Geometry.Parse(active_input_file.AvgLapSVG);
                updateLaps();

            }
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
            }
        }

        public void BuildCharts()
        {
            ChartBuilder.Build(charts_grid, activeLaps, active_input_file);
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

            foreach (Lap lap in active_input_file.Laps)
            {
                LapListElement lap_list_element = new LapListElement(lap, pilot.Name,
                                                                     active_input_file.ActiveLaps[lap.Index],
                                                                     channels
                                                                     );
                lap_list_elements.Add(lap_list_element);
                laps_stackpanel.Children.Add(lap_list_element);
            }
        }

        private void metricCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
