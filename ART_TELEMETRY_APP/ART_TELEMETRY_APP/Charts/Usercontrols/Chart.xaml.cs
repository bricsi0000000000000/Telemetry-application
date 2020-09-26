using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP.Charts.Usercontrols
{
    /// <summary>
    /// This represents a charts values, that can later used on <seealso cref="CartesianChart"/>
    /// </summary>
    public partial class Chart : UserControl
    {
        public string Name { get; set; }
        private const ushort chart_minimum_height = 100;
        private const ushort chart_height = 350;
        private int max_step_x
        {
            get
            {
                return MaxValueX - MinValueX;
            }
        }
        private const int step_size = 1;
        private int chart_step_size = 0;

        public Chart(string name)
        {
            InitializeComponent();
            Name = name;
            chart.DataTooltip = null;
            chart.DisableAnimations = true;
            chart.Hoverable = false;
           // chart.MinHeight = chart_minimum_height;
            chart.Height = chart_height;
            chart.Zoom = ZoomingOptions.Xy;

            UpdateAxisValues();
        }

        public void UpdateAxisValues()
        {
            try
            {
                chart.AxisX[0].MinValue = chart_step_size;
                chart.AxisX[0].MaxValue = chart_step_size + MaxValueY;
                chart.AxisY[0].MinValue = MinValueX;
                chart.AxisY[0].MaxValue = MaxValueX;
            }
            catch (Exception) { }
        }

        private void pushLeft_Click(object sender, RoutedEventArgs e)
        {
            chart_step_size += step_size;
            UpdateAxisValues();
        }

        private void pushRight_Click(object sender, RoutedEventArgs e)
        {
            chart_step_size -= step_size;
            UpdateAxisValues();
        }

        public void AddAxisX(Axis axis) => chart.AxisX.Add(axis);

        public void AddAxisY(Axis axis) => chart.AxisY.Add(axis);

        public Axis AxisY => chart.AxisY[0];

        public Axis AxisX => chart.AxisX[0];

        public void AddSerie(LineSeries serie) => chart.Series.Add(serie);

        public void ResetZoom()
        {
            chart.AxisX[0].MinValue = double.NaN;
            chart.AxisX[0].MaxValue = double.NaN;
            chart.AxisY[0].MinValue = double.NaN;
            chart.AxisY[0].MaxValue = double.NaN;
        }

        public int MaxValueX { get; set; } = 0;
        public int MinValueX { get; set; } = 0;
        public int MaxValueY { get; set; } = 0;
        public int MinValueY { get; set; } = 0;
    }
}
