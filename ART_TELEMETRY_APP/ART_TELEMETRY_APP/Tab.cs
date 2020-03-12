using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ART_TELEMETRY_APP
{
    class Tab
    {
        string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        TabItem tab_item;
        public TabItem TabItem
        {
            get
            {
                return tab_item;
            }
        }

        Diagrams_UI diagrams_UI;
        public Diagrams_UI DiagramsUI
        {
            get
            {
                return diagrams_UI;
            }
            set
            {
                diagrams_UI = value;
            }
        }

        LapReport_UI lap_report_UI;
        public LapReport_UI LapReportUI
        {
            get
            {
                return lap_report_UI;
            }
            set
            {
                lap_report_UI = value;
            }
        }

        Map_UI map_UI;
        public Map_UI MapUI
        {
            get
            {
                return map_UI;
            }
            set
            {
                map_UI = value;
            }
        }

        public Tab(string name)
        {
            this.name = name;
            tab_item = new TabItem();
            tab_item.Header = name;
            tab_item.Name = string.Format("{0}_tab_item", name);
        }
    }
}
