using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    public class WorkspaceNewItem
    {
        public static Func<HeaderedItemViewModel> Factory
        {
            get
            {
                return
                    () =>
                    {
                        var dateTime = DateTime.Now;

                        return new HeaderedItemViewModel()
                        {
                            Header = "helo",
                            Content = dateTime.ToString("R")
                        };
                    };
            }
        }
    }
}
