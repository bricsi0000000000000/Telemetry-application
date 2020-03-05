﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        public Tab(string name)
        {
            this.name = name;
            tab_item = new TabItem();
            tab_item.Header = name;
            tab_item.Name = string.Format("{0}_tab_item", name);
        }

        Grid charts_grid;
    }
}