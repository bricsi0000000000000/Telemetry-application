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
    class Groups
    {
        #region instance
        private static Groups instance = null;
        private Groups() { }

        public static Groups Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Groups();
                }
                return instance;
            }
        }
        #endregion

        List<Group> groups = new List<Group>();
        string active_group;
        TreeViewItem active_tree_view_item;

        public void AddGroup(string name)
        {
            groups.Add(new Group(name));
        }

        public Group GetGroup(string name = "")
        {
            if (name == "")
            {
                name = active_group;
            }
            return groups.Find(n => n.Name == name);
        }

        public List<string> GetGroupsNames
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

        public List<string> GetGroupAttributes(string name = "")
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
                        return group.Attributes;
                    }
                }
            }

            return null;
        }

        public void AddAttributeToGroup(string attribute)
        {
            if ((groups.Find(n => n.Name == active_group).Attributes.Find(attr => attr == attribute)) == null)
            {
                groups.Find(n => n.Name == active_group).AddAttribute(attribute);
            }
        }

        public void RemoveAttributeFromGroup(string attribute)
        {
            groups.Find(n => n.Name == active_group).Attributes.Remove(attribute);
        }

        public string ActiveGroup
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

        public int GroupsCount
        {
            get
            {
                return groups.Count;
            }
        }

        public List<Group> GetGroups
        {
            get
            {
                return groups;
            }
        }

        public TreeViewItem ActiveGroupTreeViewItem
        {
            get
            {
                return active_tree_view_item;
            }
            set
            {
                active_tree_view_item = value;
            }
        }
    }
}
