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
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Settings
{
    /// <summary>
    /// Interaction logic for Settings_Window.xaml
    /// </summary>
    public partial class Settings_Window : Window
    {
        public Settings_Window()
        {
            InitializeComponent();

            initTabs();
        }
    
        void initTabs()
        {
            foreach (Tab tab in TabManager.Tabs)
            {
                TabItem item = new TabItem();
                item.Header = tab.TabItem.Header;
                tabs_tabcontrol.Items.Add(item);
            }

            TabItem custom_tab = new TabItem();
            custom_tab.Header = "Custom";
            tabs_tabcontrol.Items.Add(custom_tab);

            DiagramsSettings_UC diagrams_settings_UC = new DiagramsSettings_UC();
            SettingsManager.SettingsWindowContent_UC = diagrams_settings_UC;

            custom_tab.Content = diagrams_settings_UC;
            custom_tab.IsSelected = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsManager.SettingsIsOpen = false;

            TabManager.MakeDiagrams();
        }
    }
}
