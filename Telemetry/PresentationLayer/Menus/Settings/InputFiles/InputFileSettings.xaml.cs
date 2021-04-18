using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataLayer;
using DataLayer.Groups;
using DataLayer.InputFiles;
using PresentationLayer.Errors;
using PresentationLayer.Menus.Driverless;
using PresentationLayer.Menus.Live;
using PresentationLayer.Menus.Settings.Groups;
using PresentationLayer.ValidationRules;
using LocigLayer.InputFiles;
using LocigLayer.Colors;
using LocigLayer.Texts;
using LocigLayer.Groups;
using PresentationLayer.Extensions;

namespace PresentationLayer.Menus.Settings.InputFiles
{
    /// <summary>
    /// Represents the <see cref="InputFile"/>s settigns in settings menu.
    /// </summary>
    public partial class InputFilesSettings : UserControl
    {
        /// <summary>
        /// List of <see cref="InputFileSettingsItem"/>s.
        /// </summary>
        private readonly List<InputFileSettingsItem> inputFileSettingsItems = new List<InputFileSettingsItem>();

        /// <summary>
        /// List of <see cref="InputFileChannelSettingsItem"/>s.
        /// </summary>
        private readonly List<InputFileChannelSettingsItem> inputFileChannelSettingsItems = new List<InputFileChannelSettingsItem>();

        /// <summary>
        /// Active <see cref="InputFile"/>s ID.
        /// </summary>
        public int ActiveInputFileID { get; set; }

        /// <summary>
        /// Active <see cref="Channel"/>.
        /// </summary>
        public int ActiveChannelID { get; set; }

        private bool isAnyInputFile = false;

        private readonly FieldsViewModel fieldsViewModel = new FieldsViewModel();

        public InputFilesSettings()
        {
            InitializeComponent();

            fieldsViewModel.ChannelName = string.Empty;
            fieldsViewModel.LineWidth = 1;

            DataContext = fieldsViewModel;

            ChangeAvailabilityOfInputFileButtons();
        }

