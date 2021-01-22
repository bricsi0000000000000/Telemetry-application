using System;
using System.Collections.Generic;
using System.Text;
using Telemetry_data_and_logic_layer.Groups;

namespace Telemetry_data_and_logic_layer.InputFiles
{
    public class DriverlessInputFile : InputFile
    {
        public DriverlessInputFile(string name, List<Channel> channels) : base(name, channels)
        {
            InitRequiredChannels();
            Driverless = true;
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

