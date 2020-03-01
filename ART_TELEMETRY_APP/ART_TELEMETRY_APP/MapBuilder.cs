using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    class MapBuilder
    {
        #region instance
        private static MapBuilder instance = null;
        private MapBuilder() { }

        public static MapBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MapBuilder();
                }
                return instance;
            }
        }
        #endregion

        List<Map> maps = new List<Map>();
        public void Build(Path map_svg_path, ColorZone map_nothing = null)
        {
            map_svg_path.Data = Geometry.Parse(GetMap().SvgPathes[Datas.Instance.GetData().ActLap++].Item1);

            if (map_nothing != null)
            {
                map_nothing.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public void Make(string mape_name,
                         ProgressBar map_progressbar,
                         ColorZone map_progressbar_colorzone,
                         Path map_svg,
                         ColorZone map_nothing
                        )
        {
            Map map = new Map(mape_name,
                              map_progressbar,
                              map_progressbar_colorzone,
                              map_svg,
                              map_nothing
                              );
            maps.Add(map);
        }

        public Map GetMap(string name = "")
        {
            if (name.Equals(""))
            {
                return maps.Find(n => n.Name == Datas.Instance.ActiveFileName);
            }
            else
            {
                return maps.Find(n => n.Name == name);
            }
        }
    }
}
