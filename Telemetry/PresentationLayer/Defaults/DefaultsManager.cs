using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using DataLayer.Defaults;
using LocigLayer.Texts;
using PresentationLayer.Extensions;
using System.Drawing;

namespace PresentationLayer.Defaults
{
    public static class DefaultsManager
    {
        public static List<Default> Defaults = new List<Default>();

        public static void LoadDefaults(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception($"Couldn't load default settings, because file '{fileName}' not found!");
            }

            if (new FileInfo(fileName).Length == 0)
            {
                throw new Exception($"'{fileName}' is empty");
            }

            ReadDefaults(fileName);
        }

        /// <summary>
        /// Reads defaults from file.
        /// </summary>
        /// <param name="fileName">File name from read defaults.</param>
        private static void ReadDefaults(string fileName)
        {
            using var reader = new StreamReader(fileName);

            try
            {
                dynamic JSON = JsonConvert.DeserializeObject(reader.ReadToEnd());

                for (int i = 0; i < JSON.Count; i++)
                {
                    if (JSON[i].Name == null)
                    {
                        throw new Exception("Can't add default settings, because 'name' is null!");
                    }

                    if (JSON[i].Name.ToString().Equals(string.Empty))
                    {
                        throw new Exception("Can't add default settings, because 'name' is empty!");
                    }

                    if (JSON[i].Value == null)
                    {
                        throw new Exception("Can't add default settings, because 'value' is null!");
                    }

                    if (JSON[i].Value.ToString().Equals(string.Empty))
                    {
                        throw new Exception("Can't add default settings, because 'value' is empty!");
                    }

                    AddDefault(new Default(JSON[i].Name.ToString(), JSON[i].Value.ToString()));
                }
            }
            catch (JsonReaderException)
            {
                throw new Exception($"There was a problem reading '{TextManager.DefaultFileName}'");
            }
        }

        /// <summary>
        /// Saves default settings to file.
        /// </summary>
        public static void SaveDefaults()
        {
            string fileName = TextManager.DefaultFileName;
            if (!File.Exists(fileName))
            {
                throw new Exception($"Can't save default settings because '{TextManager.DefaultFileName}' not found!");
            }

            using var writer = new StreamWriter(fileName);

            var serializer = new JsonSerializer();

            try
            {
                serializer.Serialize(writer, Defaults);
            }
            catch (Exception)
            {
                throw new Exception("Can't save default settings file!");
            }
        }

        /// <summary>
        /// Add a <see cref="Default"/> to <see cref="Defaults"/>.
        /// </summary>
        /// <param name="defaultSettings">The group that you want to add to <see cref="Groups"/>.</param>
        public static void AddDefault(Default defaultSettings) => Defaults.Add(defaultSettings);

        /// <summary>
        /// Finds a <see cref="Default"/> based on <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="Default"/>s name.</param>
        /// <returns>A <see cref="Default"/>.</returns>
        public static Default GetDefault(string name) => Defaults.Find(x => x.Name.Equals(name));

        /// <summary>
        /// Removes a <see cref="Group"/> from <see cref="Groups"/>.
        /// </summary>
        /// <param name="name">Removable <see cref="Group"/>s name.</param>
        public static void RemoveDefault(string name) => Defaults.Remove(GetDefault(name));

        public static Color DefaultChartHighlightColor => GetDefault("DefaultChartHighlightColor").Value.ConvertToChartColor();
    }
}