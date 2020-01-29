using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class Groups
    {
        List<Group> groups = new List<Group>();
        string active_group;

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

        public void AddGroup(string name)
        {
            groups.Add(new Group(name));
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

        public List<string> GetGroupAttributes(string name)
        {
            foreach (Group group in groups)
            {
                if (name == group.Name)
                {
                    return group.Attributes;
                }
            }

            return null;
        }

        public void AddAttributeToGroup(string attribute)
        {
            groups.Find(n => n.Name == active_group).Attributes.Add(attribute);

            foreach (var item in groups)
            {
                if (item.Name == active_group)
                {
                    Trace.WriteLine(active_group);
                    foreach (var r in item.Attributes)
                    {
                        Trace.WriteLine(r);
                    }
                }
            }
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
    }
}
