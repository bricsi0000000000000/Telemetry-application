using System;
using System.Collections.Generic;
using System.Text;
using DataLayer.Groups;

namespace DataLayer.InputFiles
{
    public class DriverlessInputFile : InputFile
    {
        public DriverlessInputFile(int id, string name, List<Channel> channels) : base(id, name, channels)
        {
            InitRequiredChannels();
            Driverless = true;
        }

        public DriverlessInputFile(InputFile inputFile) : base(inputFile)
        {
            InitRequiredChannels();
            Driverless = true;
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

