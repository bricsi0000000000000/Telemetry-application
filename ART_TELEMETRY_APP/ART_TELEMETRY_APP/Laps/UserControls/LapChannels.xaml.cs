using ART_TELEMETRY_APP.Laps;
using ART_TELEMETRY_APP.Pilots;
using ART_TELEMETRY_APP.Settings.Classes;
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
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// Interaction logic for LapChannels.xaml
    /// </summary>
    public partial class LapChannels : Window
    {
        Lap lap;
        List<string> channels;
        List<string> selected_channels = new List<string>();
        string pilots_name;
        string group_name;

        public LapChannels(Lap lap, List<string> channels, string pilots_name, string group_name)
        {
            InitializeComponent();

            this.Title = string.Format("Select channels to {0}. lap", lap.Index);

            this.lap = lap;
            this.channels = channels;
            this.pilots_name = pilots_name;
            this.group_name = group_name;

            initSelectedChannels();
            initChannelsListBox();
            initSelectedChannelsListBox();

            kalman_filter_sensitivity_txtbox.Text =
                ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(group_name).Content).GetLapListElement(lap.Index).KalmanSensitivity.ToString();
        }

        private void initSelectedChannels()
        {
            selected_channels.Clear();
            foreach (string attribute in lap.SelectedChannels)
            {
                selected_channels.Add(attribute);
            }
        }

        private void initSelectedChannelsListBox()
        {
            selected_channels_listbox.Items.Clear();
            foreach (string attribute in selected_channels)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute;
                item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                selected_channels_listbox.Items.Add(item);
            }
            selected_channels_lbl.Content = string.Format("Selected channels ({0})", selected_channels.Count);
        }

        private void initChannelsListBox()
        {
            channels_listbox.Items.Clear();
            foreach (string attribute in channels)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute;
                item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(channelListBoxItemClick);
                channels_listbox.Items.Add(item);
            }
            all_channels_lbl.Content = string.Format("All channels ({0})", channels.Count);
        }

        private void updateLapSelectedChannels()
        {
            lap.SelectedChannels.Clear();
            foreach (string attribute in selected_channels)
            {
                lap.SelectedChannels.Add(attribute);
            }
        }

        private void channelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string attribute = ((ListBoxItem)sender).Content.ToString();
                addToSelectedChannels(attribute);
                updateSelectedListBoxItems();
                updateLapSelectedChannels();
            }
        }

        private void addToSelectedChannels(string attribute)
        {
            if (!selected_channels.Contains(attribute))
            {
                selected_channels.Add(attribute);
            }
            else
            {
                error_snack_bar.MessageQueue.Enqueue(string.Format("'{0}' is already selected!", attribute),
                                                     null, null, null, false, true, TimeSpan.FromSeconds(1));
            }
        }

        private void updateSelectedListBoxItems()
        {
            selected_channels_listbox.Items.Clear();
            foreach (string attribute in selected_channels)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute;
                item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                selected_channels_listbox.Items.Add(item);
            }
            selected_channels_lbl.Content = string.Format("Selected channels ({0})", selected_channels.Count);
        }

        private void selectedChannelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string attribute = ((ListBoxItem)sender).Content.ToString();
                selected_channels.Remove(attribute);
                updateSelectedListBoxItems();
                updateLapSelectedChannels();
            }
        }

        private void filterChannelsTxtbox_KeyUp(object sender, KeyEventArgs e)
        {
            List<ListBoxItem> items = new List<ListBoxItem>();
            foreach (var attribute in channels)
            {
                if (string.IsNullOrEmpty(filter_channels_txtbox.Text) || attribute.ToUpper().Contains(filter_channels_txtbox.Text.ToUpper()))
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = attribute;
                    item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(channelListBoxItemClick);
                    items.Add(item);
                }
            }

            channels_listbox.Items.Clear();
            foreach (ListBoxItem item in items)
            {
                channels_listbox.Items.Add(item);
            }
        }

        private void filterSelectedChannelsTxtbox_KeyUp(object sender, KeyEventArgs e)
        {
            List<ListBoxItem> items = new List<ListBoxItem>();
            foreach (var attribute in selected_channels)
            {
                if (string.IsNullOrEmpty(filter_selected_channels_txtbox.Text) || attribute.ToUpper().Contains(filter_selected_channels_txtbox.Text.ToUpper()))
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = attribute;
                    item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                    items.Add(item);
                }
            }

            selected_channels_listbox.Items.Clear();
            foreach (ListBoxItem item in items)
            {
                selected_channels_listbox.Items.Add(item);
            }
        }

        private void kalmanFilterSensitivityTxtbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (float.TryParse(kalman_filter_sensitivity_txtbox.Text, out float sensitivity))
            {
                saveKalmanSensitivity();
            }
        }

        private void saveKalmanSensitivity()
        {
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(group_name).Content).GetLapListElement(lap.Index).KalmanSensitivity =
                    float.Parse(kalman_filter_sensitivity_txtbox.Text);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            saveKalmanSensitivity();
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab(TextManager.DiagramsMenuName).Content).GetTab(pilots_name).Content).GetTab(group_name).Content).BuildCharts();
        }
    }
}
