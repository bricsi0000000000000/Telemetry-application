using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    class Workspace
    {
        string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        List<Tab> tabs = new List<Tab>();
        Tab settings_tab;

        public Workspace(string name)
        {
            this.name = name;

            settings_tab = new Tab(string.Format("{0}_settings", name));
        }

        public TabItem GetTab(string name)
        {
            return tabs.Find(n => n.TabItem.Name.Equals(string.Format("{0}_tab_item", name))).TabItem;
        }

        public void AddTab(Tab tab)
        {
            tabs.Add(tab);
        }

        public Tab SettingsTab
        {
            get
            {
                return settings_tab;
            }
        }
    }
}
