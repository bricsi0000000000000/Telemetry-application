using LiveCharts;
using LiveCharts.Wpf;
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

namespace ART_TELEMETRY_APP.Charts.Usercontrols
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        private short chart_min_height = 100;
        private int max_step_x
        {
            get
            {
                return (int)(max_value_x - min_value_x);
            }
        }
        private int step = 1;
        private int chart_step = 0;

        private double max_value_x = 0;
        private double min_value_x = 0;
        private double max_value_y = 0;
        private double min_value_y = 0;

        public Chart()
        {
            InitializeComponent();

            chart.DataTooltip = null;
            chart.DisableAnimations = true;
            chart.Hoverable = false;
            chart.MinHeight = chart_min_height;
            chart.Zoom = ZoomingOptions.Xy;
        }

        public void UpdateAxisValues()
        {
            try
            {
                chart.AxisX[0].MinValue = chart_step;
                chart.AxisX[0].MaxValue = chart_step + max_value_y;
                chart.AxisY[0].MinValue = min_value_x;
                chart.AxisY[0].MaxValue = max_value_x;
            }
            catch (Exception e){}
        }

        private void pushLeft_Click(object sender, RoutedEventArgs e)
        {
            chart_step += step;
            UpdateAxisValues();
        }

        private void pushRight_Click(object sender, RoutedEventArgs e)
        {
            chart_step -= step;
            UpdateAxisValues();
        }

        public void AddAxisX(Axis axis)
        {
            chart.AxisX.Add(axis);
        }

        public void AddAxisY(Axis axis)
        {
            chart.AxisY.Add(axis);
        }

        public Axis AxisY
        {
            get
            {
                return chart.AxisY[0];
            }
        }

        public Axis AxisX
        {
            get
            {
                return chart.AxisX[0];
            }
        }

        public void AddSerie(LineSeries serie)
        {
            chart.Series.Add(serie);
        }

        public void ResetZoom()
        {
            chart.AxisX[0].MinValue = double.NaN;
            chart.AxisX[0].MaxValue = double.NaN;
            chart.AxisY[0].MinValue = double.NaN;
            chart.AxisY[0].MaxValue = double.NaN;
        }

        public double MaxValueX { get => max_value_x; set => max_value_x = value; }
        public double MinValueX { get => min_value_x; set => min_value_x = value; }
        public double MaxValueY { get => max_value_y; set => max_value_y = value; }
        public double MinValueY { get => min_value_y; set => min_value_y = value; }
    }
}
