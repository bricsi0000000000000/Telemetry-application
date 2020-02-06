using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ART_TELEMETRY_APP
{
    public class SwitchForms
    {
        public static readonly RoutedUICommand SwitchToGroups = new RoutedUICommand
            (
                "Switch To Groups",
                "SwitchToGroups",
                typeof(SwitchForms),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.G, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand SwitchToCharts = new RoutedUICommand
          (
              "Switch To Charts",
              "SwitchToCharts",
              typeof(SwitchForms),
              new InputGestureCollection()
              {
                    new KeyGesture(Key.D, ModifierKeys.Control)
              }
          );
    }
}
