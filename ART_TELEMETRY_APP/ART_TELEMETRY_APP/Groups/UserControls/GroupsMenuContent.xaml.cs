using ART_TELEMETRY_APP.Groups.Classes;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Interaction logic for GroupsMenuContent.xaml
    /// </summary>
    public partial class GroupsMenuContent : UserControl
    {
        private readonly List<TabItem> groups_tabs = new List<TabItem>();

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

        public TabItem GetTab(string name) => groups_tabs.Find(n => n.Header.Equals(name));
    }
}
