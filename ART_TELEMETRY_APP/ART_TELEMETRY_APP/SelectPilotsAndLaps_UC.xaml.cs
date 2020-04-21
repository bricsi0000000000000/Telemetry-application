using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
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

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// Interaction logic for SelectPilotsAndLaps_UC.xaml
    /// </summary>
    public partial class SelectPilotsAndLaps_UC : UserControl
    {
        Group group;
        Grid diagrams_grid;

        public SelectPilotsAndLaps_UC(ref Group group, string pilots_name, Grid diagrams_grid)
        {
            InitializeComponent();

            this.group = group;
            this.diagrams_grid = diagrams_grid;
            pilots_name_lbl.Content = pilots_name;

            List<bool> laps = new List<bool>();
            for (int i = 0; i < LapManager.Laps.Count; i++)
            {
                laps.Add(false);
                CheckBox check_box = new CheckBox();
                check_box.Name = string.Format("{0}chkbox", pilots_name);
                check_box.Content = string.Format("{0}. lap", i + 1);
                check_box.Margin = new Thickness(5);
                check_box.Checked += new RoutedEventHandler(lap_checkbox_Checked);
                check_box.Unchecked += new RoutedEventHandler(lap_checkbox_Checked);
                selected_laps_stackpanel.Children.Add(check_box);
            }
            group.SelectedPilotsAndLaps.Add(new Tuple<string, List<bool>>(pilots_name, laps));
        }

        private void lap_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            string index_name = ((CheckBox)sender).Content.ToString().Split(' ')[0];
            int index = int.Parse(index_name.Substring(0, index_name.Length - 1)) - 1;
            string pilots_name = ((CheckBox)sender).Name.ToString();
            pilots_name = pilots_name.Substring(0, pilots_name.Length - 6);
            group.SetLap(pilots_name, index);
           // ChartBuilder.Build(diagrams_grid, group);
        }
    }
}
