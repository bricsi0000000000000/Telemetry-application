using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ART_TELEMETRY_APP.Groups.Classes
{
    /// <summary>
    /// Stores all <see cref="Group"/>s.
    /// </summary>
    static class GroupManager
    {
        /// <summary>
        /// List of <see cref="Group"/>s.
        /// </summary>
        public static List<Group> Groups { get; } = new List<Group>();

        /// <summary>
        /// Initializes the groups from file.
        /// </summary>
        /// <param name="errorSnackbar"><see cref="Snackbar"/> that shows error message.</param>
        public static void InitGroups(ref Snackbar errorSnackbar)
        {
            if (File.Exists(TextManager.GroupsFileName))
            {
                ReadGroups(ref errorSnackbar);
            }
            else
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, string.Format($"Couldn't load groups, because file '{TextManager.GroupsFileName}' not found!"), 3);
            }
        }

        /// <summary>
        /// Reads groups from file.
        /// </summary>
        private static void ReadGroups(ref Snackbar errorSnackbar)
        {
            using var reader = new StreamReader(TextManager.GroupsFileName);

            try
            {
                dynamic groupsJSON = JsonConvert.DeserializeObject(reader.ReadToEnd());

                for (int i = 0; i < groupsJSON.Count; i++)
                {
                    string groupName = groupsJSON[i].Name.ToString();
                    if (!groupName.Equals(string.Empty))
                    {
                        var group = new Group(groupName)
                        {
                            Customizable = groupsJSON[i].Customizable,
                            Driverless = groupsJSON[i].Driverless
                        };

                        for (int j = 0; j < groupsJSON[i].Attributes.Count; j++)
                        {
                            group.AddAttribute(groupsJSON[i].Attributes[j].ToString());
                        }
                        AddGroup(group);
                    }
                    else
                    {
                        ShowError.ShowErrorMessage(ref errorSnackbar, "Can't add group, because the name is empty!");
                    }
                }
            }
            catch (JsonReaderException)
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, $"There was a problem reading {TextManager.GroupsFileName}");
            }

        }

        /// <summary>
        /// Saves groups to file.
        /// </summary>
        public static void SaveGroups()
        {
            using var writer = new StreamWriter(TextManager.GroupsFileName);

            var serializer = new JsonSerializer();
            serializer.Serialize(writer, Groups);
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
