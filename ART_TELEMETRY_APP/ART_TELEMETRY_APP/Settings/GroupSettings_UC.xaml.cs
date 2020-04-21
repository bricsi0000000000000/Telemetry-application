using ART_TELEMETRY_APP.InputFiles;
using ART_TELEMETRY_APP.Pilots;
using LiveCharts;
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
        List<string> channel_attributes = new List<string>();
        TabItem tab;
        string active_selected_channel;

        public GroupSettings_UC(Group group, ref TabItem tab)
        {
            InitializeComponent();

            this.group = group;
            this.tab = tab;

            InitPilots();

            selected_channels_properties_nothing.Visibility = Visibility.Visible;
        }

        public void InitPilots()
        {
          //  selected_pilots_nothing_lbl.Content = "Select pilot(s)";
            pilots_wrappanel.Children.Clear();
            foreach (Pilot pilot in PilotManager.Pilots)
            {
                CheckBox check_box = new CheckBox();
                check_box.Content = pilot.Name;
                if (group.Pilots.Find(n => n.Name == pilot.Name) != null)
                {
                    check_box.IsChecked = true;
                   // selected_pilots_nothing_lbl.Content = "Add file to the pilot";
                }
                check_box.Margin = new Thickness(5);
                check_box.Checked += new RoutedEventHandler(pilot_checkbox_Checked);
                check_box.Unchecked += new RoutedEventHandler(pilot_checkbox_Unchecked);
                pilots_wrappanel.Children.Add(check_box);
                InitSelectedChannelsList();
            }
        }

        private void pilot_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            group.Pilots.Add(PilotManager.GetPilot(((CheckBox)sender).Content.ToString()));
          //  selected_pilots_nothing_lbl.Content = "Add file to the pilot";
            InitSelectedChannelsList();
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
            if (group.Pilots.Count <= 0)
            {
              //  selected_pilots_nothing_lbl.Content = "Select pilot(s)";
            }
            makeChannelsList();
        }

        private void makeChannelsList()
        {
            List<Tuple<string, string>> attributes = new List<Tuple<string, string>>();

            foreach (Pilot pilot in group.Pilots)
            {
                foreach (InputFile input_file in pilot.InputFiles)
                {
                    foreach (Data data in input_file.Datas)
                    {
                        attributes.Add(new Tuple<string, string>(data.Attribute, pilot.Name));
                    }
                }
            }

            updateChannelsListboxItems(ref attributes);
        }

        private void updateChannelsListboxItems(ref List<Tuple<string, string>> attributes)
        {
            channels_listbox.Items.Clear();
            channel_attributes.Clear();

            foreach (var attribute in attributes)
            {
                string content = string.Format("{0} [{1}]", attribute.Item1, attribute.Item2);
                channel_attributes.Add(content);
                ListBoxItem item = new ListBoxItem();
                item.Content = content;
                item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(channelListBoxItemClick);
                channels_listbox.Items.Add(item);
            }

            if (channels_listbox.Items.Count > 0)
            {
               // selected_pilots_nothing.Visibility = Visibility.Hidden;
            }
            else
            {
              //  selected_pilots_nothing.Visibility = Visibility.Visible;
            }
        }

        public void InitSelectedChannelsList()
        {
            makeChannelsList();

            //TODO a listákat összerakni

            selected_channels_listbox.Items.Clear();
            foreach (Data attribute in GroupManager.GetGroup(group.Name).SelectedChannels)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute.Attribute;
                item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                selected_channels_listbox.Items.Add(item);
            }

            if (selected_channels_listbox.Items.Count > 0)
            {
                selected_channels_nothing.Visibility = Visibility.Hidden;
            }
            else
            {
                selected_channels_nothing.Visibility = Visibility.Visible;
            }
        }

        private void channelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string attribute = ((ListBoxItem)sender).Content.ToString();
                if (!containsListBox(selected_channels_listbox, attribute))
                {
                    //channel_attributes.Add(attribute);
                    ListBoxItem item = new ListBoxItem();
                    item.Content = attribute;
                    item.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                    selected_channels_listbox.Items.Add(item);

                    Data single_data = new Data();

                    foreach (Pilot pilot in GroupManager.GetGroup(group.Name).Pilots)
                    {
                        if (pilot.Name == attribute.Split(' ')[1].Substring(1, attribute.Split(' ')[1].Length - 2))
                        {
                            foreach (InputFile inputfile in pilot.InputFiles)
                            {
                                foreach (Data data in inputfile.Datas)
                                {
                                    if (data.Attribute == attribute.Split(' ')[0])
                                    {
                                        single_data = data;
                                    }
                                }
                            }
                        }
                    }

                   /* single_data.Option = new LineSerieOptions
                    {
                        stroke_thickness = .7f,
                        stroke_color = Brushes.White
                    };*/

                    GroupManager.GetGroup(group.Name).SelectedChannels.Add(single_data);

                    selected_channels_nothing.Visibility = Visibility.Hidden;
                }
            }
        }

        private bool containsListBox(ListBox listbox, string name)
        {
            foreach (ListBoxItem item in listbox.Items)
            {
                if (item.Content.ToString() == name)
                {
                    return true;
                }
            }

            return false;
        }

        private void selectedChannelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                string attribute = ((ListBoxItem)sender).Content.ToString();
                selected_channels_listbox.Items.Remove(sender);
                channel_attributes.Remove(attribute);
                GroupManager.GetGroup(group.Name).SelectedChannels.Remove(
                    GroupManager.GetGroup(group.Name).SelectedChannels.Find(n => n.Attribute == attribute)
                    );

                if (selected_channels_listbox.Items.Count <= 0)
                {
                    selected_channels_nothing.Visibility = Visibility.Visible;
                }
            }
            else
            {
                selected_channels_properties_nothing.Visibility = Visibility.Hidden;
                active_selected_channel = ((ListBoxItem)sender).Content.ToString();
                stroke_thickness_txtbox.Text = GroupManager.GetGroup(group.Name).SelectedChannels.Find(n => n.Attribute == active_selected_channel.Split(' ')[0]).Option.stroke_thickness.ToString();
                stroke_color_colorpicker.Color = ((SolidColorBrush)GroupManager.GetGroup(group.Name).SelectedChannels.Find(n => n.Attribute == active_selected_channel.Split(' ')[0]).Option.stroke_color).Color;
            }
        }


        private void filterChannelsTxtbox_KeyUp(object sender, KeyEventArgs e)
        {
            List<ListBoxItem> items = new List<ListBoxItem>();
            foreach (var attribute in channel_attributes)
            {
                if (string.IsNullOrEmpty(filter_channels_txtbox.Text) || attribute.ToUpper().Contains(filter_channels_txtbox.Text.ToUpper()))
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = attribute;
                    item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(channelListBoxItemClick);
                    items.Add(item);
                }
            }

            channels_listbox.Items.Clear();
            foreach (ListBoxItem item in items)
            {
                channels_listbox.Items.Add(item);
            }
        }

        private void changeGroupNameTxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!change_group_name_txtbox.Text.Equals(string.Empty))
                {
                    GroupManager.ChangeGroupName(group.Name, change_group_name_txtbox.Text);
                    GroupManager.ActiveGroup = change_group_name_txtbox.Text;
                    tab.Header = change_group_name_txtbox.Text;
                    change_group_name_txtbox.Text = string.Empty;
                }
            }
        }

        private void filterSelectedChannelsTxtbox_KeyUp(object sender, KeyEventArgs e)
        {
            List<ListBoxItem> items = new List<ListBoxItem>();
            if (string.IsNullOrEmpty(filter_selected_channels_txtbox.Text))
            {
                foreach (Data attribute in GroupManager.GetGroup(group.Name).SelectedChannels)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = attribute.Attribute;
                    item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(selectedChannelListBoxItemClick);
                    items.Add(item);
                }
            }
            else
            {
                foreach (ListBoxItem attribute in selected_channels_listbox.Items)
                {
                    if (attribute.Content.ToString().ToUpper().Contains(filter_selected_channels_txtbox.Text.ToUpper()))
                    {
                        items.Add(attribute);
                    }
                }
            }

            selected_channels_listbox.Items.Clear();
            foreach (ListBoxItem item in items)
            {
                selected_channels_listbox.Items.Add(item);
            }
        }

        private void strokeColorColorpicker_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GroupManager.GetSelectedChannelsData(group, active_selected_channel.Split(' ')[0]).Option.stroke_color = new SolidColorBrush(stroke_color_colorpicker.Color);
        }

        private void strokeThicknessTxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GroupManager.GetSelectedChannelsData(group, active_selected_channel.Split(' ')[0]).Option.stroke_thickness = float.Parse(stroke_thickness_txtbox.Text);
            }
        }

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
