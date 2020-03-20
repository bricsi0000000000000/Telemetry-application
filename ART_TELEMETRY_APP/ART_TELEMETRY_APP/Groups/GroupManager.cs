using ART_TELEMETRY_APP.Settings;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    static class GroupManager
    {
        static List<Group> groups = new List<Group>();
        static string active_group;

        public static void AddGroup(Group group)
        {
            groups.Add(group);
            active_group = group.Name;
        }

        public static Group GetGroup(string name = "")
        {
            if (name == "")
            {
                name = active_group;
            }
            return groups.Find(n => n.Name == name);
        }

        public static void DeleteGroup(string name = "")
        {
            if (name == "")
            {
                name = active_group;
            }

            groups.Remove(GetGroup(name));
        }

        public static void ChangeGroupName(string name, string new_name)
        {
            GetGroup(name).Name = new_name;
        }

        public static List<string> GetGroupsNames
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Group group in groups)
                {
                    names.Add(group.Name);
                }

                return names;
            }
        }

        public static List<SelectedChannelSettings_UC> GetGroupAttributes(string name = "")
        {
            if (name == "")
            {
                return GetGroupAttributes(active_group);
            }
            else
            {
                foreach (Group group in groups)
                {
                    if (name == group.Name)
                    {
                       // return group.SelectedChannelSettingsUCs;
                    }
                }
            }

            return null;
        }

        public static void AddAttributeToGroup(string attribute)
        {
            /*if ((groups.Find(n => n.Name == active_group).SelectedChannelSettingsUCs.Find(attr => attr.Attribute == attribute)) == null)
            {
                groups.Find(n => n.Name == active_group).SelectedChannelSettingsUCs.Add(new Settings.SelectedChannelSettings_UC(attribute));
            }*/
        }

        public static void RemoveAttributeFromGroup(string attribute)
        {
            foreach (Group group in groups)
            {
                if (group.Name == active_group)
                {
                  /*  foreach (SelectedChannelSettings_UC item in group.SelectedChannelSettingsUCs)
                    {
                        if (item.Attribute == attribute)
                        {
                            group.SelectedChannelSettingsUCs.Remove(item);
                        }
                    }*/
                }
            }
        }

        public static string ActiveGroup
        {
            get
            {
                return active_group;
            }
            set
            {
                active_group = value;
            }
        }

        public static int GroupsCount
        {
            get
            {
                return groups.Count;
            }
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
