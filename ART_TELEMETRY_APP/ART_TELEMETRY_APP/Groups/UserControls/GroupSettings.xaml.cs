using ART_TELEMETRY_APP.Driverless.UserControls;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
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
        /// <summary>
        /// List of <see cref="GroupSettingsItem"/>s.
        /// </summary>
        private readonly List<GroupSettingsItem> groupSettingsItems = new List<GroupSettingsItem>();

        /// <summary>
        /// List of <see cref="GroupSettingsAttribute"/>s.
        /// </summary>
        private readonly List<GroupSettingsAttribute> groupSettingsAttributes = new List<GroupSettingsAttribute>();

        /// <summary>
        /// Active <see cref="Group"/> name.
        /// Default is empty string.
        /// </summary>
        public string ActiveGroupName { get; set; } = string.Empty;

        /// <summary>
        /// Active attribute name.
        /// Default is empty string.
        /// </summary>
        public Attribute ActiveAttribute { get; set; }

        /// <summary>
        /// Constructor for <see cref="GroupSettings"/>.
        /// </summary>
        public GroupSettings()
        {
            InitializeComponent();

            if (GroupManager.Groups.Count > 0)
            {
                if (ActiveGroupName.Equals(string.Empty))
                {
                    ActiveGroupName = GroupManager.Groups.First().Name;
                }

                if (!GroupManager.GetGroup(ActiveGroupName).Driverless)
                {
                    DestroyAllActiveChannelSelectableAttributes();
                }

                ChangeGroupSettingsItemTypeTitle();
                InitActiveChannelSelectableAttributes();
                InitGroups();
            }
        }

        /// <summary>
        /// Initializes the active channels selectable attributes into <see cref="GroupChannelsStackPanel"/> if driverless.
        /// </summary>
        public void InitActiveChannelSelectableAttributes()
        {
            if (GroupManager.GetGroup(ActiveGroupName).Driverless)
            {
                var channels = ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).Channels;
                GroupChannelsStackPanel.Children.Clear();

                foreach (var channel in channels)
                {
                    var checkBox = new CheckBox()
                    {
                        Content = channel.Name,
                        IsChecked = GetGroupSettingsAttribute(channel.Name) != null
                    };
                    checkBox.Click += ChannelCheckBox_Click;
                    GroupChannelsStackPanel.Children.Add(checkBox);
                }
            }
        }

        public void DestroyAllActiveChannelSelectableAttributes()
        {
            if (GroupChannelsStackPanel.Children.Count > 0)
                GroupChannelsStackPanel.Children.Clear();
        }

        /// <summary>
        /// Changes the <see cref="GroupSettingsItemTypeLbl"/> content to Driverless or Standard based on the active groups Driverless property.
        /// </summary>
        public void ChangeGroupSettingsItemTypeTitle()
        {
            GroupSettingsItemTypeLbl.Content = GroupManager.GetGroup(ActiveGroupName).Driverless ? "Driverless" : "Standard";
        }

        /// <summary>
        /// Updates selected channels after a <see cref="CheckBox"/>-es state is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            if ((bool)checkBox.IsChecked)
            {
                GroupManager.GetGroup(ActiveGroupName).AddAttribute(checkBox.Content.ToString());
            }
            else
            {
                GroupManager.GetGroup(ActiveGroupName).RemoveAttribute(checkBox.Content.ToString());
                if (GroupManager.Groups.Count > 0)
                {
                    ActiveAttribute = GroupManager.Groups.First().Attributes.First();
                }
            }

            InitGroups();
            GroupManager.SaveGroups();
        }

        /// <summary>
        /// Initializes <see cref="Group"/>s.
        /// </summary>
        public void InitGroups()
        {
            GroupsStackPanel.Children.Clear();

            foreach (var group in GroupManager.Groups)
            {
                if (group.Customizable)
                {
                    var groupSettingsItem = new GroupSettingsItem(group.Name, group.Driverless);
                    groupSettingsItem.ChangeColorMode(group.Name.Equals(ActiveGroupName));
                    GroupsStackPanel.Children.Add(groupSettingsItem);
                    groupSettingsItems.Add(groupSettingsItem);
                }
            }

            if (GroupManager.GetGroup(ActiveGroupName).Attributes.Count > 0)
            {
                ActiveAttribute = GroupManager.GetGroup(ActiveGroupName).Attributes.First();
            }

            InitAttributes();
        }

        /// <summary>
        /// Initializes attributes.
        /// </summary>
        public void InitAttributes()
        {
            AttributesStackPanel.Children.Clear();
            groupSettingsAttributes.Clear();
            foreach (var attribute in GroupManager.GetGroup(ActiveGroupName).Attributes)
            {
                var groupSettingsAttribute = new GroupSettingsAttribute(attribute.Name, ActiveGroupName, attribute.Color);
                groupSettingsAttribute.ChangeColorMode(attribute.Equals(ActiveAttribute));
                AttributesStackPanel.Children.Add(groupSettingsAttribute);
                groupSettingsAttributes.Add(groupSettingsAttribute);
            }
        }

        /// <summary>
        /// Finds a <see cref="GroupSettingsItem"/> based on <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="GroupSettingsItem"/> you want to find.</param>
        /// <returns>A <see cref="GroupSettingsItem"/>.</returns>
        public GroupSettingsItem GetGroupSettingsContent(string name) => groupSettingsItems.Find(n => n.GroupName.Equals(name));

        /// <summary>
        /// Finds a <see cref="GroupSettingsAttribute"/> based on <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="GroupSettingsAttribute"/> you want to find.</param>
        /// <returns>A <see cref="GroupSettingsAttribute"/>.</returns>
        public GroupSettingsAttribute GetGroupSettingsAttribute(string name) => groupSettingsAttributes.Find(n => n.AttributeName.Equals(name));

        /// <summary>
        /// Adds a group based on <see cref="AddGroupTxtBox"/>es content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).InitTabs();
        }

        /// <summary>
        /// Adds an attribute based on <see cref="AddAttributeTxtBox"/>es content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAttribute_Click(object sender, RoutedEventArgs e)
        {
            if (AddAttributeTxtBox.Text.Equals(string.Empty))
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, "Attribute name is empty!");
                return;
            }

            if (AddAttributeTxtBox.Text.Equals("*"))
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, "Attribute name can't be an asterisk!");
                return;
            }

            GroupManager.GetGroup(ActiveGroupName).AddAttribute(AddAttributeTxtBox.Text);
            ActiveAttribute = GroupManager.GetGroup(ActiveGroupName).GetAttribute(AddAttributeTxtBox.Text);
            AddAttributeTxtBox.Text = string.Empty;
            InitGroups();

            GroupManager.SaveGroups();
        }

        /// <summary>
        /// Updates the groups in the settings menu, when one of the <see cref="GroupSettingsItem"/> is clicked.
        /// </summary>
        /// <param name="groupName">Clicked <see cref="GroupSettingsItem"/>s group name.</param>
        public void GroupSettingsItemClicked(string groupName)
        {
            ActiveGroupName = groupName;
            InitGroups();
            if (GroupManager.GetGroup(groupName).Driverless)
            {
                InitActiveChannelSelectableAttributes();
            }
            else
            {
                DestroyAllActiveChannelSelectableAttributes();
            }
        }
    }
}
