using ART_TELEMETRY_APP.Groups.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Interaction logic for GroupSettings.xaml
    /// </summary>
    public partial class GroupSettings : UserControl
    {
        List<GroupSettingsGroup> group_settings_groups = new List<GroupSettingsGroup>();
        List<GroupSettingsAttribute> group_settings_attributes = new List<GroupSettingsAttribute>();
        string active_group_name = "";
        string active_attribute = "";

        public GroupSettings()
        {
            InitializeComponent();

            if (active_group_name.Equals(""))
            {
                active_group_name = GroupManager.Groups.First().Name;
            }

            InitGroups();
        }

        public void InitGroups()
        {
            groups_stackpanel.Children.Clear();

            foreach (Group group in GroupManager.Groups)
            {
                GroupSettingsGroup content = new GroupSettingsGroup(group.Name);
                content.ChangeColorMode(group.Name == active_group_name);
                groups_stackpanel.Children.Add(content);
                group_settings_groups.Add(content);
            }

            if (GroupManager.GetGroup(active_group_name).Attributes.Count > 0)
            {
                active_attribute = GroupManager.GetGroup(active_group_name).Attributes.First();
            }

            InitAttributes();
        }

        public void InitAttributes()
        {
            attributes_stackpanel.Children.Clear();

            foreach (string attribute in GroupManager.GetGroup(active_group_name).Attributes)
            {
                GroupSettingsAttribute content = new GroupSettingsAttribute(attribute, active_group_name);
                content.ChangeColorMode(attribute == active_attribute);
                attributes_stackpanel.Children.Add(content);
                group_settings_attributes.Add(content);
            }
        }

        public GroupSettingsGroup GetGroupSettingsContent(string name)
        {
            return group_settings_groups.Find(n => n.Name.Equals(name));
        }

        public GroupSettingsAttribute GetGroupSettingsAttributes(string name)
        {
            return group_settings_attributes.Find(n => n.Name.Equals(name));
        }

        public string ActiveGroupName
        {
            get
            {
                return active_group_name;
            }
            set
            {
                active_group_name = value;
            }
        }

        public string ActiveAttribute
        {
            get
            {
                return active_attribute;
            }
            set
            {
                active_attribute = value;
            }
        }

        private void addGroup_Click(object sender, RoutedEventArgs e)
        {
            Group group = new Group(addGroup_txtbox.Text);
            GroupManager.AddGroup(group);
            active_group_name = addGroup_txtbox.Text;
            addGroup_txtbox.Text = "";
            InitGroups();
        }

        private void addAttribute_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.GetGroup(active_group_name).AddAttribute(addAttribute_txtbox.Text);
            active_attribute = addAttribute_txtbox.Text;
            addAttribute_txtbox.Text = "";
            InitGroups();
        }
    }
}
