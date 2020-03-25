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
    /// Interaction logic for DiagramsSettings_UC.xaml
    /// </summary>
    public partial class DiagramsSettings_UC : UserControl
    {

        public DiagramsSettings_UC()
        {
            InitializeComponent();

            InitGroups();
            initPilotTabs();
        }

        private void initPilotTabs()
        {
            if (PilotManager.Pilots.Count > 0)
            {
                pilots_nothing.Visibility = Visibility.Hidden;
            }
            else
            {
                pilots_nothing.Visibility = Visibility.Visible;
            }

            foreach (Pilot pilot in PilotManager.Pilots)
            {
                TabItem item = new TabItem();
                item.Header = pilot.Name;
                item.IsSelected = true;
                item.Content = new PilotTab_UC(pilot);

                pilots_tabs.Items.Add(item);
            }
        }

        private void addPilotTxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!addPilotTxtbox.Text.Equals(string.Empty))
                {
                    if (PilotManager.GetPilot(addPilotTxtbox.Text) == null)
                    {
                        Pilot pilot = new Pilot(addPilotTxtbox.Text);
                        PilotManager.AddPilot(pilot);

                        TabItem item = new TabItem();
                        item.Header = pilot.Name;
                        item.IsSelected = true;
                        item.Content = new PilotTab_UC(pilot, pilot_error_snack_bar);

                        pilots_tabs.Items.Add(item);
                        SettingsManager.UpdatePilotsInGroups();

                        if (PilotManager.Pilots.Count > 0)
                        {
                            pilots_nothing.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        pilot_error_snack_bar.MessageQueue.Enqueue(string.Format("{0} pilot is already exists!", addPilotTxtbox.Text),
                                                             null, null, null, false, true, TimeSpan.FromSeconds(1));
                    }
                }
                else
                {
                    pilot_error_snack_bar.MessageQueue.Enqueue("Pilot's name is empty!",
                                                         null, null, null, false, true,
                                                         TimeSpan.FromSeconds(1));
                }

                addPilotTxtbox.Text = string.Empty;
            }
        }


        public void InitGroups()
        {
            if (GroupManager.GroupsCount > 0)
            {
                groups_nothing.Visibility = Visibility.Hidden;
                delete_group_btn.IsEnabled = true;
            }
            else
            {
                groups_nothing.Visibility = Visibility.Visible;
                delete_group_btn.IsEnabled = false;
            }

            groups_tabs.Items.Clear();
            foreach (Group group in GroupManager.Groups)
            {
                TabItem item = new TabItem();
                item.Header = group.Name;
                GroupSettings_UC group_settings_UC = new GroupSettings_UC(group,
                                                                          ref item
                                                                         );
                SettingsManager.AddGroupSettingsUC(group_settings_UC);
                item.Content = group_settings_UC;
                /* if (SettingsManager.GetGroupSettingsUC(group.Name).group.SelectedChannelSettingsUCs.Count > 0)
                 {
                     selected_channels_settings_card.Content = SettingsManager.GetGroupSettingsUC(group.Name).group.SelectedChannelSettingsUCs.Last();
                 }*/
                if (GroupManager.ActiveGroup.Equals(group.Name))
                {
                    item.IsSelected = true;
                }
                groups_tabs.Items.Add(item);
                /*if (group.SelectedChannelSettingsUCs.Count > 0)
                {
                    channel_settings_nothing.Visibility = Visibility.Hidden;
                }*/
            }
        }

       /* private void selectedChannelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                selected_channels_listbox.Items.Remove(sender);
                SettingsManager.SelectedChannels.Remove(((ListBoxItem)sender).Content.ToString());
            }
            else
            {
            }
        }*/

        private void addGroupTxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!add_group_txtbox.Text.Equals(string.Empty))
                {
                    if (GroupManager.GetGroup(add_group_txtbox.Text) == null)
                    {
                        Group group = new Group(add_group_txtbox.Text);
                        GroupManager.AddGroup(group);

                        TabItem item = new TabItem();
                        item.Header = add_group_txtbox.Text;
                        item.IsSelected = true;

                        GroupSettings_UC group_settings_UC = new GroupSettings_UC(group,
                                                                                  ref item
                                                                                 );
                        SettingsManager.AddGroupSettingsUC(group_settings_UC);
                        item.Content = group_settings_UC;

                        groups_tabs.Items.Add(item);

                        if (GroupManager.GroupsCount > 0)
                        {
                            groups_nothing.Visibility = Visibility.Hidden;
                            delete_group_btn.IsEnabled = true;
                        }
                    }
                    else
                    {
                        error_snack_bar.MessageQueue.Enqueue(string.Format("{0} group is already exists!", add_group_txtbox.Text),
                                                             null, null, null, false, true,
                                                             TimeSpan.FromSeconds(1));
                    }
                }
                else
                {
                    error_snack_bar.MessageQueue.Enqueue("Group name is empty!",
                                                         null, null, null, false, true,
                                                         TimeSpan.FromSeconds(1));
                }
                add_group_txtbox.Text = string.Empty;
            }
        }

        private void deleteGroupClick(object sender, RoutedEventArgs e)
        {
            GroupManager.DeleteGroup();
            InitGroups();
            if (groups_tabs.Items.Count > 0)
            {
                groups_tabs.SelectedItem = groups_tabs.Items[0];
                GroupManager.ActiveGroup = ((TabItem)groups_tabs.SelectedItem).Header.ToString();
            }
            else
            {
                groups_nothing.Visibility = Visibility.Visible;
                delete_group_btn.IsEnabled = false;
            }
        }

        private void groupsTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((TabItem)groups_tabs.SelectedItem != null)
            {
                GroupManager.ActiveGroup = ((TabItem)groups_tabs.SelectedItem).Header.ToString();
            }
        }
    }
}
