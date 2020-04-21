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
    /// Interaction logic for Diagrams_UI.xaml
    /// </summary>
    public partial class Diagrams_UI : UserControl
    {
        Group group;
        public Diagrams_UI(Group group)
        {
            InitializeComponent();

            this.group = group;

          /*  foreach (Pilot pilot in PilotManager.Pilots)
            {
                Expander expander = new Expander();
                expander.Header = pilot.Name;
                expander.HorizontalAlignment = HorizontalAlignment.Stretch;
                ListBox input_files_list_box = new ListBox();
                foreach (InputFile input_file in pilot.InputFiles)
                {
                    foreach (Data data in input_file.Datas)
                    {
                        input_files_list_box.Items.Add(new ChannelsListBoxItem_UC(data.Name));
                    }
                }
                expander.Content = input_files_list_box;
                pilots_stackpanel.Children.Add(expander);
            }*/

            group.ClearSelectedPilotsAndLaps();
            foreach (Pilot pilot in group.Pilots)
            {
                selected_pilots_wrappanel.Children.Add(new SelectPilotsAndLaps_UC(ref group, pilot.Name, diagrams_grid));
               /* StackPanel selected_laps_stackpanel = new StackPanel();
                List<bool> laps = new List<bool>();
                for (int i = 0; i < LapManager.Laps.Count; i++)
                {
                    laps.Add(false);
                    CheckBox check_box = new CheckBox();
                    check_box.Name = string.Format("{0}chkbox", pilot.Name);
                    check_box.Content = string.Format("{0}. lap", i + 1);
                    check_box.Margin = new Thickness(5);
                    check_box.Checked += new RoutedEventHandler(lap_checkbox_Checked);
                    check_box.Unchecked += new RoutedEventHandler(lap_checkbox_Checked);
                    selected_laps_stackpanel.Children.Add(check_box);
                }
                selected_pilots_wrappanel.Children.Add(selected_laps_stackpanel);
                group.SelectedPilotsAndLaps.Add(new Tuple<string, List<bool>>(pilot.Name, laps));*/
            }
        }
    }
}
