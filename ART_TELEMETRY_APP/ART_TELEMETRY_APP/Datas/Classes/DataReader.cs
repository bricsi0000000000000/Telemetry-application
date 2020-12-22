using ART_TELEMETRY_APP.Driverless.UserControls;
using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.Groups.UserControls;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.InputFiles.UserControls;
using ART_TELEMETRY_APP.Settings;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Settings.UserControls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Datas.Classes
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

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="progressBarGrid"></param>
        /// <param name="progressBar"></param>
        /// <param name="fileType"></param>
        public void ReadData(string fileName, Grid progressBarGrid, ProgressBar progressBar, FileType fileType)
        {
            this.fileName = fileName;
            this.progressBarGrid = progressBarGrid;
            this.progressBar = progressBar;
            this.fileType = fileType;

            this.progressBarGrid.Visibility = Visibility.Visible;
            this.progressBar.IsIndeterminate = false;
            this.progressBar.Value = 0;

            fileLength = File.ReadLines(fileName).Count();

            StartWorker();
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
            NumberFormatInfo numberFormatInfo = new CultureInfo("hu-HU", false).NumberFormat;

            channels = new List<Channel>();

            using var reader = new StreamReader(fileName, Encoding.Default);

            string[] channelNames = reader.ReadLine().Split(';');
            foreach (var channelName in channelNames)
            {
                channels.Add(new Channel(channelName));
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
                        channels[i].AddChannelData(float.Parse(row[i], numberFormatInfo));
                    }
                }

                worker.ReportProgress(Convert.ToInt32(progressIndex++ / (float)fileLength * 100));
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
            progressBarGrid.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = true;

            switch (fileType)
            {
                case FileType.Standard:
                    break;
                case FileType.Driverless:
                    DriverlessInputFileManager.Instance.AddInputFile(new DriverlessInputFile(FileNameWithoutPath, channels));

                    ((DriverlessMenu)MenuManager.GetTab(TextManager.DriverlessMenuName).Content).UpdateAfterReadFile();
                    ((GroupSettings)((SettingsMenu)MenuManager.GetTab(TextManager.SettingsMenuName).Content).GetTab(TextManager.GroupsSettingsName).Content).InitGroups();
                    break;
            }
        }
    }
}
