using System;
using System.Collections.Generic;
using System.Text;

namespace Telemetry_data_and_logic_layer.Drivers
{
    public class Driver
    {
        public string Name { get; }
        public bool IsSelected { get; set; }

        public Driver(string name)
        {
            Name = name;
            IsSelected = false;
        }
    }
}
