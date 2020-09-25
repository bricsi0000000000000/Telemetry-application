using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Drivers.UserControls;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// On this page will be shown all the drivers with charts
    /// </summary>
    public partial class Diagrams : UserControl
    {
        public static List<TabItem> Tabs { get; } = new List<TabItem>();

        public Diagrams()
        {
            InitializeComponent();
            InitTabs();
        }

        public void InitTabs()
        {
            Tabs.Clear();
            TabControl.Items.Clear();
            ushort lapsContentIndex = 0;
            foreach (var group in GroupManager.Groups)
            {
                var groupItem = new TabItem
                {
                    Header = group.Name,
                    IsSelected = lapsContentIndex == 0,
                    Name = string.Format("GroupTabItem{0}", lapsContentIndex++)
                };

                if (group.Customizable)
                {
                    groupItem.Content = new LapsContent(group);
                }
                else
                {
                    if (group.Name.Equals(TextManager.TractionTabName))
                    {
                        groupItem.Content = new GGDiagram_UC();
                    }
                    else if (group.Name.Equals(TextManager.LapReportTabName))
                    {
                        //groupItem.Content = new LapsContent();
                    }
                }
                Tabs.Add(groupItem);
                TabControl.Items.Add(groupItem);
            }
        }

        public TabItem GetTab(string tabHeader) => Tabs.Find(x => x.Header.Equals(tabHeader));
    }
}