        /// <summary>
        /// Adds an <see cref="InputFile"/>, updates <see cref="ActiveInputFileName"/> ,initializes <see cref="Channel"/> items.
        /// </summary>
        /// <param name="inputFile">The <see cref="InputFile"/> you want to add.</param>
        public void AddInputFileSettingsItem(InputFile inputFile)
        {
            InitActiveChannel();
            ActiveInputFileID = inputFile.ID;

            InitChannelItems();
            ChangeAllInputFileSettingsItemColorMode();
            AddSingleInputFileSettingsItem(inputFile);

            isAnyInputFile = InputFileManager.InputFiles.Count > 0;
            ChangeAvailabilityOfInputFileButtons();

            SelectedInputFileNameTextBox.Text = inputFile.Name;
            SelectedInputFileOriginalNameTextBox.Text = inputFile.OriginalName;

            NoInputFilesGrid.Visibility = isAnyInputFile ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Changes the <see cref="ActiveInputFileName"/> to <paramref name="inputFileName"/> and updates channel items.
        /// </summary>
        /// <param name="inputFileName"><see cref="InputFile"/>s name that will be the <see cref="ActiveInputFileName"/>.</param>
        public void ChangeActiveInputFileSettingsItem(int id)
        {
            ActiveInputFileID = id;
            SelectedInputFileNameTextBox.Text = InputFileManager.Get(id).Name;
            SelectedInputFileOriginalNameTextBox.Text = InputFileManager.Get(id).OriginalName;
            ChangeAllInputFileSettingsItemColorMode();

            InitChannelItems();
        }

        public void ChangeActiveChannelSettingsItem(int id)
        {
            ActiveChannelID = id;
            SelectedChannelNameTextBox.Text = InputFileManager.Get(ActiveInputFileID).GetChannel(id).Name;
            SelectedChannelLineWidthTextBox.Text = InputFileManager.Get(ActiveInputFileID).GetChannel(id).LineWidth.ToString();
            ChangeAllChannelSettingsItemColorMode();
        }

        /// <summary>
        /// Changes all <see cref="InputFileSettingsItem"/>s color mode to unselected in <see cref="InputFileStackPanel"/>.
        /// </summary>
        private void ChangeAllInputFileSettingsItemColorMode()
        {
            foreach (InputFileSettingsItem item in InputFileStackPanel.Children)
            {
                item.ChangeColorMode(item.ID == ActiveInputFileID);
            }
        }

        private void ChangeAllChannelSettingsItemColorMode()
        {
            foreach (InputFileChannelSettingsItem item in ChannelItemsStackPanel.Children)
            {
                item.ChangeColorMode(item.ChannelID == ActiveChannelID);
            }
        }

        /// <summary>
        /// Removes an <see cref="InputFileSettingsItem"/> from <see cref="InputFileStackPanel"/> based on <paramref name="inputFileName"/>.
        /// </summary>
        /// <param name="inputFileName"><see cref="InputFile"/>s name, you want to remove.</param>
        public void RemoveSingleInputFileSettingsItem(string inputFileName)
        {
            int index = 0;
            while (index < InputFileStackPanel.Children.Count &&
                   !((InputFileSettingsItem)InputFileStackPanel.Children[index]).InputFileNameLabel.Content.Equals(inputFileName))
            {
                index++;
            }

            InputFileStackPanel.Children.RemoveAt(index);
        }

        /// <summary>
        /// Initializes <see cref="InputFileSettingsItem"/>s.
        /// </summary>
        public void InitInputFileSettingsItems()
        {
            UpdateActiveInputFileID();

            InputFileStackPanel.Children.Clear();
            InitInputFileSettingsItemElements();

            InitActiveChannel();
            InitChannelItems();
        }

        /// <summary>
        /// Sets <see cref="ActiveInputFileName"/> to <see cref="DriverlessInputFileManager"/>s or <see cref="StandardInputFileManager"/>s first <see cref="InputFile"/>s name if there is any <see cref="InputFile"/>.
        /// </summary>
        private void UpdateActiveInputFileID()
        {
            if (ActiveInputFileID == -1)
            {
                if (InputFileManager.InputFiles.Count > 0)
                {
                    ActiveInputFileID = InputFileManager.InputFiles.First().ID;
                }
            }
        }

        /// <summary>
        /// Adds <see cref="InputFileSettingsItem"/> from <see cref="InputFileManager.Instance.InputFiles"/>.
        /// </summary>
        private void InitInputFileSettingsItemElements()
        {
            foreach (var inputFile in InputFileManager.InputFiles)
            {
                AddSingleInputFileSettingsItem(inputFile);
            }
        }

        /// <summary>
        /// Creates and adds an <see cref="InputFileSettingsItem"/> to <see cref="inputFileSettingsItems"/>.
        /// </summary>
        /// <param name="inputFile">The <see cref="InputFileSettingsItem"/> will be based on this <see cref="InputFile"/>.</param>
        private void AddSingleInputFileSettingsItem(InputFile inputFile)
        {
            var inputFileSettingsItem = new InputFileSettingsItem(inputFile);
            inputFileSettingsItem.ChangeColorMode(inputFile.ID == ActiveInputFileID);
            InputFileStackPanel.Children.Add(inputFileSettingsItem);
            inputFileSettingsItems.Add(inputFileSettingsItem);
        }

        private void InitActiveChannel()
        {
            var activeInputFile = InputFileManager.Get(ActiveInputFileID);
            if (activeInputFile != null)
            {
                if (activeInputFile.Channels.Count > 0)
                {
                    ActiveChannelID = activeInputFile.Channels.First().ID;
                }
            }
        }

        /// <summary>
        /// Initializes channel items.
        /// </summary>
        public void InitChannelItems()
        {
            ChannelItemsStackPanel.Children.Clear();
            inputFileChannelSettingsItems.Clear();

            var activeInputFile = InputFileManager.Get(ActiveInputFileID);
            if (activeInputFile != null)
            {
                foreach (var channel in activeInputFile.Channels)
                {
                    AddInputFileChannelSettingsItem(channel);
                }

                if (activeInputFile.Channels.Count > 0)
                {
                    ActiveChannelID = activeInputFile.Channels.First().ID;
                    InitChannelSettings();
                }
            }

            NoChannelsGrid.Visibility = NoSelectedChannelGrid.Visibility = ChannelItemsStackPanel.Children.Count > 0 ? Visibility.Hidden : Visibility.Visible;
        }

        private void InitChannelSettings()
        {
            var channel = InputFileManager.Get(ActiveInputFileID).GetChannel(ActiveChannelID);
            SelectedChannelNameTextBox.Text = channel.Name;
            SelectedChannelLineWidthTextBox.Text = channel.LineWidth.ToString();

            ChangeAllChannelSettingsItemColorMode();
        }

        /// <summary>
        /// Creates and adds an <see cref="InputFileChannelSettingsItem"/> to <see cref="inputFileChannelSettingsItems"/>.
        /// </summary>
        /// <param name="channel"></param>
        private void AddInputFileChannelSettingsItem(Channel channel)
        {
            var inputFileChannelSettingsItem = new InputFileChannelSettingsItem(channel, ActiveInputFileID);
            ChannelItemsStackPanel.Children.Add(inputFileChannelSettingsItem);
            inputFileChannelSettingsItems.Add(inputFileChannelSettingsItem);
        }

        private void ReadInputFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Read file",
                DefaultExt = ".csv",
                Multiselect = false,
                Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = openFileDialog.FileName.Split('\\').Last();

                    if (!InputFileManager.HasInputFile(fileName))
                    {
                        ReadFileProgressBarLbl.Content = $"Reading \"{fileName}\"";
                        var dataReader = new DataReader();
                        dataReader.SetupReader(ReadFileProgressBarGrid,
                                               ReadFileProgressBar,
                                               FileType.Driverless);
                        dataReader.ReadFile(openFileDialog.FileName);
                    }
                    else
                    {
                        throw new Exception($"File '{fileName}' already exists");
                    }
                }
                catch (Exception exception)
                {
                    ShowError.ShowErrorMessage(exception.Message, nameof(InputFilesSettings));
                }
            }
        }

        private void ChangeAvailabilityOfInputFileButtons()
        {
            DeleteFileCardButton.Background = isAnyInputFile ? ColorManager.Primary900.ConvertBrush() :
                                                               ColorManager.Primary400.ConvertBrush();

            ChangeSelectedInputFileNameIcon.Foreground = isAnyInputFile ? ColorManager.Secondary900.ConvertBrush() :
                                                                          ColorManager.Secondary400.ConvertBrush();

            if (!isAnyInputFile)
            {
                SelectedInputFileNameTextBox.Text = ".";
            }
        }

        private void ChangeSelectedInputFileNameCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isAnyInputFile)
            {
                ChangeSelectedInputFileNameCardButton.Background = ColorManager.Secondary200.ConvertBrush();
            }
        }

        private void ChangeSelectedInputFileNameCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isAnyInputFile)
            {
                ChangeSelectedInputFileNameCardButton.Background = ColorManager.Secondary100.ConvertBrush();
                string newName = SelectedInputFileNameTextBox.Text;
                if (!newName.Equals(string.Empty))
                {
                    if (!newName.Equals(InputFileManager.Get(ActiveInputFileID).Name))
                    {
                        InputFileManager.Get(ActiveInputFileID).Name = newName;
                        foreach (InputFileSettingsItem item in InputFileStackPanel.Children)
                        {
                            if (item.ID == ActiveInputFileID)
                            {
                                item.InputFileName = newName;
                            }
                        }

                    ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).UpdateAfterFileRename(newName);
                        ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).UpdateAfterReadFile(SelectedInputFileNameTextBox.Text);
                    }
                }
            }
        }

        private void ChangeSelectedInputFileNameCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isAnyInputFile)
            {
                ChangeSelectedInputFileNameCardButton.Background = ColorManager.Secondary100.ConvertBrush();
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void ChangeSelectedInputFileNameCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isAnyInputFile)
            {
                ChangeSelectedInputFileNameCardButton.Background = ColorManager.Secondary50.ConvertBrush();
                Mouse.OverrideCursor = null;
            }
        }

        private void ReadFileCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ReadFileCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void ReadFileCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ReadFileCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            ReadInputFile();
        }

        private void ReadFileCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ReadFileCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ReadFileCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ReadFileCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }

        private void DeleteFileCardButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isAnyInputFile)
            {
                DeleteFileCardButton.Background = ColorManager.Primary700.ConvertBrush();
            }
        }

        private void DeleteFileCardButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isAnyInputFile)
            {
                DeleteFileCardButton.Background = ColorManager.Primary800.ConvertBrush();

                LoadingGrid.Visibility = Visibility.Visible;

                var changeLiveStatusWindow = new PopUpWindow($"You are about to delete '{InputFileManager.Get(ActiveInputFileID).Name}'\n" +
                                                             $"Are you sure about that?",
                                                             PopUpWindow.PopUpType.DeleteInputFile);
                changeLiveStatusWindow.ShowDialog();
            }
        }

        public void DeleteInputFile(bool delete)
        {
            if (delete)
            {
                string inputFileName = InputFileManager.Get(ActiveInputFileID).Name;

                InputFileManager.Remove(ActiveInputFileID);

                RemoveSingleInputFileSettingsItem(inputFileName);

                if (InputFileManager.InputFiles.Count > 0)
                {
                    ActiveInputFileID = InputFileManager.InputFiles.Last().ID;
                    foreach (InputFileSettingsItem item in InputFileStackPanel.Children)
                    {
                        item.ChangeColorMode(item.ID == ActiveInputFileID);
                    }
                }
                else
                {
                    ActiveInputFileID = -1;
                }

                var inputFile = InputFileManager.Get(ActiveInputFileID);
                if (inputFile != null)
                {
                    SelectedInputFileNameTextBox.Text = inputFile.Name;
                    SelectedInputFileOriginalNameTextBox.Text = inputFile.OriginalName;
                }
                else
                {
                    SelectedInputFileNameTextBox.Text = string.Empty;
                    SelectedInputFileOriginalNameTextBox.Text = string.Empty;
                }

                InitChannelItems();

                ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).RemoveInputFileItem(inputFileName);
                ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).UpdateAfterDeleteFile(inputFileName);

                isAnyInputFile = InputFileManager.InputFiles.Count > 0;

                NoInputFilesGrid.Visibility = isAnyInputFile ? Visibility.Hidden : Visibility.Visible;

                ChangeAvailabilityOfInputFileButtons();
            }

            LoadingGrid.Visibility = Visibility.Hidden;
        }

        private void DeleteFileCardButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isAnyInputFile)
            {
                DeleteFileCardButton.Background = ColorManager.Primary800.ConvertBrush();
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void DeleteFileCardButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isAnyInputFile)
            {
                DeleteFileCardButton.Background = ColorManager.Primary900.ConvertBrush();
                Mouse.OverrideCursor = null;
            }
        }

        private void ChangeSelectedChannelNameCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedChannelNameCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void ChangeSelectedChannelNameCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedChannelNameCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            Mouse.OverrideCursor = Cursors.Wait;

            string newName = SelectedChannelNameTextBox.Text;

            if (!newName.Equals(string.Empty))
            {
                var activeChannel = InputFileManager.Get(ActiveInputFileID).GetChannel(ActiveChannelID);
                if (activeChannel.Name != newName)
                {
                    activeChannel.Name = newName;
                    foreach (InputFileChannelSettingsItem item in ChannelItemsStackPanel.Children)
                    {
                        if (item.ChannelID == ActiveChannelID)
                        {
                            item.ChannelName = newName;
                        }
                    }
                }
            }

            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedChannelNameCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeSelectedChannelNameCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSelectedChannelNameCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeSelectedChannelNameCardButton.Background = ColorManager.Secondary50.ConvertBrush();

            Mouse.OverrideCursor = null;
        }

        private void ChangeSelectedChannelLineWidthCardButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedChannelLineWidthCardButton.Background = ColorManager.Secondary200.ConvertBrush();
        }

        private void ChangeSelectedChannelLineWidthCardButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChangeSelectedChannelLineWidthCardButton.Background = ColorManager.Secondary100.ConvertBrush();

            Mouse.OverrideCursor = Cursors.Wait;

            if (int.TryParse(SelectedChannelLineWidthTextBox.Text, out int lineWidth))
            {
                if (lineWidth > 0)
                {
                    var activeChannel = InputFileManager.Get(ActiveInputFileID).GetChannel(ActiveChannelID);
                    if (activeChannel.LineWidth != lineWidth)
                    {
                        activeChannel.LineWidth = lineWidth;
                        foreach (InputFileChannelSettingsItem item in ChannelItemsStackPanel.Children)
                        {
                            if (item.ChannelID == ActiveChannelID)
                            {
                                item.LineWidth = lineWidth;
                            }
                        }

                        ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).BuildCharts();
                    }
                }
            }

            Mouse.OverrideCursor = null;
        }

        public void ChangeLineWidth(string newLineWidth, int inputFileID, string channelName, bool isGroup, bool change = false)
        {
            if (change)
            {
                if (isGroup)
                {
                    if (int.TryParse(newLineWidth, out int lineWidth))
                    {
                        if (lineWidth > 0)
                        {
                            var activeAttribute = GroupManager.GetGroup(inputFileID).GetAttribute(channelName);
                            if (activeAttribute.LineWidth != lineWidth)
                            {
                                activeAttribute.LineWidth = lineWidth;
                                GroupManager.SaveGroups();
                                ((GroupSettings)((SettingsMenu)MenuManager.GetMenuTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();

                                ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).BuildCharts();
                            }
                        }
                    }
                }
                else
                {
                    if (int.TryParse(newLineWidth, out int lineWidth))
                    {
                        if (lineWidth > 0)
                        {
                            var activeChannel = InputFileManager.Get(inputFileID).GetChannel(channelName);
                            if (activeChannel.LineWidth != lineWidth)
                            {
                                activeChannel.LineWidth = lineWidth;
                                foreach (InputFileChannelSettingsItem item in ChannelItemsStackPanel.Children)
                                {
                                    if (item.ChannelName.Equals(channelName))
                                    {
                                        item.LineWidth = lineWidth;
                                    }
                                }

                                ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).BuildCharts();
                            }
                        }
                    }
                }
            }

            ((DriverlessMenu)MenuManager.GetMenuTab(TextManager.DriverlessMenuName).Content).SetLoadingGrid(visibility: false);
        }

        private void ChangeSelectedChannelLineWidthCardButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeSelectedChannelLineWidthCardButton.Background = ColorManager.Secondary100.ConvertBrush();
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void ChangeSelectedChannelLineWidthCardButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeSelectedChannelLineWidthCardButton.Background = ColorManager.Secondary50.ConvertBrush();
            Mouse.OverrideCursor = null;
        }
    }
}
