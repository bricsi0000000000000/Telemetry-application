using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogicLayer.Configurations
{
    public static class ConfigurationManager
    {
        public static int MajorVersion { get; private set; }
        public static int MinorVersion { get; private set; }
        public static string Version => $"{MajorVersion}.{MinorVersion}";

        public static void LoadConfigurations(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception($"Couldn't load configurations, because file '{fileName}' not found!");
            }

            if (new FileInfo(fileName).Length == 0)
            {
                throw new Exception($"'{fileName}' is empty");
            }

            using var reader = new StreamReader(fileName);

            try
            {
                dynamic configurationJSON = JsonConvert.DeserializeObject(reader.ReadToEnd());

                MajorVersion = configurationJSON.major_version;
                MinorVersion = configurationJSON.minor_version;
            }
            catch (JsonReaderException)
            {
                throw new Exception($"There was a problem reading '{fileName}'");
            }
        }
    }
}
