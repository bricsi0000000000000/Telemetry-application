using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Maps
{
    class MapManager
    {
        static List<Map> maps = new List<Map>();

        public static Map GetMap(string name)
        {
            return maps.Find(n => n.Name == name);
        }
    }
}
