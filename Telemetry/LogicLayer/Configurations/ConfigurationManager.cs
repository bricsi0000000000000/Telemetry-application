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

        #region sessions
        public static string LiveSessionAPICall { get; private set; }
        public static string AllLiveSessionsAPICall { get; private set; }
        public static string PostNewSessionAPICall { get; private set; }
        public static string ChangeSessionToLiveAPICall { get; private set; }
        public static string ChangeSessionToOfflineAPICall { get; private set; }
        public static string ChangeSessionNameAPICall { get; private set; }
        public static string ChangeSessionDateAPICall { get; private set; }
        public static string DeleteSessionAPICall { get; private set; }
        #endregion

        #region packages
        public static string GetPackageByID_APICall { get; private set; }
        public static string GetPackagesAfter_APICall { get; private set; }
        public static string GetAllPackages_APICall { get; private set; }

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

                LiveSessionAPICall = configurationJSON.live.sessions.get_live_session;
                AllLiveSessionsAPICall = configurationJSON.live.sessions.get_all_sessions;
                PostNewSessionAPICall = configurationJSON.live.sessions.post_new_session;
                ChangeSessionToLiveAPICall = configurationJSON.live.sessions.change_session_to_live;
                ChangeSessionToOfflineAPICall = configurationJSON.live.sessions.change_session_to_offline;
                ChangeSessionNameAPICall = configurationJSON.live.sessions.change_session_name;
                ChangeSessionDateAPICall = configurationJSON.live.sessions.change_session_date;
                DeleteSessionAPICall = configurationJSON.live.sessions.delete_session;

                GetPackageByID_APICall = configurationJSON.live.packages.get_package_by_id;
                GetPackagesAfter_APICall = configurationJSON.live.packages.get_packages_after;
                GetAllPackages_APICall = configurationJSON.live.packages.get_all_packages;

                #endregion
            }
            catch (JsonReaderException)
            {
                throw new Exception($"There was a problem reading '{fileName}'");
            }
        }
    }
}
