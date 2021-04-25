using Newtonsoft.Json;
using System;
using System.IO;

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
        public static int WaitBetweenCollectData { get; private set; }
        public static bool IsHTTPS { get; private set; }
        public static string URL { get; private set; }
        public static int Port { get; private set; }
        public static string Address => string.Format("{0}://{1}:{2}/", (IsHTTPS ? "https" : "http"), URL, Port);

        #region API calls

        #region sections
        public static string LiveSectionAPICall { get; private set; }
        public static string AllLiveSectionsAPICall { get; private set; }
        public static string PostNewSectionAPICall { get; private set; }
        public static string ChangeSectionToLiveAPICall { get; private set; }
        public static string ChangeSectionToOfflineAPICall { get; private set; }
        public static string ChangeSectionNameAPICall { get; private set; }
        public static string ChangeSectionDateAPICall { get; private set; }
        public static string DeleteSectionAPICall { get; private set; }
        #endregion

        #region packages
        public static string GetPackageByIDAPICall { get; private set; }

        #endregion

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

                WaitBetweenCollectData = configurationJSON.live.wait_between_collect_data;
                IsHTTPS = configurationJSON.live.isHTTPS;
                URL = configurationJSON.live.url;
                Port = configurationJSON.live.port;

                LiveSectionAPICall = configurationJSON.live.sections.get_live_section;
                AllLiveSectionsAPICall = configurationJSON.live.sections.get_all_sections;
                PostNewSectionAPICall = configurationJSON.live.sections.post_new_section;
                ChangeSectionToLiveAPICall = configurationJSON.live.sections.change_section_to_live;
                ChangeSectionToOfflineAPICall = configurationJSON.live.sections.change_section_to_offline;
                ChangeSectionNameAPICall = configurationJSON.live.sections.change_section_name;
                ChangeSectionDateAPICall = configurationJSON.live.sections.change_section_date;
                DeleteSectionAPICall = configurationJSON.live.sections.delete_section;

                GetPackageByIDAPICall = configurationJSON.live.packages.get_package_by_id;

                #endregion
            }
            catch (JsonReaderException)
            {
                throw new Exception($"There was a problem reading '{fileName}'");
            }
        }
    }
}
