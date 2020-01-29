using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP
{
    class GroupBuilder
    {
        public GroupBuilder(string group_name)
        {
            Groups.Instance.AddGroup(group_name);
        }
    }
}
