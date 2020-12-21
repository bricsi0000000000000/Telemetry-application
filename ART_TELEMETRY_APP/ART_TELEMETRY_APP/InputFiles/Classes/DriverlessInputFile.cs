using ART_TELEMETRY_APP.Datas.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    public class DriverlessInputFile : InputFile
    {
        public DriverlessInputFile(string name, List<Channel> channels) : base(name, channels)
        {

        }
    }
}
