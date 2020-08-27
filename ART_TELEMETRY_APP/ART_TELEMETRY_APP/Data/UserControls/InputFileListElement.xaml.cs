using ART_TELEMETRY_APP.Maps;
using ART_TELEMETRY_APP.Maps.Classes;
using ART_TELEMETRY_APP.Maps.UserControls;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Pilots
{
    /// <summary>
    /// This User Control represents one input file under a driver with all the saved maps.
    /// </summary>
    public partial class InputFileListElement : UserControl
    {
       private readonly string driver_name;
       private readonly string file_name;
       private readonly Grid progressbar_grid;
       private readonly ProgressBar progressbar;
       private readonly Label progressbar_lbl;

        public InputFileListElement(string file_name,
                                    string driver_name,
                                    ref Grid progressbar_grid,
                                    ref ProgressBar progressbar,
                                    ref Label progressbar_lbl
                                    )
        {
            InitializeComponent();

            this.driver_name = driver_name;
            this.file_name = file_name;
            this.progressbar_grid = progressbar_grid;
            this.progressbar = progressbar;
            this.progressbar_lbl = progressbar_lbl;

            file_name_lbl.Content = this.file_name;

            updateMapsCmbbox();
        }

        private void updateMapsCmbbox()
        {
            Map attached_map = null;
            InputFile active_input_file = DriverManager.GetDriver(driver_name).GetInputFile(file_name);
            if (active_input_file != null)
            {
                attached_map = active_input_file.ActiveMap;
            }

            foreach (Map map in MapManager.Maps)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = string.Format("{0}\t{1}", map.Year, map.Name);
                //item.IsSelected = attached_map != null;
                maps_cmbbox.Items.Add(item);
            }
        }

        private void deleteInputFile_Click(object sender, RoutedEventArgs e)
        {
            DriverManager.GetDriver(driver_name).RemoveInputFile(file_name_lbl.Content.ToString());
            ((DriversMenuContent)TabManager.GetTab(TextManager.DriversMenuName).Content).InitDrivers();
            //((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab("Laps").Content).InitInputFileCmbbox();
        }

        /* private void settingsInputFile_Click(object sender, RoutedEventArgs e)
         {
             TabManager.GetTab("Settings").IsSelected = true; //TODO: ha több settings van mint csak a Maps, akkor még azt is selected-re kell tenni. :)
         }*/

        private void mapsCmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = maps_cmbbox.SelectedItem.ToString().Split(':').Last();
            name = name.Substring(1, name.Length - 1);
            DriverManager.GetDriver(driver_name).GetInputFile(file_name).ActiveMap =
                MapManager.GetMap(name.Split('\t')[1], int.Parse(name.Split('\t')[0]));

            progressbar.Visibility = Visibility.Visible;
            progressbar.IsIndeterminate = true;
            progressbar_lbl.Content = "Calculating laps..";

            ((MapSettings)((SettingsMenuContent)TabManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.MapsSettingsName).Content).ActiveMapSettingsItem =
                ((MapSettings)((SettingsMenuContent)TabManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.MapsSettingsName).Content).GetMapSettingsItem(name.Split('\t')[1], int.Parse(name.Split('\t')[0]));

            ((MapSettings)((SettingsMenuContent)TabManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.MapsSettingsName).Content).UpdateActiveMapSettingsContent(progressbar_grid);

            //((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(TextManager.DiagramCustomTabName).Content).InputFilesCmbboxSelectionChange();
        }
    }
}
