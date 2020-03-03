using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    class TabBuilder
    {
        private static TabBuilder instance = null;
        public static TabBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TabBuilder();
                }
                return instance;
            }
        }

        public void BuildTab(Tab tab, TabControl workspaces)
        {
            workspaces.Items.Add(tab.Tab_Item);
        }
    }
}
