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
using LiveCharts;
using LiveCharts.Charts;
using LiveCharts.Wpf;
using System.Diagnostics;
using System.Collections;

namespace ART_TELEMETRY_APP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            groups_grid.Visibility = Visibility.Hidden;
            charts_grid.Visibility = Visibility.Visible;
        }

        private void importFileBtnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            if (open_file_dialog.ShowDialog() == true)
            {
                DataReader data_reader = new DataReader(open_file_dialog.FileName, inputdatas_cmb_box);
                updateInputDatasCmbBox(open_file_dialog.FileName);
            }
        }

        private void addGroupBtnClick(object sender, RoutedEventArgs e)
        {
            if (new_group_txt_box.Text == string.Empty)
            {
                MessageBox.Show("Group name is empty!", "wrong input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                GroupBuilder group_builder = new GroupBuilder(new_group_txt_box.Text);
                groups_cmb_box.Items.Clear();
                foreach (string name in Groups.Instance.GetGroupsNames)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = name;
                    item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(groupsCmbBoxItemClick);
                    groups_cmb_box.Items.Add(item);
                }
                new_group_txt_box.Text = "";
            }
        }

        private void switchFormToCharts(object sender, RoutedEventArgs e)
        {
            groups_grid.Visibility = Visibility.Hidden;
            charts_grid.Visibility = Visibility.Visible;
        }

        private void switchFormToGroups(object sender, RoutedEventArgs e)
        {
            groups_grid.Visibility = Visibility.Visible;
            charts_grid.Visibility = Visibility.Hidden;
        }

        private void groupsCmbBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            Groups.Instance.ActiveGroup = sender.ToString().Split(':').Last().Trim();
            List<string> attributes = Groups.Instance.GetGroupAttributes(sender.ToString());
            if (attributes != null)
            {
                groups_listbox.Items.Clear();
                foreach (string attribute in Groups.Instance.GetGroupAttributes(sender.ToString()))
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = attribute;
                    item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(groupsListBoxItemClick);
                    groups_listbox.Items.Add(item);
                }
            } 
        }
        private void inputDatasCmbBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine(e.ToString());
            inputdatas_listbox.Items.Clear();
            foreach (var attribute in Datas.Instance.GetInputData(sender.ToString().Split(':').Last().Trim()).Datas)
            {
                /* ListBoxItem item = new ListBoxItem();
                 item.Content = attribute.Item1;
                 //item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(inputDatasListBoxItemClick);
                 inputdatas_listbox.Items.Add(item);*/
                inputdatas_listbox.Items.Add(attribute.Item1);
            }
        }
        private void updateInputDatasCmbBox(string file_name)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.Content = file_name.Split('\\').Last();
            item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(inputDatasCmbBoxItemClick);
            inputdatas_cmb_box.Items.Add(item);
        }

        private void inputDatasListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            Groups.Instance.AddAttributeToGroup(sender.ToString().Split(':').Last().Trim());
            groups_listbox.Items.Clear();
            foreach (string attribute in Groups.Instance.GetGroupAttributes(Groups.Instance.ActiveGroup))
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute;
                groups_listbox.Items.Add(item);
            }
        }

        private void groupsListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("groupsListBoxItemClick: " + sender.ToString().Split(':').Last().Trim());
        }

        private void inputdatas_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Trace.WriteLine("|" + inputdatas_listbox.SelectedItem + "|" );
        }
    }
}
