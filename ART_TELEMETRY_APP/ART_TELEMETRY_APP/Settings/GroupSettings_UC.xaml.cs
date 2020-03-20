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
    /// Interaction logic for GroupSettings_UC.xaml
    /// </summary>
    public partial class GroupSettings_UC : UserControl
    {
        Group group;

        public GroupSettings_UC(ref Group group
                               )
        {
            InitializeComponent();

            this.group = group;

            initPilots();
        }

        private void initPilots()
        {
            foreach (Pilot pilot in PilotManager.Pilots)
            {
                CheckBox check_box = new CheckBox();
                check_box.Content = pilot.Name;
                check_box.Margin = new Thickness(5);
                check_box.Checked += new RoutedEventHandler(pilot_checkbox_Checked);
                check_box.Unchecked += new RoutedEventHandler(pilot_checkbox_Unchecked);
                pilots_wrappanel.Children.Add(check_box);
            }
        }

        private void pilot_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            group.Pilots.Add(PilotManager.GetPilot(((CheckBox)sender).Content.ToString()));
            updateChannelsListBox();
        }

        private void pilot_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in group.Pilots.ToList())
            {
                if (item.Name == ((CheckBox)sender).Content.ToString())
                {
                    group.Pilots.Remove(item);
                }
            }
            updateChannelsListBox();
        }

        private void updateChannelsListBox()
        {
            HashSet<string> same_channels = new HashSet<string>();
            List<string> rest_channels = new List<string>();

            foreach (Pilot pilot in group.Pilots)
            {
                foreach (InputFile input_file in pilot.InputFiles)
                {
                    foreach (Data data in input_file.Datas)
                    {
                        same_channels.Add(data.Name);
                    }
                }
            }

            Console.WriteLine(same_channels.Count);
        }

        private void filterChannelsTxtbox_KeyUp(object sender, KeyEventArgs e)
        {
            /*List<ListBoxItem> items = new List<ListBoxItem>();
            foreach (string attribute in group.SelectedChannels)
            {
                if (string.IsNullOrEmpty(filter_channels_txtbox.Text) || attribute.ToUpper().Contains(filter_channels_txtbox.Text.ToUpper()))
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = attribute;
                    item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                    items.Add(item);
                }
            }

            channels_listbox.Items.Clear();
            foreach (ListBoxItem item in items)
            {
                channels_listbox.Items.Add(item);
            }*/
        }


        /* private void selectedChannelListBoxItemClick(object sender, MouseButtonEventArgs e)
         {
             if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
             {
                 selected_channels_listbox.Items.Remove(sender);
                 foreach (SelectedChannelSettings_UC item in group.SelectedChannelSettingsUCs)
                 {
                     if (item.Attribute == ((ListBoxItem)sender).Content.ToString())
                     {
                         group.SelectedChannelSettingsUCs.Remove(item);
                     }
                 }

                 if (selected_channels_listbox.Items.IsEmpty)
                 {
                     channel_settings_nothing.Visibility = Visibility.Visible;
                 }
                 else
                 {
                     selected_channels_listbox.SelectedItem = selected_channels_listbox.Items[selected_channels_listbox.Items.Count - 1];
                     selected_channels_settings_card.Content = group.SelectedChannelSettingsUCs.Find(n => n.Attribute == ((ListBoxItem)selected_channels_listbox.SelectedItem).Content.ToString());
                 }
             }
             else
             {
                 selected_channels_settings_card.Content = group.SelectedChannelSettingsUCs.Find(n => n.Attribute == ((ListBoxItem)sender).Content.ToString());
             }
         }*/

        /*   public void UpdateSelectedChannel()
           {
               if (selected_channels_listbox.Items.IsEmpty)
               {
                   channel_settings_nothing.Visibility = Visibility.Visible;
               }
               else
               {
                   selected_channels_listbox.SelectedItem = selected_channels_listbox.Items[selected_channels_listbox.Items.Count - 1];
                   selected_channels_settings_card.Content = group.SelectedChannelSettingsUCs.Find(n => n.Attribute == ((ListBoxItem)selected_channels_listbox.SelectedItem).Content.ToString());
               }
           }
           */

        string name;

        public string GroupName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}
