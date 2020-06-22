using ART_TELEMETRY_APP.Groups.UserControls;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings;
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
        static List<TabItem> menu_items = new List<TabItem>();

        public static void InitMenuItems(TabControl tab_control)
        {
            TabItem settings_menu_item = new TabItem();
            settings_menu_item.Header = "Settings";
            settings_menu_item.Content = new SettingsMenuContent();
            settings_menu_item.Name = "settings_menu_item";
            menu_items.Add(settings_menu_item);
            tab_control.Items.Add(settings_menu_item);

            TabItem datas_menu_item = new TabItem();
            datas_menu_item.Header = "Diagrams";
            datas_menu_item.Content = new DatasMenuContent();
            datas_menu_item.Name = "diagrams_menu_item";
            menu_items.Add(datas_menu_item);
            tab_control.Items.Add(datas_menu_item);

            TabItem pilots_menu_item = new TabItem();
            pilots_menu_item.Header = "Pilots";
            pilots_menu_item.Content = new PilotsMenuContent();
            pilots_menu_item.Name = "pilots_menu_item";
            pilots_menu_item.IsSelected = true;
            menu_items.Add(pilots_menu_item);
            tab_control.Items.Add(pilots_menu_item);

           /* TabItem groups_menu_item = new TabItem();
            groups_menu_item.Header = "Groups";
            GroupsMenuContent groups_menu_content = new GroupsMenuContent();
            groups_menu_item.Content = groups_menu_content;
            groups_menu_item.Name = "groups_menu_item";
            menu_items.Add(groups_menu_item);
            tab_control.Items.Add(groups_menu_item);
            groups_menu_content.InitGroupsTabs();*/
        }

        public static TabItem GetTab(string name)
        {
            return menu_items.Find(n => n.Header.Equals(name));
        }
    }
}
