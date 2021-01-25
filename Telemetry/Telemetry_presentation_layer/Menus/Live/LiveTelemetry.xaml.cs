using System.Windows;
using System.Windows.Controls;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.Models;

namespace Telemetry_presentation_layer.Menus.Live
{
    /// <summary>
    /// Interaction logic for LiveTelemetry.xaml
    /// </summary>
    public partial class LiveTelemetry : UserControl
    {
        private Section activeSection;
        public LiveTelemetry()
        {
            InitializeComponent();

            InitializeGroups();
            UpdateSectionTitle();
        }

        private void UpdateSectionTitle()
        {
            if (activeSection == null)
            {
                SectionNameTextBox.Text = "No active section";
            }
            else
            {
                SectionNameTextBox.Text = activeSection.Name;
                SectionDateLabel.Text = activeSection.Date.ToString();
            }
        }

        private void InitializeGroups()
        {
            foreach (var group in GroupManager.Groups)
            {
                var checkBox = new CheckBox()
                {
                    Content = group.Name
                };
                checkBox.Click += GroupCheckBox_Click;

                GroupsStackPanel.Children.Add(checkBox);
            }
        }

        private void GroupCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string content = checkBox.Content.ToString();
            if ((bool)checkBox.IsChecked)
            {
            }
            else
            {
            }
        }

        private void InitializeChannels()
        {

        }

        public void UpdateSection(Section section)
        {
            activeSection = section;

            UpdateSectionTitle();
            InitializeChannels();
            InitializeCharts();
        }

        private void InitializeCharts()
        {

        }

        private void DataSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
