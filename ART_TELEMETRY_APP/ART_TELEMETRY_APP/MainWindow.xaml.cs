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

namespace ART_TELEMETRY_APP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void importFileBtnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            DataReader data_reader = new DataReader();
            if (open_file_dialog.ShowDialog() == true)
            {
                data_reader = new DataReader(open_file_dialog.FileName);
            }

            ChartBuilder chart_builder = new ChartBuilder();
            chart_builder.AddGroup("group_a");
            chart_builder.AddChartToGroup("group_a", "chart_a", data_reader.DataTables[0].getFiltered);
            chart_builder.BuildGroup("group_a", grid);
            chart_builder.BuildChart("group_a", "chart_a");
          
        }
    }
}
