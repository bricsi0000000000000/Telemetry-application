using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ART_TELEMETRY_APP
{
    class ButtonCommands
    {
        public static readonly RoutedUICommand ImportFile = new RoutedUICommand
          (
              "Import File",
              "ImportFile",
              typeof(SwitchForms),
              new InputGestureCollection()
              {
                    new KeyGesture(Key.O, ModifierKeys.Control)
              }
          );
    }
}
