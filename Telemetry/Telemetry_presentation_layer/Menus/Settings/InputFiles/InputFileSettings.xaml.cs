﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telemetry_data_and_logic_layer;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_presentation_layer.Converters;
using Telemetry_presentation_layer.Errors;

namespace Telemetry_presentation_layer.Menus.Settings.InputFiles
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
        /// Active <see cref="InputFile"/>s name.
        /// </summary>
        public string ActiveInputFileName { get; set; } = string.Empty;

        /// <summary>
        /// Active <see cref="Channel"/>.
        /// </summary>
        public Channel ActiveChannel { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public InputFilesSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds an <see cref="InputFile"/>, updates <see cref="ActiveInputFileName"/> ,initializes <see cref="Channel"/> items.
        /// </summary>
        /// <param name="inputFile">The <see cref="InputFile"/> you want to add.</param>
        public void AddInputFileSettingsItem(InputFile inputFile)
        {
            InitActiveChannel();
            ActiveInputFileName = inputFile.Name;

            InitChannelItems();
            ChangeAllInputFileSettingsItemColorMode();
            AddSingleInputFileSettingsItem(inputFile);
        }

        /// <summary>
        /// Changes the <see cref="ActiveInputFileName"/> to <paramref name="inputFileName"/> and updates channel items.
        /// </summary>
        /// <param name="inputFileName"><see cref="InputFile"/>s name that will be the <see cref="ActiveInputFileName"/>.</param>
        public void ChangeActiveInputFileSettingsItem(string inputFileName)
        {
            ChangeAllInputFileSettingsItemColorMode();
            ActiveInputFileName = inputFileName;
            InitChannelItems();
        }

        /// <summary>
        /// Changes all <see cref="InputFileSettingsItem"/>s color mode to unselected in <see cref="InputFileStackPanel"/>.
        /// </summary>
        private void ChangeAllInputFileSettingsItemColorMode()
        {
            foreach (InputFileSettingsItem item in InputFileStackPanel.Children)
            {
                item.ChangeColorMode(selected: false);
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
                   !((InputFileSettingsItem)InputFileStackPanel.Children[index]).InputFileNameLbl.Content.Equals(inputFileName))
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
            UpdateActiveInputFileName();

            InputFileStackPanel.Children.Clear();
            InitInputFileSettingsItemElements();

            InitActiveChannel();
            InitChannelItems();
        }

        /// <summary>
        /// Sets <see cref="ActiveInputFileName"/> to <see cref="DriverlessInputFileManager"/>s or <see cref="StandardInputFileManager"/>s first <see cref="InputFile"/>s name if there is any <see cref="InputFile"/>.
        /// </summary>
        private void UpdateActiveInputFileName()
        {
            if (ActiveInputFileName.Equals(string.Empty))
            {
                if (InputFileManager.InputFiles.Count > 0)
                {
                    ActiveInputFileName = InputFileManager.InputFiles.First().Name;
                }
                else
                {
                    ActiveInputFileName = string.Empty;
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
            var inputFileSettingsItem = new InputFileSettingsItem(inputFileName: inputFile.Name, driverless: inputFile.Driverless);
            inputFileSettingsItem.ChangeColorMode(inputFile.Name.Equals(ActiveInputFileName));
            InputFileStackPanel.Children.Add(inputFileSettingsItem);
            inputFileSettingsItems.Add(inputFileSettingsItem);
        }

        /// <summary>
        /// Initializes <see cref="ActiveChannel"/>.
        /// </summary>
        private void InitActiveChannel()
        {
            var activeInputFile = InputFileManager.GetInputFile(ActiveInputFileName);
            if (activeInputFile != null)
            {
                if (activeInputFile.Channels.Count > 0)
                {
                    ActiveChannel = activeInputFile.Channels.First();
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

            var activeInputFile = InputFileManager.GetInputFile(ActiveInputFileName);
            if (activeInputFile != null)
            {
                foreach (var channel in activeInputFile.Channels)
                {
                    AddInputFileChannelSettingsItem(channel);
                }
            }

            UpdateRequiredChannels();
        }

        /// <summary>
        /// Creates and adds an <see cref="InputFileChannelSettingsItem"/> to <see cref="inputFileChannelSettingsItems"/>.
        /// </summary>
        /// <param name="channel"></param>
        private void AddInputFileChannelSettingsItem(Channel channel)
        {
            var inputFileChannelSettingsItem = new InputFileChannelSettingsItem(channel, ActiveInputFileName);
            ChannelItemsStackPanel.Children.Add(inputFileChannelSettingsItem);
            inputFileChannelSettingsItems.Add(inputFileChannelSettingsItem);
        }

        /// <summary>
        /// Reads file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadInputFileBtn_Click(object sender, RoutedEventArgs e)
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

                    if (InputFileManager.GetInputFile(fileName) == null)
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
                    ShowError.ShowErrorMessage(exception.Message);
                }
            }
        }

        /// <summary>
        /// Updates required <see cref="Channel"/>s.
        /// </summary>
        public void UpdateRequiredChannels()
        {
            RequiredChannelsStackPanel.Children.Clear();

            var activeInputFile = InputFileManager.GetInputFile(ActiveInputFileName);

            if (activeInputFile != null)
            {
                if (activeInputFile.Driverless)
                {
                    foreach (var importantChannelName in ImportantChannels.DriverlessImportantChannelNames)
                    {
                        AddImportantChannelNameCheckBox(importantChannelName, activeInputFile.IsRequiredChannelSatisfied(importantChannelName));
                    }
                }
                else
                {
                    //TODO standarddal is
                    /*foreach (var importantChannelName in ImportantChannels.DriverlessImportantChannelNames)
                    {
                        AddImportantChannelNameCheckBox(importantChannelName);
                    }*/
                }
            }
        }

        /// <summary>
        /// Creates and adds a <see cref="CheckBox"/> based on <paramref name="name"/> and <paramref name="isChecked"/>.
        /// </summary>
        /// <param name="name">The newly created <see cref="CheckBox"/>s name.</param>
        /// <param name="isChecked">
        /// If true, the <see cref="CheckBox"/> will be checked.
        /// If false, the <see cref="CheckBox"/> will not be checked.
        /// </param>
        private void AddImportantChannelNameCheckBox(string name, bool isChecked)
        {
            RequiredChannelsStackPanel.Children.Add(new CheckBox()
            {
                Content = name,
                IsChecked = isChecked,
                IsEnabled = false
            });
        }
    }
}
