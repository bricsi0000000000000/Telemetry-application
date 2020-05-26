﻿using ART_TELEMETRY_APP.Pilots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// Interaction logic for PilotContentTabs.xaml
    /// </summary>
    public partial class PilotContentTab : UserControl
    {
        List<TabItem> tabs = new List<TabItem>();
        Pilot pilot;

        public PilotContentTab(Pilot pilot)
        {
            InitializeComponent();

            this.pilot = pilot;

            initTabs();
        }

        private void initTabs()
        {
            TabItem item = new TabItem();
            item.Header = "Laps";
            item.Content = new LapsContent(pilot);
            item.Name = string.Format("{0}_item_laps", pilot.Name);
            tabs.Add(item);
            tabcontrol.Items.Add(item);

            item = new TabItem();
            item.Header = "Track";
            item.Content = new TrackContent();
            item.Name = string.Format("{0}_item_track", pilot.Name);
            tabs.Add(item);
            tabcontrol.Items.Add(item);

            item = new TabItem();
            item.Header = "Traction";
            item.Content = new GGDiagram_UC();
            item.Name = string.Format("{0}_item_traction", pilot.Name);
            tabs.Add(item);
            tabcontrol.Items.Add(item);

            item = new TabItem();
            item.Header = "LapReport";
            //item.Content = new LapsContent(pilot);
            item.Name = string.Format("{0}_item_lapreport", pilot.Name);
            tabs.Add(item);
            tabcontrol.Items.Add(item);
        }

        public TabItem GetTab(string name)
        {
            return tabs.Find(n => n.Header.Equals(name));
        }
    }
}