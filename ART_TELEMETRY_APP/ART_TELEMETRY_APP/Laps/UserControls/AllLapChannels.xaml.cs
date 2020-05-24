using ART_TELEMETRY_APP.Pilots;
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
    /// Interaction logic for AllLapChannels.xaml
    /// </summary>
    public partial class AllLapChannels : Window
    {
        string pilots_name;
        List<string> channels;
        List<string> selected_channels;
        List<string> new_selected_channels = new List<string>();

        public AllLapChannels(string pilots_name, List<string> channels, List<string> selected_channels)
        {
            InitializeComponent();

            this.pilots_name = pilots_name;
            this.channels = channels;
            this.selected_channels = selected_channels;
            //this.selected_channels = selected_channels;

            initChannelsListBox();
            initSelectedChannels();
            initSelectedChannelsListBox();
        }

        private void initSelectedChannels()
        {
            new_selected_channels.Clear();
            foreach (string attribute in selected_channels)
            {
                new_selected_channels.Add(attribute);
            }
        }

        private void initSelectedChannelsListBox()
        {
            selected_channels_listbox.Items.Clear();
            foreach (string attribute in new_selected_channels)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute;
                item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                selected_channels_listbox.Items.Add(item);
            }
        }
        /*  private void initSelectedChannels()
          {
              new_selected_channels.Clear();
              foreach (string attribute in selected_channels)
              {
                  new_selected_channels.Add(attribute);
              }
          }

          private void initSelectedChannelsListBox()
          {
              selected_channels_listbox.Items.Clear();
              foreach (string attribute in new_selected_channels)
              {
                  ListBoxItem item = new ListBoxItem();
                  item.Content = attribute;
                  item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                  selected_channels_listbox.Items.Add(item);
              }
          }
          */
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
        }

        private void channelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string attribute = ((ListBoxItem)sender).Content.ToString();
                new_selected_channels.Add(attribute);
                updateSelectedListBoxItems();
            }
        }

        private void updateSelectedListBoxItems()
        {
            selected_channels_listbox.Items.Clear();
            foreach (string attribute in new_selected_channels)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute;
                item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                selected_channels_listbox.Items.Add(item);
            }
        }

        private void selectedChannelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string attribute = ((ListBoxItem)sender).Content.ToString();
                new_selected_channels.Remove(attribute);
                updateSelectedListBoxItems();
            }
        }
        /*
        private void updateLapSelectedChannels()
        {
            selected_channels.Clear();
            foreach (string attribute in new_selected_channels)
            {
                selected_channels.Add(attribute);
            }
        }

        private void channelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string attribute = ((ListBoxItem)sender).Content.ToString();
                new_selected_channels.Add(attribute);
                updateSelectedListBoxItems();
                updateLapSelectedChannels();
            }
        }

        private void updateSelectedListBoxItems()
        {
            selected_channels_listbox.Items.Clear();
            foreach (string attribute in new_selected_channels)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute;
                item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                selected_channels_listbox.Items.Add(item);
            }
        }

        private void selectedChannelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string attribute = ((ListBoxItem)sender).Content.ToString();
                new_selected_channels.Remove(attribute);
                updateSelectedListBoxItems();
                updateLapSelectedChannels();
            }
        }
        */
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
            foreach (var attribute in new_selected_channels)
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

        private void Window_Closed(object sender, EventArgs e)
        {
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).ChangeAllSelectedChannels(new_selected_channels);
            ((LapsContent)((PilotContentTab)((DatasMenuContent)TabManager.GetTab("Diagrams").Content).GetTab(pilots_name).Content).GetTab("Laps").Content).BuildCharts();
        }
    }
}
