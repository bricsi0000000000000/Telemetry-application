using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Telemetry_data_and_logic_layer;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.InputFiles;
using Telemetry_data_and_logic_layer.Texts;
using Telemetry_presentation_layer.Errors;
using Telemetry_presentation_layer.Menus;
using Telemetry_presentation_layer.Menus.Driverless;
using Telemetry_presentation_layer.Menus.Settings;
using Telemetry_presentation_layer.Menus.Settings.Groups;
using Telemetry_presentation_layer.Menus.Settings.InputFiles;

namespace Telemetry_presentation_layer
{
    /// <summary>
    /// Reads a files content.
    /// </summary>
    public class DataReader
    {
        private string fileName;
        private Grid progressBarGrid;
        private ProgressBar progressBar;
        private long fileLength;
        private BackgroundWorker worker;
        private List<Channel> channels;
        private FileType fileType;
        private bool processingError = false;

        public void SetupReader(Grid progressBarGrid,
                                ProgressBar progressBar,
                                FileType fileType)
        {
            this.progressBarGrid = progressBarGrid;
            this.progressBar = progressBar;
            this.fileType = fileType;

            progressBarGrid.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = false;
            progressBar.Value = 0;
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="progressBarGrid"></param>
        /// <param name="progressBar"></param>
        /// <param name="fileType"></param>
        public void ReadFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception($"{fileName} not found!");
            }

            this.fileName = fileName;

            StartWorker();
        }

        public void ProcessFile(string fileName)
        {
            if (processingError)
            {
                return;
            }

            if (new FileInfo(fileName).Length == 0)
            {
                throw new Exception($"{fileName} is empty");
            }

            fileLength = File.ReadLines(fileName).Count();

            NumberFormatInfo numberFormatInfo = new CultureInfo("hu-HU", false).NumberFormat;

            channels = new List<Channel>();

            using var reader = new StreamReader(fileName, Encoding.Default);

            string[] channelNames = reader.ReadLine().Split(';');
            foreach (var channelName in channelNames)
            {
                if (channelName.Equals(string.Empty))
                {
                    throw new Exception($"Channels name is empty in {fileName}");
                }

                var channel = new Channel(channelName);

                foreach (var group in GroupManager.Groups)
                {
                    foreach (var attribute in group.Attributes)
                    {
                        if (attribute.Name.Equals(channel.Name))
                        {
                            channel.Color = attribute.Color;
                        }
                    }
                }

                channels.Add(channel);
            }

            uint progressIndex = 0;

            while (!reader.EndOfStream)
            {
                string[] row = reader.ReadLine().Split(';');
                for (ushort i = 0; i < channels.Count; i++)
                {
                    if (row[i].Equals(string.Empty))
                    {
                        channels[i].AddChannelData(float.NaN);
                    }
                    else
                    {
                        if (float.TryParse(row[i], NumberStyles.Float, numberFormatInfo, out float rowValue))
                        {
                            channels[i].AddChannelData(rowValue);
                        }
                        else
                        {
                            throw new Exception($"Can't convert {row[i]} into number in {fileName}");
                        }
                    }
                }

                ReportWorkerProgress(Convert.ToInt32(progressIndex++ / (float)fileLength * 100));
            }
        }

        private void ReportWorkerProgress(int progress)
        {
            if (worker != null)
            {
                worker.ReportProgress(progress);
            }
        }

        private void StartWorker()
        {
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += WorkerDoWork;
            worker.ProgressChanged += WorkerProgressChanged;
            worker.RunWorkerCompleted += WorkerCompleted;
            worker.RunWorkerAsync();
        }

        private string FileNameWithoutPath => fileName.Split('\\').Last();

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProcessFile(fileName);
            }
            catch (Exception exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowError.ShowErrorMessage(exception.Message);
                    processingError = true;
                });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                progressBar.Value = 0;
                progressBar.IsIndeterminate = true;
            });
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (progressBar != null && progressBarGrid != null)
            {
                progressBarGrid.Visibility = Visibility.Hidden;
                progressBar.IsIndeterminate = true;
            }

            if (!processingError)
            {
                var inputFile = new DriverlessInputFile(FileNameWithoutPath, channels);

                //TODO: ha lesz standard input file is, akkor azt még előbb kell eldönteni vagy utána rögtön válassza ki és utána nézzem meg hogy ezek benne vannak e.
                var yChannel = inputFile.GetChannel("y");
                if (yChannel == null)
                {
                    ShowError.ShowErrorMessage("Can't find 'y' channel, so the track will not shown");
                }

                var c0refChannel = inputFile.GetChannel("c0ref");
                if (c0refChannel == null)
                {
                    ShowError.ShowErrorMessage("Can't find 'c0ref' channel, so the track will not shown");
                }

                InputFileManager.AddInputFile(inputFile);
                InputFileManager.ActiveInputFileName = FileNameWithoutPath;
                ((InputFilesSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.FilesSettingsName).Content).AddInputFileSettingsItem(inputFile);
                ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateAfterReadFile();
                ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).UpdateAfterReadFile(FileNameWithoutPath);
            }
        }
    }
}
