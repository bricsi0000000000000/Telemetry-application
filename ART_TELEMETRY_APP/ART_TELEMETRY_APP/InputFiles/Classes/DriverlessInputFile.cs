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
            InitRequiredChannels();
        }
      
        public DriverlessInputFile(InputFile inputFile) : base(inputFile)
        {
            InitRequiredChannels();
        }

        private void InitRequiredChannels()
        {
            RequiredChannels = new Dictionary<string, bool>();

            foreach (var item in ImportantChannels.DriverlessImportantChannelNames)
            {
                RequiredChannels.Add(item, false);
            }
        }

        /*    public override Dictionary<string, bool> RequiredChannels
            {
                get
                {
                    requiredChannels = new Dictionary<string, bool>();

                    foreach (var item in ImportantChannels.DriverlessImportantChannelNames)
                    {
                        requiredChannels.Add(item, false);
                    }

                    return requiredChannels;
                }
            }*/
    }
}
