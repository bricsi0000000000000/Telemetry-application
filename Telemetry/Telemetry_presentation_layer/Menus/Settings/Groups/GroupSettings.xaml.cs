using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataLayer.Groups;
using DataLayer.InputFiles;
using LocigLayer.Colors;
using LocigLayer.Groups;
using LocigLayer.InputFiles;
using LocigLayer.Texts;
using PresentationLayer.Converters;
using PresentationLayer.Menus.Driverless;
using PresentationLayer.Menus.Live;
using PresentationLayer.ValidationRules;

namespace PresentationLayer.Menus.Settings.Groups
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
        /// Selected <see cref="InputFile"/>s name.
        /// </summary>
        public string SelectedInputFileName { get; set; }

        private readonly FieldsViewModel fieldsViewModel = new FieldsViewModel();

        public GroupSettings()
        {
            InitializeComponent();

            fieldsViewModel.AddGroupName = "";
            fieldsViewModel.AddAttributeName = "";
            fieldsViewModel.AttributeName = "";
            fieldsViewModel.AddAttributeLineWidth = 1;
            fieldsViewModel.HorizontalAxis = "";
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
                GroupManager.GetGroup(ActiveGroupID).AddAttribute(inputFile.GetChannel(attributeName));
                ActiveAttributeID = GroupManager.GetGroup(ActiveGroupID).Attributes.Last().ID;
            }
            else
            {
                GroupManager.GetGroup(ActiveGroupID).RemoveAttribute(checkBox.Content.ToString());
                if (GroupManager.Groups.Count > 0)
                {
                    ActiveAttributeID = GroupManager.Groups[0].Attributes.Last().ID;
                }
            }

            InitGroups();
            GroupManager.SaveGroups();
            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).BuildCharts();
        }

        /// <summary>
        /// Initializes <see cref="GroupSettingsItem"/>s in to <see cref="GroupsStackPanel"/>.
        /// </summary>
        public void InitGroups()
        {
            GroupsStackPanel.Children.Clear();

            foreach (var group in GroupManager.Groups)
            {
                var groupSettingsItem = new GroupSettingsItem(group);
                groupSettingsItem.ChangeColorMode(group.ID == ActiveGroupID);
                GroupsStackPanel.Children.Add(groupSettingsItem);
                groupSettingsItems.Add(groupSettingsItem);
            }

            var activeGroup = GroupManager.GetGroup(ActiveGroupID);
            SelectedGroupNameTextBox.Text = activeGroup.Name;
            fieldsViewModel.HorizontalAxis = activeGroup.HorizontalAxis;
            if (activeGroup.Attributes.Count > 0)
            {
                if (ActiveAttributeID == -1)
                {
                    ActiveAttributeID = activeGroup.Attributes.Last().ID;
                }
            }
            fieldsViewModel.GroupName = activeGroup.Name;

            InitAttributes();
        }

        public void ChangeActiveGroupItem(int groupID)
        {
            if (ActiveGroupID != groupID)
            {
                ActiveGroupID = groupID;

                var group = GroupManager.GetGroup(ActiveGroupID);
                SelectedGroupNameTextBox.Text = group.Name;
                SelectedGroupHorizontalAxisTextBox.Text = group.HorizontalAxis;

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

            var group = GroupManager.GetGroup(ActiveGroupID);
            if (group != null)
            {
                var attributes = group.Attributes;

                if (attributes.Count > 0)
                {
                    if (ActiveAttributeID == -1)
                    {
                        ActiveAttributeID = attributes[0].ID;
                    }

                    foreach (var attribute in attributes)
                    {
                        var groupSettingsAttribute = new GroupSettingsAttribute(ActiveGroupName, attribute);
                        groupSettingsAttribute.ChangeColorMode(attribute.ID == ActiveAttributeID);
                        AttributesStackPanel.Children.Add(groupSettingsAttribute);
                        groupSettingsAttributes.Add(groupSettingsAttribute);
                    }

                }
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
            var group = GroupManager.GetGroup(ActiveGroupID);
            if (group != null)
            {
                NoAttributesGrid.Visibility = Visibility.Hidden;

                var attribute = group.GetAttribute(ActiveAttributeID);
                if (attribute != null)
                {
                    fieldsViewModel.LineWidth = attribute.LineWidth;
                    fieldsViewModel.AttributeName = attribute.Name;
                    SelectedAttributeNameTextBox.Text = attribute.Name;
                    SelectedAttributeColorCard.Background = ConvertColor.ConvertStringColorToSolidColorBrush(attribute.Color);

                    UpdateInputFiles();
                }
                else
                {
                    NoAttributesGrid.Visibility = Visibility.Visible;
                }
            }
            else
            {
                NoAttributesGrid.Visibility = Visibility.Visible;
            }
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
                var group = GroupManager.GetGroup(ActiveGroupID);
                foreach (var channel in inputFile.Channels)
                {
                    var channelCheckbox = new CheckBox()
                    {
                        Content = channel.Name
                    };
                    channelCheckbox.IsChecked = group.GetAttribute(channel.Name) != null;
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
            Mouse.OverrideCursor = Cursors.Wait;

            string content = ((CheckBox)sender).Content.ToString();

            if ((bool)((CheckBox)sender).IsChecked)
            {
                GroupManager.GetGroup(ActiveGroupID).AddAttribute(InputFileManager.GetInputFile(activeInputFileID).GetChannel(content));
                ActiveAttributeID = GroupManager.GetGroup(ActiveGroupID).GetAttribute(content).ID;
            }
            else
            {
                GroupManager.GetGroup(ActiveGroupID).RemoveAttribute(content);
                var group = GroupManager.GetGroup(ActiveGroupID);
                if (group.Attributes.Count > 0)
                {
                    ActiveAttributeID = group.Attributes.Last().ID;
                }
                else
                {
                    ActiveAttributeID = -1;
                }
            }

            GroupManager.SaveGroups();
            InitGroups();
            SelectInputFile();
            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).BuildCharts();

            Mouse.OverrideCursor = null;
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

        public void SelectInputFile(string fileName = "")
        {
            InputFile inputFile;
            if (fileName.Equals(string.Empty))
            {
                inputFile = InputFileManager.GetInputFile(activeInputFileID);
            }
            else
            {
                inputFile = InputFileManager.GetInputFile(fileName);
            }


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
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).BuildCharts();

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

            if (int.TryParse(SelectedAttributeLineWidthTextBox.Text, out int lineWidth))
            {
                if (lineWidth > 0)
                {
                    var activeAttribute = GroupManager.GetGroup(ActiveGroupID).GetAttribute(ActiveAttributeID);
                    if (activeAttribute.LineWidth != lineWidth)
                    {
                        activeAttribute.LineWidth = lineWidth;
                        GroupManager.SaveGroups();
                        foreach (GroupSettingsAttribute item in AttributesStackPanel.Children)
                        {
                            if (item.ID == ActiveAttributeID)
                            {
                                item.LineWidth = activeAttribute.LineWidth;
                            }
                        }

                        ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).BuildCharts();
                    }
                }
            }

            Mouse.OverrideCursor = null;
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

        private void DeleteAttributeCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeleteAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary700);
        }

        private void DeleteAttributeCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DeleteAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);

            AddGroupGridBackground.Visibility = Visibility.Visible;

            var changeLiveStatusWindow = new PopUpWindow($"You are about to delete '{GroupManager.GetGroup(ActiveGroupID).GetAttribute(ActiveAttributeID).Name}'\n" +
                                                         $"Are you sure about that?",
                                                         PopUpWindow.PopUpType.DeleteGroupAttribute);
            changeLiveStatusWindow.ShowDialog();
        }

        public void DeleteAttribute(bool delete)
        {
            if (delete)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                GroupManager.GetGroup(ActiveGroupID).RemoveAttribute(ActiveAttributeID);
                var group = GroupManager.GetGroup(ActiveGroupID);
                if (group.Attributes.Count > 0)
                {
                    ActiveAttributeID = group.Attributes.Last().ID;
                }
                else
                {
                    ActiveAttributeID = -1;
                }

                GroupManager.SaveGroups();
                InitGroups();
                SelectInputFile();
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).BuildCharts();

                Mouse.OverrideCursor = null;
            }

            AddGroupGridBackground.Visibility = Visibility.Hidden;
        }

        private void DeleteAttributeCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary800);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DeleteAttributeCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Primary900);
            Mouse.OverrideCursor = null;
        }

        private void AddAttributeCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void AddAttributeCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            Mouse.OverrideCursor = Cursors.Wait;

            AddGroupGridBackground.Visibility = Visibility.Visible;
            AddAttributeGrid.Visibility = Visibility.Visible;
            AddAttributeNameTextBox.Focus();

            Mouse.OverrideCursor = null;
        }

        private void AddAttributeCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AddAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void AddAttributeCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AddAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void DeleteGroupCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeleteGroupCardButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary700));
        }

        private void DeleteGroupCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DeleteGroupCardButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary800));

            AddGroupGridBackground.Visibility = Visibility.Visible;

            var changeLiveStatusWindow = new PopUpWindow($"You are about to delete '{GroupManager.GetGroup(ActiveGroupID).Name}'\n" +
                                                         $"Are you sure about that?",
                                                         PopUpWindow.PopUpType.DeleteGroup);
            changeLiveStatusWindow.ShowDialog();
        }

        public void DeleteGroup(bool delete)
        {
            if (delete)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                GroupManager.RemoveGroup(ActiveGroupID);
                ActiveGroupID = GroupManager.Groups[0].ID;
                InitGroups();

                GroupManager.SaveGroups();

                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).InitializeGroupItems();

                SelectInputFile();

                Mouse.OverrideCursor = null;
            }

            AddGroupGridBackground.Visibility = Visibility.Hidden;
        }

        private void DeleteGroupCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteGroupCardButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary800));
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void DeleteGroupCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteGroupCardButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Primary900));
            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedGroupNameCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedGroupNameCardButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary200));
        }

        private void ChangeSelectedGroupNameCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            ChangeSelectedGroupNameCardButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));

            string newName = SelectedGroupNameTextBox.Text;
            if (!newName.Equals(string.Empty))
            {
                GroupManager.GetGroup(ActiveGroupID).Name = newName;
                GroupManager.SaveGroups();
                foreach (GroupSettingsItem item in GroupsStackPanel.Children)
                {
                    if (item.ID == ActiveGroupID)
                    {
                        item.GroupName = newName;
                    }
                }

                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).AddRenamedGroupToSelectedGroups(newName);
            }

            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedGroupNameCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeSelectedGroupNameCardButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary100));
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSelectedGroupNameCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeSelectedGroupNameCardButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary50));
            Mouse.OverrideCursor = null;
        }

        private void AddGroupCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddGroupCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void AddGroupCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddGroupCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Wait;

            AddGroupGridBackground.Visibility = Visibility.Visible;
            AddGroupGrid.Visibility = Visibility.Visible;
            AddGroupNameTextBox.Focus();

            Mouse.OverrideCursor = null;
        }

        private void AddGroupCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AddGroupCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void AddGroupCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AddGroupCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void AddGroupPopUpCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddGroupPopUpCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void AddGroupPopUpCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddGroupPopUpCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            Mouse.OverrideCursor = Cursors.Wait;

            if (!AddGroupNameTextBox.Text.Equals(string.Empty))
            {
                AddGroupGridBackground.Visibility = Visibility.Hidden;
                AddGroupGrid.Visibility = Visibility.Hidden;

                var group = new Group(GroupManager.LastGroupID++, AddGroupNameTextBox.Text);
                GroupManager.AddGroup(group);
                GroupManager.SaveGroups();
                ActiveGroupID = group.ID;
                AddGroupNameTextBox.Text = string.Empty;
                InitGroups();
                UpdateChannels();
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).InitializeGroupItems();
                // ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).InitTabs();
            }

            Mouse.OverrideCursor = null;
        }

        private void AddGroupPopUpCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AddGroupPopUpCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void AddGroupPopUpCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AddGroupPopUpCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void AddAttributePopUpCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddGroupPopUpCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void AddAttributePopUpCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddGroupPopUpCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            if (AddAttributeNameTextBox.Text.Equals(string.Empty) ||
                AddAttributeLineWidthTextBox.Text.Equals(string.Empty))
            {
                return;
            }

            if (!int.TryParse(AddAttributeLineWidthTextBox.Text, out int lineWidth))
            {

                return;
            }

            if (lineWidth <= 0)
            {
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            AddGroupGridBackground.Visibility = Visibility.Hidden;
            AddAttributeGrid.Visibility = Visibility.Hidden;

            GroupManager.GetGroup(ActiveGroupID).AddAttribute(AddAttributeNameTextBox.Text, ColorManager.GetChartColor, lineWidth);
            ActiveAttributeID = GroupManager.GetGroup(ActiveGroupID).Attributes.Last().ID;

            GroupManager.SaveGroups();
            InitGroups();
            SelectInputFile();

            ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).BuildCharts();

            AddAttributeNameTextBox.Text = string.Empty;
            AddAttributeLineWidthTextBox.Text = "1";

            Mouse.OverrideCursor = null;
        }

        private void AddAttributePopUpCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AddGroupPopUpCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void AddAttributePopUpCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AddGroupPopUpCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void CancelAddAttributeCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CancelAddAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void CancelAddAttributeCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CancelAddAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            AddGroupGridBackground.Visibility = Visibility.Hidden;
            AddAttributeGrid.Visibility = Visibility.Hidden;

            AddAttributeNameTextBox.Text = string.Empty;
            fieldsViewModel.AddAttributeLineWidth = 1;
            //AddAttributeLineWidthTextBox.Text = "1";
        }

        private void CancelAddAttributeCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CancelAddAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void CancelAddAttributeCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CancelAddAttributeCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void CancelAddGroupCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CancelAddGroupCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void CancelAddGroupCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CancelAddGroupCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            AddGroupGridBackground.Visibility = Visibility.Hidden;
            AddGroupGrid.Visibility = Visibility.Hidden;

            AddGroupNameTextBox.Text = string.Empty;
        }

        private void CancelAddGroupCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CancelAddGroupCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void CancelAddGroupCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CancelAddGroupCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedAttributeNameCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedAttributeNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void ChangeSelectedAttributeNameCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedAttributeNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            Mouse.OverrideCursor = Cursors.Wait;

            if (!SelectedAttributeNameTextBox.Text.Equals(string.Empty))
            {
                string oldName = GroupManager.GetGroup(ActiveGroupID).GetAttribute(ActiveAttributeID).Name;
                string newName = SelectedAttributeNameTextBox.Text;

                if (!oldName.Equals(newName))
                {
                    GroupManager.GetGroup(ActiveGroupID).GetAttribute(ActiveAttributeID).Name = newName;
                    GroupManager.SaveGroups();

                    foreach (GroupSettingsAttribute item in AttributesStackPanel.Children)
                    {
                        if (item.AttributeName.Equals(oldName))
                        {
                            item.AttributeName = newName;
                        }
                    }

                    //TODO update group charts
                }
            }

            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedAttributeNameCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeSelectedAttributeNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSelectedAttributeNameCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeSelectedAttributeNameCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedGroupHorizontalAxisCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedGroupHorizontalAxisCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary200);
        }

        private void ChangeSelectedGroupHorizontalAxisCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedGroupHorizontalAxisCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);

            Mouse.OverrideCursor = Cursors.Wait;

            string newHorizontalAxis = SelectedGroupHorizontalAxisTextBox.Text;
            if (!newHorizontalAxis.Equals(string.Empty))
            {
                GroupManager.GetGroup(ActiveGroupID).HorizontalAxis = newHorizontalAxis;
                GroupManager.SaveGroups();

                //TODO update charts
            }

            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedGroupHorizontalAxisCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeSelectedGroupHorizontalAxisCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary100);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSelectedGroupHorizontalAxisCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeSelectedGroupHorizontalAxisCardButton.Background = ConvertColor.ConvertStringColorToSolidColorBrush(ColorManager.Secondary50);
            Mouse.OverrideCursor = null;
        }
    }
}
