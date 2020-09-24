using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.IO;

namespace ART_TELEMETRY_APP.Groups.Classes
{
    static class GroupManager
    {
        public static List<Group> Groups { get; } = new List<Group>();

        public static void InitGroups(ref Snackbar errorSnackbar)
        {
            if (File.Exists(TextManager.GroupsFileName))
            {
                ReadGroups();
            }
            else
            {
                ShowError.ShowErrorMessage(ref errorSnackbar, string.Format("Couldn't load groups, because file '{0}' not found!", TextManager.GroupsFileName), 3);
            }
        }

        private static void ReadGroups()
        {
            using var reader = new StreamReader(TextManager.GroupsFileName);
            while (!reader.EndOfStream)
            {
                string[] row = reader.ReadLine().Split(';');
                Group group = new Group(row[0]);
                string[] attributes = row[1].Split(',');
                foreach (string attribute in attributes)
                {
                    group.AddAttribute(attribute);
                }
                AddGroup(group);
            }
        }

        public static void SaveGroups()
        {
            using var writer = new StreamWriter(TextManager.GroupsFileName);

            foreach (var group in Groups)
            {
                string attributes = "";
                foreach (var attribute in group.Attributes)
                {
                    attributes += attribute + ",";
                }
                writer.WriteLine("{0};{1}", group.Name, attributes.Length > 0 ? attributes.Substring(0, attributes.Length - 1) : string.Empty);
            }
        }

        public static void AddGroup(Group group) => Groups.Add(group);

        public static Group GetGroup(string name) => Groups.Find(x => x.Name.Equals(name));

        public static void RemoveGroup(string name) => Groups.Remove(GetGroup(name));
    }
}
