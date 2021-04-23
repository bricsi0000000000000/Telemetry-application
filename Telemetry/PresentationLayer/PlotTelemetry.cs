using DataLayer.Groups;
using LocigLayer.Colors;
using LocigLayer.Groups;
using PresentationLayer.Charts;
using PresentationLayer.Extensions;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PresentationLayer
{
    public abstract class PlotTelemetry : UserControl
    {
        protected const float deltaTime = .05f;

        protected List<string> selectedGroups = new List<string>();
        protected List<string> selectedChannels = new List<string>();
        protected List<double> horizontalAxisData = new List<double>();

        protected abstract Channel HorizontalAxis(int inputFileID);
        protected abstract Chart BuildGroupChart(Group group);
        protected abstract void UpdateCharts();
        protected abstract void UpdateChannelsList();
        public abstract void BuildCharts();
        public abstract Channel GetChannel(int inputFileID, string channelName);

        public virtual void SetChannelActivity(string channelName, int inputFileID, bool isActive)
        {
            BuildCharts();
        }

        protected virtual void Init(StackPanel groupsStackPanel = null)
        {
            InitializeGroupItems(groupsStackPanel);
        }

        public virtual void InitializeGroupItems(StackPanel groupsStackPanel = null)
        {
            groupsStackPanel.Children.Clear();

            foreach (var group in GroupManager.Groups)
            {
                var checkBox = new CheckBox()
                {
                    Content = group.Name
                };

                checkBox.IsChecked = selectedGroups.Contains(group.Name);
                checkBox.Checked += GroupCheckBox_CheckedClick;
                checkBox.Unchecked += GroupCheckBox_CheckedClick;

                groupsStackPanel.Children.Add(checkBox);
            }
        }


        private void GroupCheckBox_CheckedClick(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string content = checkBox.Content.ToString();

            if ((bool)checkBox.IsChecked)
            {
                selectedGroups.Add(content);
            }
            else
            {
                selectedGroups.Remove(content);
            }

            BuildCharts();
        }

        protected void BuildChartGrid(Group group, ref int rowIndex, Grid chartsGrid)
        {
            RowDefinition chartRow = new RowDefinition()
            {
                Height = new GridLength(200)
            };
            RowDefinition gridSplitterRow = new RowDefinition
            {
                Height = new GridLength(5)
            };

            chartsGrid.RowDefinitions.Add(chartRow);
            chartsGrid.RowDefinitions.Add(gridSplitterRow);

            GridSplitter splitter = new GridSplitter
            {
                ResizeDirection = GridResizeDirection.Rows,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = ColorManager.Secondary100.ConvertBrush()
            };
            chartsGrid.Children.Add(splitter);

            var chart = BuildGroupChart(group);
            chartsGrid.Children.Add(chart);

            Grid.SetRow(chart, rowIndex++);
            Grid.SetRow(splitter, rowIndex++);
        }

    }
}
