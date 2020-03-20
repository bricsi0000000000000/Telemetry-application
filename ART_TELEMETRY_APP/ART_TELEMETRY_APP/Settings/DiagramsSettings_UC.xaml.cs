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
using LiveCharts;
using LiveCharts.Charts;
using LiveCharts.Wpf;

namespace ART_TELEMETRY_APP.Settings
{
    /// <summary>
    /// Interaction logic for DiagramsSettings_UC.xaml
    /// </summary>
    public partial class DiagramsSettings_UC : UserControl
    {
        List<CartesianChart> preview_charts = new List<CartesianChart>();

        public DiagramsSettings_UC()
        {
            InitializeComponent();

            InitGroups();

            if (GroupManager.GroupsCount > 0)
            {
                //groups_nothing.Visibility = Visibility.Hidden;
                //groups_settings_nothing.Visibility = Visibility.Hidden;
              //  delete_group_btn.IsEnabled = true;
            }
        }

        public void InitGroups()
        {
           /* groups_tabs.Items.Clear();
            foreach (Group group in GroupManager.Groups)
            {
                TabItem item = new TabItem();
                item.Header = group.Name;
                item.Content = SettingsManager.GetGroupSettingsUC(group.Name);
                if (SettingsManager.GetGroupSettingsUC(group.Name).group.SelectedChannelSettingsUCs.Count > 0)
                {
                    selected_channels_settings_card.Content = SettingsManager.GetGroupSettingsUC(group.Name).group.SelectedChannelSettingsUCs.Last();
                }

                groups_tabs.Items.Add(item);

                if (group.SelectedChannelSettingsUCs.Count > 0)
                {
                    channel_settings_nothing.Visibility = Visibility.Hidden;
                }
            }*/
        }

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

                        GroupSettings_UC group_settings_UC = new GroupSettings_UC(ref group
                                                                                  );
                        SettingsManager.AddGroupSettingsUC(group_settings_UC);
                        item.Content = group_settings_UC;

                        groups_tabs.Items.Add(item);

                        /*if (GroupManager.GroupsCount > 0)
                        {
                            groups_nothing.Visibility = Visibility.Hidden;
                            groups_settings_nothing.Visibility = Visibility.Hidden;
                            delete_group_btn.IsEnabled = true;
                        }*/
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

        private void changeGroupNameTxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.Enter)
            {
                if (!changeGroupNameTxtbox.Text.Equals(string.Empty))
                {
                    SettingsManager.ChangeGroupSettingUCName(GroupManager.ActiveGroup, changeGroupNameTxtbox.Text);
                    GroupManager.ChangeGroupName(GroupManager.ActiveGroup, changeGroupNameTxtbox.Text);
                    GroupManager.ActiveGroup = changeGroupNameTxtbox.Text;
                    InitGroups();
                    foreach (TabItem item in groups_tabs.Items)
                    {
                        if (item.Header.ToString() == changeGroupNameTxtbox.Text)
                        {
                            groups_tabs.SelectedItem = item;
                        }
                    }
                    changeGroupNameTxtbox.Text = string.Empty;
                }
            }*/
        }

        private void deleteGroupClick(object sender, RoutedEventArgs e)
        {
           /* GroupManager.DeleteGroup();
            InitGroups();
            if (groups_tabs.Items.Count > 0)
            {
                groups_tabs.SelectedItem = groups_tabs.Items[0];
                GroupManager.ActiveGroup = ((TabItem)groups_tabs.SelectedItem).Header.ToString();
            }
            else
            {
                groups_nothing.Visibility = Visibility.Visible;
                groups_settings_nothing.Visibility = Visibility.Visible;
                delete_group_btn.IsEnabled = false;
                channel_settings_nothing.Visibility = Visibility.Visible;
            }*/
        }

        private void groupsTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*try
            {
                GroupManager.ActiveGroup = ((TabItem)groups_tabs.SelectedItem).Header.ToString();
                HintAssist.SetHint(changeGroupNameTxtbox, string.Format("Change {0}'s name", GroupManager.ActiveGroup));

                switch (GroupManager.GetGroup().Zooming)
                {
                    case ZoomingOptions.None:
                        break;
                    case ZoomingOptions.X:
                        x_zoom_radiobtn.IsChecked = true;
                        break;
                    case ZoomingOptions.Y:
                        y_zoom_radiobtn.IsChecked = true;
                        break;
                    case ZoomingOptions.Xy:
                        xy_zoom_radiobtn.IsChecked = true;
                        break;
                }

                try
                {
                    object o = (GroupSettings_UC)e.Source;
                }
                catch (Exception)
                {
                    Console.WriteLine("--------------");
                    SettingsManager.GetGroupSettingsUC(((TabItem)groups_tabs.SelectedItem).Header.ToString()).UpdateSelectedChannel();
                }
            }
            catch (Exception) { }*/
        }

        private void zoomRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            if (getNameFromSender(sender).Equals("X"))
            {
                GroupManager.GetGroup().Zooming = ZoomingOptions.X;
            }
            else if (getNameFromSender(sender).Equals("Y"))
            {
                GroupManager.GetGroup().Zooming = ZoomingOptions.Y;
            }
            else if (getNameFromSender(sender).Equals("XY"))
            {
                GroupManager.GetGroup().Zooming = ZoomingOptions.Xy;
            }
        }

        private string getNameFromSender(object sender)
        {
            string[] s = sender.ToString().Split(':');
            return s[1].Split(' ')[0].Trim();
        }

        private void filterSelectedChannelsTxtbox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void strokeColorColorpicker_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void strokeThicknessTxtbox_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
