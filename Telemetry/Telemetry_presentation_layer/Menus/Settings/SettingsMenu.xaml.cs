using System.Collections.Generic;
using System.Windows.Controls;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Menus.Settings.Groups;
using Telemetry_presentation_layer.Menus.Settings.InputFiles;
using Telemetry_presentation_layer.Menus.Settings.Live;
using Telemetry_presentation_layer.Menus.Settings.Units;

namespace Telemetry_presentation_layer.Menus.Settings
{
    /// <summary>
    /// Represents the content of the main settings menu.
    /// </summary>
    public partial class SettingsMenu : UserControl
    {
        /// <summary>
        /// List of <see cref="TabItem"/>s in this settings.
        /// </summary>
        private static readonly List<TabItem> settingsTabs = new List<TabItem>();

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsMenu()
        {
            InitializeComponent();
            InitSettingsTabs();
        }

        /// <summary>
        /// Initializes settings menu tabs.
        /// </summary>
        public void InitSettingsTabs()
        {
            settingsTabControl.Items.Clear();

            AddSettingsTab(TextManager.FilesSettingsName, new InputFilesSettings(), selected: true);
            AddSettingsTab(TextManager.GroupsSettingsName, new GroupSettings());
            AddSettingsTab(TextManager.UnitsSettingsName, new UnitsMenu());
            //   AddSettingsTab(TextManager.TracksSettingsName, new TrackSettings());

            /*  AddSettingsTab(new TabItem
              {
                  Header = TextManager.SectorsSettingsName,
                  // Content = new SectorsSettings()
              });*/

            /*  AddSettingsTab(new TabItem
              {
                  Header = TextManager.GeneralSettingsName,
                  Content = new GeneralSettings()
              });*/
        }

        /// <summary>
        /// Adds a newly created <see cref="TabItem"/> to <see cref="settingsTabControl"/>.
        /// </summary>
        /// <param name="header"><see cref="TabItem"/>s name.</param>
        /// <param name="content">
        /// <see cref="TabItem"/>s content.
        /// It can be anything, I use it with a <see cref="UserControl"/>.
        /// </param>
        /// <param name="selected">
        /// If true, the <see cref="TabItem"/> will be selected.
        /// If false, the <see cref="TabItem"/> will not be selected.
        /// Default is false.
        /// </param>
        private void AddSettingsTab(string header, object content, bool selected = false)
        {
            var tab = new TabItem
            {
                Header = header,
                Content = content,
                Name = $"{header.Replace(" ", "")}Tab",
                IsSelected = selected
            };

            settingsTabs.Add(tab);
            settingsTabControl.Items.Add(tab);
        }

        /// <summary>
        /// Finds a <see cref="TabItem"/> in <see cref="settingsTabs"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="TabItem"/>s name.</param>
        /// <returns>A <see cref="TabItem"/> whose name is <paramref name="name"/>.</returns>
        public TabItem GetTab(string name) => settingsTabs.Find(x => x.Header.Equals(name));
    }
}

