using ART_TELEMETRY_APP.Groups.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Interaction logic for GroupSettings.xaml
    /// </summary>
    public partial class GroupSettings : UserControl
    {
        private readonly List<GroupSettingsGroup> group_settings_groups = new List<GroupSettingsGroup>();
        private readonly List<GroupSettingsAttribute> group_settings_attributes = new List<GroupSettingsAttribute>();

        public string ActiveGroupName { get; set; } = string.Empty;
        public string ActiveAttribute { get; set; } = string.Empty;

        public GroupSettings()
        {
            InitializeComponent();

            if (ActiveGroupName.Equals(string.Empty))
            {
                ActiveGroupName = GroupManager.Groups.First().Name;
            }

            InitGroups();
        }

        public void InitGroups()
        {
            groups_stackpanel.Children.Clear();

            foreach (Group group in GroupManager.Groups)
            {
                GroupSettingsGroup content = new GroupSettingsGroup(group.Name);
                content.ChangeColorMode(group.Name == ActiveGroupName);
                groups_stackpanel.Children.Add(content);
                group_settings_groups.Add(content);
            }

            if (GroupManager.GetGroup(ActiveGroupName).Attributes.Count > 0)
            {
                ActiveAttribute = GroupManager.GetGroup(ActiveGroupName).Attributes.First();
            }

            InitAttributes();
        }

        public void InitAttributes()
        {
            attributes_stackpanel.Children.Clear();

            foreach (string attribute in GroupManager.GetGroup(ActiveGroupName).Attributes)
            {
                GroupSettingsAttribute content = new GroupSettingsAttribute(attribute, ActiveGroupName);
                content.ChangeColorMode(attribute == ActiveAttribute);
                attributes_stackpanel.Children.Add(content);
                group_settings_attributes.Add(content);
            }
        }

        public GroupSettingsGroup GetGroupSettingsContent(string name) => group_settings_groups.Find(n => n.Name.Equals(name));

        public GroupSettingsAttribute GetGroupSettingsAttributes(string name) => group_settings_attributes.Find(n => n.Name.Equals(name));

        private void addGroup_Click(object sender, RoutedEventArgs e)
        {
            Group group = new Group(addGroup_txtbox.Text);
            GroupManager.AddGroup(group);
            ActiveGroupName = addGroup_txtbox.Text;
            addGroup_txtbox.Text = string.Empty;
            InitGroups();
        }

        private void addAttribute_Click(object sender, RoutedEventArgs e)
        {
            GroupManager.GetGroup(ActiveGroupName).AddAttribute(addAttribute_txtbox.Text);
            ActiveAttribute = addAttribute_txtbox.Text;
            addAttribute_txtbox.Text = string.Empty;
            InitGroups();
        }
    }
}
