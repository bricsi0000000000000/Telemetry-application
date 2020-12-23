using ART_TELEMETRY_APP.Driverless.UserControls;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.Groups.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Groups.UserControls
{
    /// <summary>
    /// Represents the content of the group settings in settings menu.
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
        /// Active <see cref="Group"/>s name.
        /// Default is empty string.
        /// </summary>
        public string ActiveGroupName { get; set; } = string.Empty;

        /// <summary>
        /// Active <see cref="Attribute"/>s name.
        /// Default is empty string.
        /// </summary>
        public Attribute ActiveAttribute { get; set; }

        /// <summary>
        /// Selected <see cref="InputFile"/>s name.
        /// </summary>
        public string SelectedInputFileName { get; set; }

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

                InitActiveChannelSelectableAttributes();
                InitGroups();
            }

            InitInputFilesComboBox();
        }

        /// <summary>
        /// Initializes the <see cref="InputFilesComboBox"/>es items.
        /// </summary>
        public void InitInputFilesComboBox()
        {
            InputFilesComboBox.Items.Clear();

            foreach (var item in DriverlessInputFileManager.Instance.InputFiles)
            {
                AddInputFileComboBoxItem(item.Name);
            }

            foreach (var item in StandardInputFileManager.Instance.InputFiles)
            {
                AddInputFileComboBoxItem(item.Name);
            }
        }

        /// <summary>
        /// Adds a newly created <see cref="ComboBoxItem"/> to <see cref="InputFilesComboBox"/>.
        /// </summary>
        /// <param name="name"><see cref="ComboBoxItem"/>s name you want to add.</param>
        private void AddInputFileComboBoxItem(string name)
        {
            InputFilesComboBox.Items.Add(new ComboBoxItem()
            {
                Content = name,
                IsSelected = name.Equals(SelectedInputFileName)
            });
        }

        /// <summary>
        /// Initializes the active channels selectable attributes into <see cref="GroupChannelsStackPanel"/> if driverless.
        /// </summary>
        public void InitActiveChannelSelectableAttributes()
        {
            if (GroupManager.GetGroup(ActiveGroupName).Driverless)
            {
                var channels = DriverlessInputFileManager.Instance.GetInputFile(SelectedInputFileName).Channels;
                if (channels == null)
                {
                    return;
                }

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

        /// <summary>
        /// Destroys all active channels selectable attributes in <see cref="GroupChannelsStackPanel"/>.
        /// </summary>
        public void DestroyAllActiveChannelSelectableAttributes()
        {
            if (GroupChannelsStackPanel.Children.Count > 0)
                GroupChannelsStackPanel.Children.Clear();
        }

        /// <summary>
        /// Updates selected channels after a <see cref="CheckBox"/>-es state is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string attributeName = checkBox.Content.ToString();

            if ((bool)checkBox.IsChecked)
            {
                var inputFile = new InputFile();
                inputFile = DriverlessInputFileManager.Instance.GetInputFile(SelectedInputFileName);
                if (inputFile == null)
                {
                    inputFile = StandardInputFileManager.Instance.GetInputFile(SelectedInputFileName);
                }
                GroupManager.GetGroup(ActiveGroupName).AddAttribute(inputFile.GetChannel(attributeName));
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
            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateCharts();
        }

        /// <summary>
        /// Initializes <see cref="GroupSettingsItem"/>s in to <see cref="GroupsStackPanel"/>.
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
        /// Initializes <see cref="GroupSettingsAttribute"/>s in to <see cref="AttributesStackPanel"/>.
        /// </summary>
        public void InitAttributes()
        {
            AttributesStackPanel.Children.Clear();
            groupSettingsAttributes.Clear();
            foreach (var attribute in GroupManager.GetGroup(ActiveGroupName).Attributes)
            {
                var groupSettingsAttribute = new GroupSettingsAttribute(attribute.Name, ActiveGroupName, attribute.Color);
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
        /// Adds a <see cref="Group"/> based on <see cref="AddGroupTxtBox"/>es text.
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

            // ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).InitTabs();
        }

        /// <summary>
        /// Adds an <see cref="Attribute"/> based on <see cref="AddAttributeTxtBox"/>es text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAttribute_Click(object sender, RoutedEventArgs e)
        {
            string addAttributeText = AddAttributeTxtBox.Text;
            if (addAttributeText.Equals(string.Empty))
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, "Attribute name is empty!");
                return;
            }

            if (addAttributeText.Equals("*"))
            {
                ShowError.ShowErrorMessage(ref ErrorSnackbar, "Attribute name can't be an asterisk!");
                return;
            }

            GroupManager.GetGroup(ActiveGroupName).AddAttribute(DriverlessInputFileManager.Instance.InputFiles.First().GetChannel(addAttributeText));
            ActiveAttribute = GroupManager.GetGroup(ActiveGroupName).GetAttribute(addAttributeText);
            AddAttributeTxtBox.Text = string.Empty;
            InitGroups();

            GroupManager.SaveGroups();
        }

        /// <summary>
        /// Updates the <see cref="Group"/>s in the settings menu, when one of the <see cref="GroupSettingsItem"/> is clicked.
        /// </summary>
        /// <param name="groupName">Clicked <see cref="Group"/>s name.</param>
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

        /// <summary>
        /// Changes <see cref="SelectedInputFileName"/> every time when the <see cref="InputFilesComboBox"/>es selected item changes to the selected items content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputFilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((ComboBoxItem)InputFilesComboBox.SelectedItem);
            if (selectedItem != null)
            {
                SelectedInputFileName = selectedItem.Content.ToString();
                InitActiveChannelSelectableAttributes();
            }
        }
    }
}
