using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.Errors.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.InputFiles.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP.Settings.UserControls
{
    /// <summary>
    /// Interaction logic for InputFilesSettings.xaml
    /// </summary>
    public partial class InputFilesSettings : UserControl
    {
        private readonly List<InputFileSettingsItem> inputFileSettingsItems = new List<InputFileSettingsItem>();

        private readonly List<InputFileChannelSettingsItem> inputFileChannelSettingsItems = new List<InputFileChannelSettingsItem>();

        public string ActiveInputFileName { get; set; } = string.Empty;

        public Channel ActiveChannel { get; set; }

        public InputFilesSettings()
        {
            InitializeComponent();
        }

        public void InitInputFileSettingsItems()
        {
            UpdateActiveInputFileName();
            InitDriverlessInputFileSettingsItems();
            //TODO standarddal is
            InitChannelItems();
        }

        private void UpdateActiveInputFileName()
        {
            if (ActiveInputFileName.Equals(string.Empty))
            {
                if (DriverlessInputFileManager.Instance.InputFiles.Count > 0)
                {
                    ActiveInputFileName = DriverlessInputFileManager.Instance.InputFiles.First().Name;
                }
                else
                {
                    ActiveInputFileName = string.Empty;
                }
            }
        }

        private void InitDriverlessInputFileSettingsItems()
        {
            InputFileStackPanel.Children.Clear();

            foreach (var inputFile in DriverlessInputFileManager.Instance.InputFiles)
            {
                var inputFileSettingsItem = new InputFileSettingsItem(inputFileName: inputFile.Name, driverless: true);
                inputFileSettingsItem.ChangeColorMode(inputFile.Name.Equals(ActiveInputFileName));
                InputFileStackPanel.Children.Add(inputFileSettingsItem);
                inputFileSettingsItems.Add(inputFileSettingsItem);
            }

            var activeInputFile = DriverlessInputFileManager.Instance.GetInputFile(ActiveInputFileName);
            if (activeInputFile != null)
            {
                if (activeInputFile.Channels.Count > 0)
                {
                    ActiveChannel = activeInputFile.Channels.First();
                }
            }
        }

        public void InitChannelItems()
        {
            ChannelItemsStackPanel.Children.Clear();
            inputFileChannelSettingsItems.Clear();

            //TODO standarddal is

            var activeInputFile = DriverlessInputFileManager.Instance.GetInputFile(ActiveInputFileName);
            if (activeInputFile != null)
            {
                foreach (var channel in activeInputFile.Channels)
                {
                    var inputFileChannelSettingsItem = new InputFileChannelSettingsItem(channel.Name, ActiveInputFileName, channel.Color);
                    ChannelItemsStackPanel.Children.Add(inputFileChannelSettingsItem);
                    inputFileChannelSettingsItems.Add(inputFileChannelSettingsItem);
                }
            }
        }

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
                string fileName = openFileDialog.FileName.Split('\\').Last();

                if (DriverlessInputFileManager.Instance.InputFiles.Find(x => x.Name.Equals(fileName)) == null /*|| TODO: standarddal is*/)
                {
                    ReadFileProgressBarLbl.Content = $"Reading \"{fileName}\"";
                    var dataReader = new DataReader();
                    dataReader.ReadData(openFileDialog.FileName,
                                        ReadFileProgressBarGrid,
                                        ReadFileProgressBar,
                                        fileType: FileType.Driverless);
                }
                else
                {
                    ShowError.ShowErrorMessage(ref ErrorSnackbar, $"File '{fileName}' already exists");
                }
            }
        }
    }
}
