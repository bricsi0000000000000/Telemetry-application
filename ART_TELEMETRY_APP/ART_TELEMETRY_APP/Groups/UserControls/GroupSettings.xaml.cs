using ART_TELEMETRY_APP.Errors.Classes;
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
        private readonly List<GroupSettingsItem> groupSettingsItems = new List<GroupSettingsItem>();
        private readonly List<GroupSettingsAttribute> groupSettingsAttributes = new List<GroupSettingsAttribute>();

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
            GroupsStackPanel.Children.Clear();

            foreach (var group in GroupManager.Groups)
            {
                var groupSettingsItem = new GroupSettingsItem(group.Name);
                groupSettingsItem.ChangeColorMode(group.Name.Equals(ActiveGroupName));
                GroupsStackPanel.Children.Add(groupSettingsItem);
                groupSettingsItems.Add(groupSettingsItem);
            }

            if (GroupManager.GetGroup(ActiveGroupName).Attributes.Count > 0)
            {
                ActiveAttribute = GroupManager.GetGroup(ActiveGroupName).Attributes.First();
            }

            InitAttributes();
        }

        public void InitAttributes()
        {
            AttributesStackPanel.Children.Clear();

            foreach (string attribute in GroupManager.GetGroup(ActiveGroupName).Attributes)
            {
                var groupSettingsAttribute = new GroupSettingsAttribute(attribute, ActiveGroupName);
                groupSettingsAttribute.ChangeColorMode(attribute.Equals(ActiveAttribute));
                AttributesStackPanel.Children.Add(groupSettingsAttribute);
                groupSettingsAttributes.Add(groupSettingsAttribute);
            }
        }

        public GroupSettingsItem GetGroupSettingsContent(string name) => groupSettingsItems.Find(n => n.Name.Equals(name));

        public GroupSettingsAttribute GetGroupSettingsAttributes(string name) => groupSettingsAttributes.Find(n => n.Name.Equals(name));

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            if (AddGroupTxtBox.Text.Equals(string.Empty))
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, "Group name is empty!");
                return;
            }

            var group = new Group(AddGroupTxtBox.Text);
            GroupManager.AddGroup(group);
            ActiveGroupName = AddGroupTxtBox.Text;
            AddGroupTxtBox.Text = string.Empty;
            InitGroups();

            GroupManager.SaveGroups();
        }

        private void AddAttribute_Click(object sender, RoutedEventArgs e)
        {
            if (AddAttributeTxtBox.Text.Equals(string.Empty))
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, "Attribute name is empty!");
                return;
            }

            GroupManager.GetGroup(ActiveGroupName).AddAttribute(AddAttributeTxtBox.Text);
            ActiveAttribute = AddAttributeTxtBox.Text;
            AddAttributeTxtBox.Text = string.Empty;
            InitGroups();

            GroupManager.SaveGroups();
        }
    }
}
