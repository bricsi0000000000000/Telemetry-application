using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class TabManager
    {
        #region instance
        private static TabManager instance = null;
        private TabManager() { }

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
        #endregion

        List<Tab> tabs = new List<Tab>();
        string active_workspace;
        public string ActiveWorkspace
        {
            get
            {
                return active_workspace;
            }
            set
            {
                active_workspace = value;
            }
        }

        public void AddTab(Tab tab)
        {
            tabs.Add(tab);
        }

        public Tab GetTab(string name = "")
        {
            if (name.Equals(""))
            {
                return tabs.Find(n => n.Name == active_workspace);
            }
            return tabs.Find(n => n.Name == name);
        }
    }
}
