using ART_TELEMETRY_APP.Tracks;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Tracks.UserControls;

namespace ART_TELEMETRY_APP.InputFiles.UserControls
{
    /// <summary>
    /// This User Control represents one input file under a driver with all the saved maps.
    /// </summary>
    public partial class InputFileListElement : UserControl
    {
        private readonly string fileName;
        private readonly Grid ProgressBarGrid;
        private readonly ProgressBar ProgressBar;
        private readonly Label ProgressBarLbl;

        public InputFileListElement(string fileName,
                                    ref Grid ProgressBarGrid,
                                    ref ProgressBar ProgressBar,
                                    ref Label ProgressBarLbl
                                    )
        {
            InitializeComponent();

            this.fileName = fileName;
            this.ProgressBarGrid = ProgressBarGrid;
            this.ProgressBar = ProgressBar;
            this.ProgressBarLbl = ProgressBarLbl;

            FileNameLbl.Content = fileName;

            UpdateTrackNamesCmbBox();
        }

        private void UpdateTrackNamesCmbBox()
        {
            foreach (var nameAndYear in TrackManager.TrackNamesAndYears)
            {
                TrackNamesCmbBox.Items.Add(new ComboBoxItem
                {
                    Content = string.Format("{0}\t{1}", nameAndYear.Item2, nameAndYear.Item1)
                });
            }
        }

        private void DeleteInputFile_Click(object sender, RoutedEventArgs e)
        {
            // DriverManager.GetDriver(driver_name).RemoveInputFile(file_name_lbl.Content.ToString());
            //  ((DriversMenu)MenuManager.GetTab(TextManager.DriversMenuName).Content).InitDriverCards();
            //((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab("Laps").Content).InitInputFileCmbbox();
        }

        /* private void settingsInputFile_Click(object sender, RoutedEventArgs e)
         {
             TabManager.GetTab("Settings").IsSelected = true; //TODO: ha több settings van mint csak a Maps, akkor még azt is selected-re kell tenni. :)
         }*/

        private void TrackNamesCmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
              string name = TrackNamesCmbBox.SelectedItem.ToString().Split(':').Last();
              name = name.Substring(1, name.Length - 1);

              InputFileManager.GetInputFile(fileName).ActiveTrack =
                  TrackManager.GetTrack(name.Split('\t')[1], int.Parse(name.Split('\t')[0]));

              ProgressBar.Visibility = Visibility.Visible;
              ProgressBar.IsIndeterminate = true;
              ProgressBarLbl.Content = "Calculating laps..";

              ((TrackSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.TracksSettingsName).Content)
                .SetActiveTrackSettingsItem(name.Split('\t')[1], int.Parse(name.Split('\t')[0]));

              ((TrackSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.TracksSettingsName).Content)
                .UpdateActiveMapSettingsContent(ProgressBarGrid);
            
            //((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(TextManager.DiagramCustomTabName).Content).InputFilesCmbboxSelectionChange();
        }
    }
}
