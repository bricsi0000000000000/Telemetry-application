using System.Collections.Generic;
using System.IO;

namespace ART_TELEMETRY_APP.Groups.Classes
{
    static class GroupManager
    {
        public static List<Group> Groups { get; } = new List<Group>();

        public static void InitGroups()
        {
            StreamReader stream_reader = new StreamReader("groups.csv");
            stream_reader.ReadLine();
            while (!stream_reader.EndOfStream)
            {
                string[] row = stream_reader.ReadLine().Split(';');
                Group group = new Group(row[0]);
                string[] attributes = row[1].Split(',');
                foreach (string attribute in attributes)
                {
                    group.AddAttribute(attribute);
                }
                AddGroup(group);
            }
            stream_reader.Close();
        }

        public static void AddGroup(Group group) => Groups.Add(group);

        public static Group GetGroup(string name) => Groups.Find(n => n.Name.Equals(name));

        public static void RemoveGroup(string name) => Groups.Remove(GetGroup(name));
    }
}
