using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ART_TELEMETRY_APP
{
    class Workspace : Tab
    {
        Dragablz.Dockablz.Layout layout;

        public Workspace(string name) : base(name)
        {
            layout = new Dragablz.Dockablz.Layout();
            settings_tab = new Tab(string.Format("{0}_settings", name));
            TabablzControl tab_control = new TabablzControl();
            this.TabItem.Content = layout;

            InterTabController inter_tab_contorller = new InterTabController();
            //inter_tab_contorller.InterTabClient.
            //Binding binding = new Binding("InterTabClient");
            //binding.Source = inter_tab_contorller;
            //inter_tab_contorller.SetBinding()
             tab_control.InterTabController = inter_tab_contorller;

            layout.Content = tab_control;

            TabItem item = new TabItem();
            item.Header = "1";
            tab_control.Items.Add(item);

            TabItem item1 = new TabItem();
            item1.Header = "2";
            tab_control.Items.Add(item1);
        }

        private void drop(object sender, DragEventArgs e)
        {
            Console.WriteLine(sender.ToString());
        }


        Tab settings_tab;
        List<Tab> tabs = new List<Tab>();

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
