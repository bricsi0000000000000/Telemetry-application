using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using DataLayer.Groups;
using PresentationLayer.Texts;
using LogicLayer.Colors;

namespace PresentationLayer.Groups
{
    /// <summary>
    /// Stores all <see cref="Group"/>s.
    /// </summary>
    public static class GroupManager
    {
        /// <summary>
        /// All <see cref="Group"/>s whose the user can use in the program.
        /// </summary>
        public static List<Group> Groups { get; } = new List<Group>();

        public static int LastGroupID = 0;

        private static string groupsFileNameWithPath;

        /// <summary>
        /// Initializes groups from file.
        /// </summary>
        /// <param name="fileName">File name from read groups.</param>
        public static void InitGroups(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception($"Couldn't load groups, because file '{fileName}' not found!");
            }

            if (new FileInfo(fileName).Length == 0)
            {
                throw new Exception($"'{fileName}' is empty");
            }

            groupsFileNameWithPath = fileName;
            ReadGroups(groupsFileNameWithPath);
        }

        /// <summary>
        /// Reads groups from file.
        /// </summary>
        /// <param name="fileName">File name from read groups.</param>
        private static void ReadGroups(string fileName)
        {
            using var reader = new StreamReader(fileName);

            try
            {
                dynamic groupsJSON = JsonConvert.DeserializeObject(reader.ReadToEnd());

                for (int i = 0; i < groupsJSON.Count; i++)
                {
                    if (groupsJSON[i].Name == null)
                    {
                        throw new Exception("Can't add group, because 'name' is null!");
                    }

                    if (groupsJSON[i].Name.ToString().Equals(string.Empty))
                    {
                        throw new Exception("Can't add group, because 'name' is empty!");
                    }

                    if (groupsJSON[i].Attributes == null)
                    {
                        throw new Exception("Can't add group, because 'attributes' are null!");
                    }
                    else
                    {
                        var group = new Group(LastGroupID++, groupsJSON[i].Name.ToString());

                        for (int j = 0; j < groupsJSON[i].Attributes.Count; j++)
                        {
                            string attributeName = "";
                            string attributeColor = "";
                            int attributeLineWidth = 0;

                            if (groupsJSON[i].Attributes[j].Name == null)
                            {
                                throw new Exception("Can't add attribute, because 'name' is null!");
                            }

                            if (groupsJSON[i].Attributes[j].ColorText == null)
                            {
                                throw new Exception("Can't add attribute, because 'color' is null!");
                            }

                            if (groupsJSON[i].Attributes[j].LineWidth == null)
                            {
                                throw new Exception("Can't add attribute, because 'line width' is null!");
                            }

                            attributeName = groupsJSON[i].Attributes[j].Name.ToString();
                            attributeColor = groupsJSON[i].Attributes[j].ColorText.ToString();
                            attributeLineWidth = int.Parse(groupsJSON[i].Attributes[j].LineWidth.ToString());

                            if (!attributeName.Equals(string.Empty) &&
                                !attributeColor.Equals(string.Empty))
                            {
                                group.AddAttribute(attributeName, attributeColor, attributeLineWidth);
                            }
                            else
                            {
                                throw new Exception("Can't add attribute, because 'name' or/and 'color' are empty!");
                            }
                        }

                        AddGroup(group);
                    }

                }

                var temporaryGroup = GetGroup($"Temporary{TemporaryGroupIndex}");

                while (temporaryGroup != null)
                {
                    TemporaryGroupIndex++;
                    temporaryGroup = GetGroup($"Temporary{TemporaryGroupIndex}");
                }
            }
            catch (JsonReaderException)
            {
                throw new Exception($"There was a problem reading '{Texts.TextManager.GroupsFileName}'");
            }
        }

        /// <summary>
        /// Saves groups to file.
        /// </summary>
        public static void SaveGroups()
        {
            if (!File.Exists(groupsFileNameWithPath))
            {
                throw new Exception($"Can't save groups because '{Texts.TextManager.GroupsFileName}' not found!");
            }

            using var writer = new StreamWriter(groupsFileNameWithPath);

            var serializer = new JsonSerializer();

            try
            {
                serializer.Serialize(writer, Groups);
            }
            catch (Exception)
            {
                throw new Exception("Can't save file!");
            }
        }

        /// <summary>
        /// Add a <see cref="Group"/> to <see cref="Groups"/>.
        /// </summary>
        /// <param name="group">The group that you want to add to <see cref="Groups"/>.</param>
        public static void AddGroup(Group group) => Groups.Add(group);

        public static Group MakeGroupWithAttributes(string chartName, List<string> channelNames)
        {
            var newGroup = new Group(LastGroupID++, chartName);
            foreach (var name in channelNames)
            {
                newGroup.AddAttribute(name, ColorManager.GetChartColor.ToString(), 1);
            }

            return newGroup;
        }

        /// <summary>
        /// Finds a <see cref="Group"/> based on <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="Group"/>s name.</param>
        /// <returns>A <see cref="Group"/>.</returns>
        public static Group GetGroup(string name) => Groups.Find(x => x.Name.Equals(name));
        public static Group GetGroup(int id) => Groups.Find(x => x.ID == id);

        /// <summary>
        /// Removes a <see cref="Group"/> from <see cref="Groups"/>.
        /// </summary>
        /// <param name="name">Removable <see cref="Group"/>s name.</param>
        public static void RemoveGroup(string name) => Groups.Remove(GetGroup(name));
        public static void RemoveGroup(int id) => Groups.Remove(GetGroup(id));

        public static int TemporaryGroupIndex { get; set; }
    }
}
