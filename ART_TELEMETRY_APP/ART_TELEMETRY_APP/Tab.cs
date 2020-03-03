using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    class Tab
    {
        TabItem tab;
        TabControl workspaces;

        public Tab(string tab_name, string tab_header, TabControl workspaces)
        {
            this.workspaces = workspaces;
            tab = new TabItem();
            tab.Name = tab_name;
            tab.Header = tab_header;
            TabManager.Instance.AddTab(this);
        }

        public void AddContent(Label label)
        {
            tab.Content = label;
        }

        public TabItem Tab_Item
        {
            get
            {
                return tab;
            }
        }
    }
}
