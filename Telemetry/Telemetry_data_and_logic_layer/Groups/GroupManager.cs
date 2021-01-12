using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Telemetry_data_and_logic_layer.Texts;

namespace Telemetry_data_and_logic_layer.Groups
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

            ReadGroups(fileName);
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
                        throw new Exception("Can't add group, because the name is null!");
                    }

                    if (groupsJSON[i].Name.ToString().Equals(string.Empty))
                    {
                        throw new Exception("Can't add group, because the name is empty!");
                    }

                    if (groupsJSON[i].Driverless == null)
                    {
                        throw new Exception("Can't add group, because driverless is null!");
                    }

                    if (groupsJSON[i].Customizable == null)
                    {
                        throw new Exception("Can't add group, because customizable is null!");
                    }

                    if (groupsJSON[i].Attributes == null)
                    {
                        throw new Exception("Can't add group, because attributes are null!");
                    }
                    else
                    {
                        var group = new Group(groupsJSON[i].Name.ToString())
                        {
                            Driverless = groupsJSON[i].Driverless,
                            Customizable = groupsJSON[i].Customizable
                        };

                        for (int j = 0; j < groupsJSON[i].Attributes.Count; j++)
                        {
                            string attributeName = "";
                            string attributeColor = "";

                            if (groupsJSON[i].Attributes[j].Name == null)
                            {
                                throw new Exception("Can't add attribute, because name is null!");
                            }
                            attributeName = groupsJSON[i].Attributes[j].Name.ToString();

                            if (groupsJSON[i].Attributes[j].Color == null)
                            {
                                throw new Exception("Can't add attribute, because color is null!");
                            }
                            attributeColor = groupsJSON[i].Attributes[j].Color.ToString();

                            if (!attributeName.Equals(string.Empty) &&
                                !attributeColor.Equals(string.Empty))
                            {
                                group.AddAttribute(attributeName, attributeColor);
                            }
                        }

                        AddGroup(group);
                    }

                }
            }
            catch (JsonReaderException)
            {
                throw new Exception($"There was a problem reading {TextManager.GroupsFileName}");
            }
        }

        /// <summary>
        /// Saves groups to file.
        /// </summary>
        public static void SaveGroups()
        {
            if (!File.Exists(TextManager.GroupsFileName))
            {
                throw new Exception($"Can't save groups because {TextManager.GroupsFileName} not found!");
            }

            using var writer = new StreamWriter(TextManager.GroupsFileName);

            var serializer = new JsonSerializer();

            try
            {
                serializer.Serialize(writer, Groups);
            }
            catch (Exception)
            {
                throw new Exception($"Can't save file!");
            }
        }

        /// <summary>
        /// Add a <see cref="Group"/> to <see cref="Groups"/>.
        /// </summary>
        /// <param name="group">The group that you want to add to <see cref="Groups"/>.</param>
        public static void AddGroup(Group group) => Groups.Add(group);

        /// <summary>
        /// Finds a <see cref="Group"/> based on <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Findable <see cref="Group"/>s name.</param>
        /// <returns>A <see cref="Group"/>.</returns>
        public static Group GetGroup(string name) => Groups.Find(x => x.Name.Equals(name));

        /// <summary>
        /// Removes a <see cref="Group"/> from <see cref="Groups"/>.
        /// </summary>
        /// <param name="name">Removable <see cref="Group"/>s name.</param>
        public static void RemoveGroup(string name) => Groups.Remove(GetGroup(name));
    }
}
