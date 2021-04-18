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
            InputFileType = InputFileTypes.driverless;
        }

        public DriverlessInputFile(InputFile inputFile) : base(inputFile)
        {
            InitRequiredChannels();
            InputFileType = InputFileTypes.driverless;
        }

        private void InitRequiredChannels()
        {
            RequiredChannels = new Dictionary<string, bool>();

            foreach (var item in ImportantChannels.DriverlessImportantChannelNames)
            {
                RequiredChannels.Add(item, false);
            }
        }
    }
}

