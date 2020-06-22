using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Groups.Classes
{
    static class GroupManager
    {
        static List<Group> groups = new List<Group>();

        public static void InitGroups()
        {
            StreamReader sr = new StreamReader("groups.csv");
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] row = sr.ReadLine().Split(';');
                Group group = new Group(row[0]);
                string[] attributes = row[1].Split(',');
                foreach (string attribute in attributes)
                {
                    group.AddAttribute(attribute);
                }
                AddGroup(group);
            }
            sr.Close();
        }

        public static void AddGroup(Group group)
        {
            groups.Add(group);
        }

        public static Group GetGroup(string name)
        {
            return groups.Find(n => n.Name == name);
        }

        public static void RemoveGroup(string name)
        {
            groups.Remove(GetGroup(name));
        }

        public static List<Group> Groups
        {
            get
            {
                return groups;
            }
        }
    }
}
