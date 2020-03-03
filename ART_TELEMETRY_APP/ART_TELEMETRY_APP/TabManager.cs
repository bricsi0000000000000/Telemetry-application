using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class TabManager
    {
        private static TabManager instance = null;
        public static TabManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TabManager();
                }
                return instance;
            }
        }

        List<Tab> tabs = new List<Tab>();

        public List<Tab> Tabs
        {
            get
            {
                return tabs;
            }
        }

        public void AddTab(Tab tab)
        {
            tabs.Add(tab);
        }

        public Tab GetTab(string name)
        {
            return tabs.Find(n => n.Tab_Item.Name == name);
        }
    }
}
