using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogicLayer.Configurations
{
    public static class ConfigurationManager
    {
        #region version
        private static int majorVersion;
        private static int minorVersion;
        public static string Version => $"{majorVersion}.{minorVersion}";
        #endregion

        #region live
        /// <summary>
        /// in milliseconds
        /// </summary>
        public static int WaitBerweenTries { get; private set; }
        public static bool IsHTTPS { get; private set; }
        public static string URL { get; private set; }
        public static int Port { get; private set; }
        public static string Address => string.Format("{0}://{1}:{2}/", (IsHTTPS ? "https" : "http"), URL, Port);

        #region API calls
        public static string LiveSectionAPICall { get; private set; }
        public static string AllLiveSectionsAPICall { get; private set; }
        public static string PostNewSectionAPICall { get; private set; }
        public static string ChangeSectionToLiveAPICall { get; private set; }
        public static string ChangeSectionToOfflineAPICall { get; private set; }
        public static string ChangeSectionNameAPICall { get; private set; }
        public static string ChangeSectionDateAPICall { get; private set; }
        public static string DeleteSectionAPICall { get; private set; }
        #endregion
        #endregion

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

                #region version
                majorVersion = configurationJSON.version.major_version;
                minorVersion = configurationJSON.version.minor_version;
                #endregion

                #region live
                WaitBerweenTries = configurationJSON.live.wait_between_tries;
                IsHTTPS = configurationJSON.live.isHTTPS;
                URL = configurationJSON.live.url;
                Port = configurationJSON.live.port;
                LiveSectionAPICall = configurationJSON.live.get_live_section_api_call;
                AllLiveSectionsAPICall = configurationJSON.live.get_all_sections_api_call;
                PostNewSectionAPICall = configurationJSON.live.post_new_section_api_call;
                ChangeSectionToLiveAPICall = configurationJSON.live.change_section_to_live_api_call;
                ChangeSectionToOfflineAPICall = configurationJSON.live.change_section_to_offline_api_call;
                ChangeSectionNameAPICall = configurationJSON.live.change_section_name_api_call;
                ChangeSectionDateAPICall = configurationJSON.live.change_section_date_api_call;
                DeleteSectionAPICall = configurationJSON.live.delete_section_call;
                #endregion
            }
            catch (JsonReaderException)
            {
                throw new Exception($"There was a problem reading '{fileName}'");
            }
        }
    }
}
