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
using System.IO;
using MaterialDesignThemes.Wpf;

namespace ART_TELEMETRY_APP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            groupsColorZone.Visibility = Visibility.Hidden;
            chartsColorZone.Visibility = Visibility.Visible;

            BrushConverter brush_converter = new BrushConverter();
            // swicthFormToChartsBtn.Background = (Brush)brush_converter.ConvertFrom("#FFFAFAFA");
            swicthFormToChartsBtn.Foreground = (Brush)brush_converter.ConvertFrom("#e53935");

            importFileDarkening.Visibility = Visibility.Hidden;

            group_options_nothing.Visibility = Visibility.Visible;
            channel_options_nothing.Visibility = Visibility.Visible;
            input_file_nothing.Visibility = Visibility.Visible;
            diagram_nothing.Visibility = Visibility.Visible;
            map_nothing.Visibility = Visibility.Visible;

            map_progressbar_colorzone.Visibility = Visibility.Hidden;
            diagram_calculate_laps.Visibility = Visibility.Hidden;
        }

        private void channelCmbBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            updateChannelCmbboxItems(sender.ToString().Split(':').Last().Trim());
        }

        private void activeChannelItemClick(object sender, MouseButtonEventArgs e)
        {
            string name = getNameFromSender(e.Source);
            var item = ((TreeViewItem)sender).Parent;
            Groups.Instance.ActiveGroup = ((TreeViewItem)item).Header.ToString();
            Groups.Instance.ActiveGroupTreeViewItem = (TreeViewItem)item;

            activeGroupNameLbl.Content = string.Format("{0}", Groups.Instance.ActiveGroup);
            updateGroupOptionsZoomingOption();

            channel_options_nothing.Visibility = Visibility.Hidden;

            selected_channel_lbl.Content = string.Format("{0}", name);
            line_smoothness_toogle_button.IsChecked = Datas.Instance.GetData().GetSingleData(name).Option.line_smoothness;
            stroke_thickness_txtbox.Text = Datas.Instance.GetData().GetSingleData(name).Option.stroke_thickness.ToString();


            Trace.WriteLine(Datas.Instance.GetData().GetSingleData(name).Option.stroke_color);

            stroke_color_colorpicker.Color = ((SolidColorBrush)Datas.Instance.GetData().GetSingleData(name).Option.stroke_color).Color;
            Trace.WriteLine(stroke_color_colorpicker.Color);
        }

        private void updateChannelCmbboxItems(string name)
        {
            active_channel_items_listbox.Items.Clear();
            foreach (var attribute in Datas.Instance.GetData(name).Datas)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = attribute.Name;
                item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(channelListBoxItemClick);
                active_channel_items_listbox.Items.Add(item);
            }
            Datas.Instance.ActiveFileName = name;
        }
        private void updateFilesCmbBox(string file_name)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.Content = file_name.Split('\\').Last();
            item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(channelCmbBoxItemClick);
            channels_cmbbox.Items.Add(item);
            //channels_cmbbox.SelectedItem = item;
        }

        private void channelListBoxItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Groups.Instance.AddAttributeToGroup(sender.ToString().Split(':').Last().Trim());
                Groups.Instance.ActiveGroupTreeViewItem.Items.Clear();

                updateGroupItems();
            }
        }

        private void updateGroupItems(string name = "")
        {
            foreach (string attribute in Groups.Instance.GetGroupAttributes(name))
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = attribute;
                Groups.Instance.ActiveGroupTreeViewItem.Items.Add(item);
                item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(activeChannelItemClick);
            }
        }

        private void groupsTreeViewItemClick(object sender, MouseButtonEventArgs e)
        {
            if (Groups.Instance.GetGroups.Find(name => name.Name == getNameFromSender(e.Source)) != null)
            {
                updateGroupsTreeViewItem((TreeViewItem)sender, getNameFromSender(e.Source));
                channel_options_nothing.Visibility = Visibility.Visible;
            }
        }

        private void updateGroupsTreeViewItem(TreeViewItem item, string name)
        {
            Groups.Instance.ActiveGroupTreeViewItem = item;
            Groups.Instance.ActiveGroup = name;
            activeGroupNameLbl.Content = string.Format("{0}", Groups.Instance.ActiveGroup);
            updateGroupOptionsZoomingOption();

            group_options_nothing.Visibility = Visibility.Hidden;
        }

        private void updateGroupOptionsZoomingOption()
        {
            zoomingOptionsX.IsChecked = Groups.Instance.GetGroup().Zooming == ZoomingOptions.X;
            zoomingOptionsY.IsChecked = Groups.Instance.GetGroup().Zooming == ZoomingOptions.Y;
            zoomingOptionsXY.IsChecked = Groups.Instance.GetGroup().Zooming == ZoomingOptions.Xy;
        }

        private string getNameFromSender(object sender)
        {
            string[] s = sender.ToString().Split(':');
            return s[1].Split(' ')[0].Trim();
        }

        private void addNewGroupDialog(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(add_group_txtbox.Text))
            {
                channel_options_nothing.Visibility = Visibility.Visible;

                groups_tree_view.Items.Clear();
                GroupBuilder group_builder = new GroupBuilder(add_group_txtbox.Text);

                foreach (string name in Groups.Instance.GetGroupsNames)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = name;
                    item.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(groupsTreeViewItemClick);
                    Groups.Instance.ActiveGroupTreeViewItem = item;

                    try
                    {
                        updateGroupItems(name);
                    }
                    catch (Exception)
                    {
                    }

                    item.IsExpanded = true;

                    groups_tree_view.Items.Add(item);

                    updateGroupsTreeViewItem(item, name);
                }
            }

            add_group_txtbox.Text = string.Empty;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void switchFormToGroupsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            groupsColorZone.Visibility = Visibility.Visible;
            chartsColorZone.Visibility = Visibility.Hidden;

            BrushConverter brush_converter = new BrushConverter();
            switchFormToGroupsBtn.Foreground = (Brush)brush_converter.ConvertFrom("#e53935");
            swicthFormToChartsBtn.Foreground = (Brush)brush_converter.ConvertFrom("#DD000000");
        }

        private void switchFormToChartsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            groupsColorZone.Visibility = Visibility.Hidden;
            chartsColorZone.Visibility = Visibility.Visible;

            BrushConverter brush_converter = new BrushConverter();
            switchFormToGroupsBtn.Foreground = (Brush)brush_converter.ConvertFrom("#DD000000");
            swicthFormToChartsBtn.Foreground = (Brush)brush_converter.ConvertFrom("#e53935");
        }

        private void groupOptionsZoomingOptionBtn(object sender, RoutedEventArgs e)
        {
            if (getNameFromSender(sender).Equals("X"))
            {
                Groups.Instance.GetGroup().Zooming = ZoomingOptions.X;
            }
            else if (getNameFromSender(sender).Equals("Y"))
            {
                Groups.Instance.GetGroup().Zooming = ZoomingOptions.Y;
            }
            else if (getNameFromSender(sender).Equals("XY"))
            {
                Groups.Instance.GetGroup().Zooming = ZoomingOptions.Xy;
            }
        }

        private void importFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            importFileProgressBar.Value = 0;
            loading_file_lbl.Content = string.Empty;

            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";

            if (open_file_dialog.ShowDialog() == true)
            {
                importFileDarkening.Visibility = Visibility.Visible;
                loading_file_lbl.Content = string.Format("Loading: {0}", open_file_dialog.FileName.Split('\\').Last());
                DataReader.Instance.ReadData(open_file_dialog.FileName,
                                             importFileProgressBar,
                                             importFileDarkening,
                                             input_file_nothing,
                                             map_svg,
                                             map_nothing,
                                             map_progressbar,
                                             map_progressbar_colorzone
                                             );
            }
            else
            {
                importFileDarkening.Visibility = Visibility.Hidden;
            }

            if (!open_file_dialog.FileName.Equals(string.Empty))
            {
                updateFilesCmbBox(open_file_dialog.FileName);
            }
        }

        private void lineSmoothnessToggleButtonClick(object sender, RoutedEventArgs e)
        {
            Datas.Instance.GetData().GetSingleData(selected_channel_lbl.Content.ToString()).Option.line_smoothness = (bool)line_smoothness_toogle_button.IsChecked;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Groups.Instance.GetGroup().CalculateMultiplier();
            LapBuilder.Instance.Build(diagram_nothing,
                                      diagram_calculate_laps,
                                      charts_grid,
                                      act_lap_lbl
                                      );
        }

        private void stroke_thickness_txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Datas.Instance.GetData().GetSingleData(selected_channel_lbl.Content.ToString()).Option.stroke_thickness = float.Parse(stroke_thickness_txtbox.Text);
        }

        private void stroke_color_colorpicker_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Datas.Instance.GetData().GetSingleData(selected_channel_lbl.Content.ToString()).Option.stroke_color = new SolidColorBrush(stroke_color_colorpicker.Color);
        }

        private void prev_lab_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Datas.Instance.GetData().ActLap - 1 >= 0)
            {
                Datas.Instance.GetData().ActLap--;
                ChartBuilder.Instance.Build(charts_grid, diagram_nothing);
                act_lap_lbl.Content = string.Format("{0}/{1}", Datas.Instance.GetData().ActLap, Datas.Instance.GetData().Laps.Count);
                map_svg.Data = Geometry.Parse(MapBuilder.Instance.GetMap().SvgPathes[Datas.Instance.GetData().ActLap - 1].Item1);
            }
        }

        private void next_lap_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Datas.Instance.GetData().ActLap + 1 <= Datas.Instance.GetData().Laps.Count)
            {
                Datas.Instance.GetData().ActLap++;
                ChartBuilder.Instance.Build(charts_grid, diagram_nothing);
                act_lap_lbl.Content = string.Format("{0}/{1}", Datas.Instance.GetData().ActLap, Datas.Instance.GetData().Laps.Count);
                map_svg.Data = Geometry.Parse(MapBuilder.Instance.GetMap().SvgPathes[Datas.Instance.GetData().ActLap - 1].Item1);
            }
        }

        private void chartsColorZone_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Point position = Mouse.GetPosition(chartsColorZone);
                try
                {
                    Canvas.SetLeft(act_position_circle, MapBuilder.Instance.GetMap().SvgPathes[Datas.Instance.GetData().ActLap - 1].Item2[Convert.ToInt32((position.X * (MapBuilder.Instance.GetMap().SvgPathes[Datas.Instance.GetData().ActLap - 1].Item2.Count)) / chartsColorZone.ActualWidth)].Item1);
                    Canvas.SetTop(act_position_circle, MapBuilder.Instance.GetMap().SvgPathes[Datas.Instance.GetData().ActLap - 1].Item2[Convert.ToInt32((position.X * (MapBuilder.Instance.GetMap().SvgPathes[Datas.Instance.GetData().ActLap - 1].Item2.Count)) / chartsColorZone.ActualWidth)].Item2);
                    Console.WriteLine(MapBuilder.Instance.GetMap().SvgPathes[Datas.Instance.GetData().ActLap - 1].Item2[Convert.ToInt32((position.X * (MapBuilder.Instance.GetMap().SvgPathes[Datas.Instance.GetData().ActLap - 1].Item2.Count)) / chartsColorZone.ActualWidth)].ToString());
                }
                catch (Exception)
                {
                }
                separator.Margin = new Thickness(position.X, 0, 0, 0);
            }
        }
    }
}
