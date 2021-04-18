using DataLayer.Groups;
using System.Collections.Generic;

namespace DataLayer.InputFiles
{
    public class LiveInputFile : InputFile
    {
        public LiveInputFile(InputFile inputFile) : base(inputFile)
        {
            InputFileType = InputFileTypes.live;
        }

        public LiveInputFile(int id, string name, List<Channel> channels) : base(id, name, channels)
        {
            InputFileType = InputFileTypes.live;
        }
    }
}
