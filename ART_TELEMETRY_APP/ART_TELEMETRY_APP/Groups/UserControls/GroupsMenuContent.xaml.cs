using ART_TELEMETRY_APP.Groups.Classes;
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

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Interaction logic for GroupsMenuContent.xaml
    /// </summary>
    public partial class GroupsMenuContent : UserControl
    {
        List<TabItem> groups_tabs = new List<TabItem>();

        public GroupsMenuContent()
        {
            InitializeComponent();
            InitGroupsTabs();
        }

        public void InitGroupsTabs()
        {
            tabControl.Items.Clear();
            foreach (Group group in GroupManager.Groups)
            {
                TabItem tab_item = new TabItem();
                tab_item.Header = group.Name;
                tab_item.Content = new GroupContent();
                groups_tabs.Add(tab_item);
                tabControl.Items.Add(tab_item);
            }
        }

        public TabItem GetTab(string name)
        {
            return groups_tabs.Find(n => n.Header.Equals(name));
        }
    }
}
