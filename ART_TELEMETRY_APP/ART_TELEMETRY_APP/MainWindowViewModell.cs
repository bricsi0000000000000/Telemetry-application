using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class MainWindowViewModell
    {
        public IInterTabClient InterTabClient { get; set; }
        public MainWindowViewModell()
        {
            InterTabClient = new MyInterTabClient();
        }
    }
}
