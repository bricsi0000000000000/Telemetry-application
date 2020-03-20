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

                if (tab.DiagramsUI != null)
                {
                    item.Content = new DiagramsSettings_UC();
                }
            }

            TabItem new_tab_item = new TabItem();
            new_tab_item.Header = "NewTab";
            tabs_tabcontrol.Items.Add(new_tab_item);
        }
    }
}
