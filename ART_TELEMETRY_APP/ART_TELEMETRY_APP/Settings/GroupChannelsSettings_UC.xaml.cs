using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Pilots;
using MaterialDesignThemes.Wpf;
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

namespace ART_TELEMETRY_APP.Settings
{
    /// <summary>
    /// Interaction logic for GroupChannelsSettings_UC.xaml
    /// </summary>
    public partial class GroupChannelsSettings_UC : UserControl
    {
        Pilot pilot;
        ListBox selected_channels_listbox;
        Card selected_channels_settings_card;
        ColorZone channel_settings_nothing;
        Group group;

        public GroupChannelsSettings_UC(Pilot pilot,
                                        ListBox selected_channels_listbox,
                                        ref Group group,
                                        Card selected_channels_settings_card,
                                        ColorZone channel_settings_nothing
                                        )
        {
            InitializeComponent();

            this.pilot = pilot;
            this.selected_channels_listbox = selected_channels_listbox;
            this.group = group;
            this.selected_channels_settings_card = selected_channels_settings_card;
            this.channel_settings_nothing = channel_settings_nothing;

            initFilesCmbbox();
        }

        void initFilesCmbbox()
        {
            files_cmbbox.Items.Clear();
            foreach (InputFile input_file in pilot.InputFiles)
            {
                files_cmbbox.Items.Add(new ComboBoxItem { Content = input_file.FileName });
            }
        }

        private void files_cmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputFile input_file = PilotManager.GetPilot(pilot.Name).GetInputFile(((ComboBoxItem)files_cmbbox.SelectedItem).Content.ToString());
            foreach (Data data in input_file.Datas)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = data.Attribute;
                item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(channelListBoxItemClick);
                channels_listbox.Items.Add(item);
            }
        }

        private void filterChannelsTxtbox_KeyUp(object sender, KeyEventArgs e)
        {
            channels_listbox.Items.Clear();
            InputFile input_file = PilotManager.GetPilot(pilot.Name).GetInputFile(((ComboBoxItem)files_cmbbox.SelectedItem).Content.ToString());

            foreach (Data data in input_file.Datas)
            {
                if (string.IsNullOrEmpty(filterChannelsTxtbox.Text) || data.Attribute.ToUpper().Contains(filterChannelsTxtbox.Text.ToUpper()))
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = data.Attribute;
                    item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(channelListBoxItemClick);
                    channels_listbox.Items.Add(item);
                }
            }
        }

        private void channelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (!containsListBox(selected_channels_listbox, ((ListBoxItem)sender).Content.ToString()))
                {
                    string content = string.Format("{0}[{1}]", ((ListBoxItem)sender).Content.ToString(), pilot.Name);
                    ListBoxItem item = new ListBoxItem();
                    item.Content = content;
                    item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                    selected_channels_listbox.Items.Add(item);

                   // group.SelectedChannelSettingsUCs.Add(new SelectedChannelSettings_UC(content));

                    channel_settings_nothing.Visibility = Visibility.Hidden;
                }

                selected_channels_listbox.SelectedItem = selected_channels_listbox.Items[selected_channels_listbox.Items.Count - 1];
                //selected_channels_settings_card.Content = group.SelectedChannelSettingsUCs.Find(n => n.Attribute == ((ListBoxItem)selected_channels_listbox.SelectedItem).Content.ToString());
            }
        }

        private void selectedChannelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                selected_channels_listbox.Items.Remove(sender);
               /* foreach (SelectedChannelSettings_UC item in group.SelectedChannelSettingsUCs.ToList())
                {
                    if (item.Attribute == ((ListBoxItem)sender).Content.ToString())
                    {
                        group.SelectedChannelSettingsUCs.Remove(item);
                    }
                }*/
                if (selected_channels_listbox.Items.IsEmpty)
                {
                    channel_settings_nothing.Visibility = Visibility.Visible;
                }
                else
                {
                    selected_channels_listbox.SelectedItem = selected_channels_listbox.Items[selected_channels_listbox.Items.Count - 1];
                   // selected_channels_settings_card.Content = group.SelectedChannelSettingsUCs.Find(n => n.Attribute == ((ListBoxItem)selected_channels_listbox.SelectedItem).Content.ToString());
                }
            }
            else
            {
               // selected_channels_settings_card.Content = group.SelectedChannelSettingsUCs.Find(n => n.Attribute == ((ListBoxItem)sender).Content.ToString());
            }
        }

        private bool containsListBox(ListBox listbox, string name)
        {
            foreach (ListBoxItem item in listbox.Items)
            {
                if (item.Content.ToString() == string.Format("{0}[{1}]", name, pilot.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
