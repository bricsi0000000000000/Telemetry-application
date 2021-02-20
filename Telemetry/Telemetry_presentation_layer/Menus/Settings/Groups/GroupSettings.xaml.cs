using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.Menus.Driverless;
using Telemetry_presentation_layer.ValidationRules;

namespace Telemetry_presentation_layer.Menus.Settings.Groups
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
        public int ActiveGroupID { get; set; }
        public int ActiveAttributeID { get; set; }

        private int activeInputFileID;

        /// <summary>
        /// Active <see cref="Attribute"/>s name.
        /// Default is empty string.
        /// </summary>
        public Telemetry_data_and_logic_layer.Groups.Attribute ActiveAttribute { get; set; }

        /// <summary>
        /// Selected <see cref="InputFile"/>s name.
        /// </summary>
        public string SelectedInputFileName { get; set; }

        private FieldsViewModel fieldsViewModel = new FieldsViewModel();

        /// <summary>
        /// Constructor for <see cref="GroupSettings"/>.
        /// </summary>
        public GroupSettings()
        {
            InitializeComponent();

            DataContext = fieldsViewModel;

            if (GroupManager.Groups.Count > 0)
            {
                if (ActiveGroupName.Equals(string.Empty))
                {
                    ActiveGroupName = GroupManager.Groups[0].Name;
                }

                InitGroups();
            }
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
                var inputFile = InputFileManager.GetInputFile(SelectedInputFileName);
                GroupManager.GetGroup(ActiveGroupName).AddAttribute(inputFile.GetChannel(attributeName));
            }
            else
            {
                GroupManager.GetGroup(ActiveGroupName).RemoveAttribute(checkBox.Content.ToString());
                if (GroupManager.Groups.Count > 0)
                {
                    ActiveAttribute = GroupManager.Groups[0].Attributes[0];
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
                    var groupSettingsItem = new GroupSettingsItem(group);
                    groupSettingsItem.ChangeColorMode(group.Name.Equals(ActiveGroupName));
                    GroupsStackPanel.Children.Add(groupSettingsItem);
                    groupSettingsItems.Add(groupSettingsItem);
                }
            }

            var activeGroup = GroupManager.GetGroup(ActiveGroupName);
            if (activeGroup.Attributes.Count > 0)
            {
                ActiveAttribute = activeGroup.Attributes[0];
            }

            InitAttributes();
        }

        public void ChangeActiveGroupItem(int groupID)
        {
            if (ActiveGroupID != groupID)
            {
                ActiveGroupID = groupID;

                foreach (GroupSettingsItem item in GroupsStackPanel.Children)
                {
                    item.ChangeColorMode(item.ID == groupID);
                }

                InitAttributes();
            }
        }

        /// <summary>
        /// Initializes <see cref="GroupSettingsAttribute"/>s in to <see cref="AttributesStackPanel"/>.
        /// </summary>
        public void InitAttributes()
        {
            AttributesStackPanel.Children.Clear();
            groupSettingsAttributes.Clear();

            var attributes = GroupManager.GetGroup(ActiveGroupID).Attributes;

            ActiveAttributeID = attributes[0].ID;

            foreach (var attribute in attributes)
            {
                var groupSettingsAttribute = new GroupSettingsAttribute(ActiveGroupName, attribute);
                groupSettingsAttribute.ChangeColorMode(attribute.ID == ActiveAttributeID);
                AttributesStackPanel.Children.Add(groupSettingsAttribute);
                groupSettingsAttributes.Add(groupSettingsAttribute);
            }

            UpdateActiveAttributeOptions();
        }

        public void ChangeActiveAttributeItem(int attributeID)
        {
            if (ActiveAttributeID != attributeID)
            {
                ActiveAttributeID = attributeID;

                foreach (GroupSettingsAttribute item in AttributesStackPanel.Children)
                {
                    item.ChangeColorMode(item.ID == attributeID);
                }

                UpdateActiveAttributeOptions();
            }
        }

        private void UpdateActiveAttributeOptions()
        {
            var attribute = GroupManager.GetGroup(ActiveGroupID).GetAttribute(ActiveAttributeID);
            fieldsViewModel.LineWidth = attribute.LineWidth;
            SelectedAttributeNameTextBox.Text = attribute.Name;
            SelectedAttributeColorCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(attribute.Color);

            UpdateInputFiles();
        }

        private void UpdateInputFiles()
        {
            InputFilesStackPanel.Children.Clear();

            NoInputFilesGrid.Visibility = InputFileManager.InputFiles.Count == 0 ? Visibility.Visible : Visibility.Hidden;

            foreach (var inputFile in InputFileManager.InputFiles)
            {
                var inputFileRadioButton = new RadioButton()
                {
                    Content = inputFile.Name
                };
                inputFileRadioButton.GroupName = "inputFiles";
                inputFileRadioButton.Checked += InputFile_Checked;

                InputFilesStackPanel.Children.Add(inputFileRadioButton);
            }
        }

        private void UpdateChannels()
        {
            ChannelsStackPanel.Children.Clear();

            var inputFile = InputFileManager.GetInputFile(activeInputFileID);

            if (inputFile != null)
            {
                foreach (var channel in inputFile.Channels)
                {
                    var channelCheckbox = new CheckBox()
                    {
                        Content = channel.Name
                    };
                    channelCheckbox.Checked += Channel_Checked;
                    channelCheckbox.Unchecked += Channel_Checked;

                    ChannelsStackPanel.Children.Add(channelCheckbox);
                }
            }
        }

        private void InputFile_Checked(object sender, RoutedEventArgs e)
        {
            activeInputFileID = InputFileManager.GetInputFile(((RadioButton)sender).Content.ToString()).ID;
            UpdateChannels();
        }

        private void Channel_Checked(object sender, RoutedEventArgs e)
        {
            string content = ((CheckBox)sender).Content.ToString();

            Mouse.OverrideCursor = Cursors.Wait;

            GroupManager.GetGroup(ActiveGroupID).AddAttribute(InputFileManager.GetInputFile(activeInputFileID).GetChannel(content));
            ActiveAttribute = GroupManager.GetGroup(ActiveGroupID).GetAttribute(content);
            GroupManager.SaveGroups();
            InitGroups();
            SelectInputFile(InputFileManager.GetInputFile(activeInputFileID).Name);

            //TODO maradjon is kipipálva a channel checkbox

            Mouse.OverrideCursor = Cursors.Hand;
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
                throw new Exception("Group name is empty!");
            }

            var group = new Group(GroupManager.LastGroupID++, AddGroupTxtBox.Text);
            GroupManager.AddGroup(group);
            ActiveGroupName = AddGroupTxtBox.Text;
            AddGroupTxtBox.Text = string.Empty;
            InitGroups();

            GroupManager.SaveGroups();

            // ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).InitTabs();
        }

        /// <summary>
        /// Updates the <see cref="Group"/>s in the settings menu, when one of the <see cref="GroupSettingsItem"/> is clicked.
        /// </summary>
        /// <param name="groupName">Clicked <see cref="Group"/>s name.</param>
        public void GroupSettingsItemClicked(string groupName)
        {
            /*ActiveGroupName = groupName;
            InitGroups();
            if (GroupManager.GetGroup(groupName).Driverless)
            {
                InitActiveChannelSelectableAttributes();
            }
            else
            {
                DestroyAllActiveChannelSelectableAttributes();
            }*/
        }

        /// <summary>
        /// Changes <see cref="SelectedInputFileName"/> every time when the <see cref="InputFilesComboBox"/>es selected item changes to the selected items content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputFilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*var selectedItem = ((ComboBoxItem)InputFilesComboBox.SelectedItem);
            if (selectedItem != null)
            {
                SelectedInputFileName = selectedItem.Content.ToString();
                InitActiveChannelSelectableAttributes();
            }*/
        }

        public void UpdateAfterReadFile(string fileName)
        {
            UpdateActiveAttributeOptions();
            SelectInputFile(fileName);
            /* SelectedInputFileName = fileName;
             InitInputFilesComboBox();
             InitActiveChannelSelectableAttributes();*/
        }

        public void UpdateAfterDeleteFile(string fileName)
        {
            UpdateInputFiles();
            UpdateChannels();
            SelectInputFile(fileName);
        }

        private void SelectInputFile(string fileName)
        {
            var inputFile = InputFileManager.GetInputFile(fileName);

            if (inputFile != null)
            {
                activeInputFileID = inputFile.ID;
            }
            else
            {
                if (InputFileManager.InputFiles.Count > 0)
                {
                    activeInputFileID = InputFileManager.InputFiles.Last().ID;
                }
                else
                {
                    activeInputFileID = -1;
                }
            }

            inputFile = InputFileManager.GetInputFile(activeInputFileID);
            if (inputFile != null)
            {
                foreach (RadioButton item in InputFilesStackPanel.Children)
                {
                    item.IsChecked = item.Content.Equals(inputFile.Name);
                }
            }

            UpdateChannels();
        }

        private void SelectedAttributeColorCard_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            PickColor pickColor = new PickColor(GroupManager.GetGroup(ActiveGroupID).GetAttribute(ActiveAttributeID).Color);
            if (pickColor.ShowDialog() == true)
            {
                var pickedColor = pickColor.ColorPicker.Color;
                SelectedAttributeColorCard.Background = new SolidColorBrush(pickedColor);

                var activeAttribute = GroupManager.GetGroup(ActiveGroupID).GetAttribute(ActiveAttributeID);
                activeAttribute.Color = pickedColor.ToString();
                GroupManager.SaveGroups();

                foreach (GroupSettingsAttribute item in AttributesStackPanel.Children)
                {
                    if (item.ID == ActiveAttributeID)
                    {
                        item.ChangeColor(pickedColor.ToString());
                    }
                }
            }

            Mouse.OverrideCursor = null;
        }

        private void SelectedAttributeColorCard_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void SelectedAttributeColorCard_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedAttributeLineWidthCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedAttributeLineWidthCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void ChangeSelectedAttributeLineWidthCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedAttributeLineWidthCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Wait;
            var activeAttribute = GroupManager.GetGroup(ActiveGroupID).GetAttribute(ActiveAttributeID);
            activeAttribute.LineWidth = int.Parse(SelectedAttributeLineWidthTextBox.Text);
            GroupManager.SaveGroups();
            foreach (GroupSettingsAttribute item in AttributesStackPanel.Children)
            {
                if (item.ID == ActiveAttributeID)
                {
                    item.LineWidth = activeAttribute.LineWidth;
                }
            }
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSelectedAttributeLineWidthCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeSelectedAttributeLineWidthCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSelectedAttributeLineWidthCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeSelectedAttributeLineWidthCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }
    }
}
