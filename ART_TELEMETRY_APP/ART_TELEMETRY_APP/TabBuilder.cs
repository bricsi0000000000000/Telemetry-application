using Dragablz;
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
        #region instance
        private static TabBuilder instance = null;
        private TabBuilder() { }

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
        #endregion

        public void Build(TabItem tab, TabablzControl tabs)
        {
            tab.IsSelected = true;
            tabs.Items.Add(tab);
        }
    }
}
