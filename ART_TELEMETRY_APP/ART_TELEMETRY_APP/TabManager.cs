using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    static class TabManager
    {
        static List<Tab> tabs = new List<Tab>();

        static string active_tab;
        public static string ActiveTab
        {
            get
            {
                return active_tab;
            }
            set
            {
                active_tab = value;
            }
        }

        public static List<Tab> Tabs
        {
            get
            {
                return tabs;
            }
        }

        public static void AddTab(Tab tab)
        {
            tabs.Add(tab);
        }

        public static Tab GetTab(string name = "")
        {
            if (name.Equals(""))
            {
                return tabs.Find(n => n.Name == active_tab);
            }
            return tabs.Find(n => n.Name == name);
        }

        public static void InitTabs(TabablzControl tab_control)
        {
            Tab diagrams_tab = new Tab("Diagrams");
            diagrams_tab.DiagramsUI = new Diagrams_UI();
            diagrams_tab.TabItem.Content = diagrams_tab.DiagramsUI;
            tabs.Add(diagrams_tab);
            tab_control.Items.Add(diagrams_tab.TabItem);

            Tab lap_report_tab = new Tab("LapReport");
            lap_report_tab.LapReportUI = new LapReport_UI();
            lap_report_tab.TabItem.Content = lap_report_tab.LapReportUI;
            tabs.Add(lap_report_tab);
            tab_control.Items.Add(lap_report_tab.TabItem);

            Tab gg_diagram_tab = new Tab("GG");
            gg_diagram_tab.DiagramsUI = new Diagrams_UI();
            gg_diagram_tab.TabItem.Content = gg_diagram_tab.DiagramsUI;
            tabs.Add(gg_diagram_tab);
            tab_control.Items.Add(gg_diagram_tab.TabItem);

            Tab map_tab = new Tab("Map");
            map_tab.MapUI = new Map_UI();
            map_tab.TabItem.Content = map_tab.MapUI;
            tabs.Add(map_tab);
            tab_control.Items.Add(map_tab.TabItem);
        }
    }
}
