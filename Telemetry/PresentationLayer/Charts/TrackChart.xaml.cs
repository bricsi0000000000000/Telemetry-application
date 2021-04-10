using ScottPlot;
using ScottPlot.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PresentationLayer.Charts
{
    /// <summary>
    /// Represents a driverless track.
    /// </summary>
    public partial class TrackChart : UserControl
    {
        private PlottableScatterHighlight plottableScatterHighlight;

        /// <summary>
        /// Car image on track.
        /// </summary>
        private readonly Bitmap carImage;

        private static int lastTrackDataID = 0;

        private GridLength expanderWidth = GridLength.Auto;

        private const string CAR_IMAGE_NAME = @"..\..\..\..\..\default_files\images\car_top_game.png";

        public class TrackData
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public double[] XAxisValues { get; set; }
            public double[] YAxisValues { get; set; }
            public double[] IntegratedYawAngle { get; set; }
            public float C0refChannelFirstValue { get; set; }
            public bool IsSelected { get; set; }
        }

        private List<TrackData> trackData = new List<TrackData>();

        private Tuple<double[], double[]> leftSide;
        private Tuple<double[], double[]> rightSide;
        private Tuple<double[], double[]> centerSide;

        /// <summary>
        /// Constructor for TrackChart
        /// </summary>
        public TrackChart()
        {
            InitializeComponent();

            try
            {
                carImage = new Bitmap(CAR_IMAGE_NAME);
            }
            catch (Exception)
            {
                throw new Exception($"{CAR_IMAGE_NAME} found");
            }

            ScottPlotChart.plt.Frame(false);
            ScottPlotChart.plt.Style(ScottPlot.Style.Light1);
            ScottPlotChart.plt.Colorset(Colorset.OneHalfDark);
            /*ScottPlotChart.plt.YLabel(yAxisLabel);
            ScottPlotChart.plt.XLabel(xAxisLabel);
            ScottPlotChart.plt.Legend();*/
            ScottPlotChart.plt.Grid(enable: false);
            ScottPlotChart.plt.Ticks(displayTicksX: false, displayTicksY: false);
            ScottPlotChart.Render();
        }

        public void AddTrackLayout(Tuple<double[], double[]> leftSide,
                             Tuple<double[], double[]> rightSide,
                             Tuple<double[], double[]> centerSide)
        {
            this.leftSide = leftSide;
            this.rightSide = rightSide;
            this.centerSide = centerSide;

            UpdateTrackLayout();
        }

        private void UpdateTrackLayout()
        {
            AddPlot(xAxisValues: leftSide.Item1,
                                 yAxisValues: leftSide.Item2,
                                 color: Color.Black);

            AddPlot(xAxisValues: rightSide.Item1,
                                  yAxisValues: rightSide.Item2,
                                  color: Color.Black);

            AddPlot(xAxisValues: centerSide.Item1,
                                  yAxisValues: centerSide.Item2,
                                  color: Color.Black,
                                  lineStyle: LineStyle.Dash);

            SetAxisLimitsToAuto();
        }

        public void AddTrackData(int id, string name, double[] xAxisValues, double[] yAxisValues, double[] integratedYawAngle, float c0refChannelFirstValue)
        {
            NoInputFilesGrid.Visibility = Visibility.Hidden;

            if (trackData.Find(x => x.Name.Equals(name)) == null)
            {
                trackData.Add(new TrackData()
                {
                    ID = id,
                    Name = name,
                    XAxisValues = xAxisValues,
                    YAxisValues = yAxisValues,
                    IntegratedYawAngle = integratedYawAngle,
                    C0refChannelFirstValue = c0refChannelFirstValue,
                    IsSelected = true
                });

                lastTrackDataID++;
                AddInputFileItem(name);
            }
        }

        public void RemoveTrackData(int id)
        {
            var data = trackData.Find(x => x.ID == id);
            RemoveInputFileItem(data.Name);
            trackData.Remove(data);

            if (trackData.Count == 0)
            {
                NoInputFilesGrid.Visibility = Visibility.Visible;
            }
        }

        private void AddInputFileItem(string name)
        {
            var checkBox = new CheckBox() { Content = name };
            checkBox.Checked += InputFileCheckBox_Click;
            checkBox.Unchecked += InputFileCheckBox_Click;
            checkBox.IsChecked = true;
            InputFilesStackPanel.Children.Add(checkBox);
        }

        private void RemoveInputFileItem(string fileName)
        {
            int index = GetInputFileItemIndex(fileName);
            if (index != -1)
            {
                InputFilesStackPanel.Children.RemoveAt(index);
            }
        }

        private int GetInputFileItemIndex(string fileName)
        {
            int index = 0;

            foreach (CheckBox item in InputFilesStackPanel.Children)
            {
                if (item.Content.Equals(fileName))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        private void InputFileCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string content = checkBox.Content.ToString();

            trackData.Find(x => x.Name.Equals(content)).IsSelected = (bool)checkBox.IsChecked;

            Update(0);
        }

        public void Update(int dataIndex)
        {
            ScottPlotChart.plt.Clear();
            UpdateTrackLayout();

            foreach (var item in trackData)
            {
                if (item.IsSelected)
                {
                    var actTrackData = trackData.Find(x => x.Name.Equals(item.Name));

                    double xValue = 0;
                    double yValue = 0;
                    if (dataIndex < actTrackData.XAxisValues.Length)
                    {
                        xValue = actTrackData.YAxisValues[dataIndex];
                        yValue = actTrackData.XAxisValues[dataIndex];
                    }
                    else
                    {
                        xValue = actTrackData.YAxisValues.Last();
                        yValue = actTrackData.XAxisValues.Last();
                    }

                    AddPlot(CreateOffset(actTrackData.YAxisValues, actTrackData.C0refChannelFirstValue).ToArray(), CreateOffset(actTrackData.XAxisValues, actTrackData.C0refChannelFirstValue).ToArray(), Color.Black);

                    var rotation = CreateOffset(actTrackData.IntegratedYawAngle, actTrackData.C0refChannelFirstValue);
                    PlotImage(xValue - actTrackData.C0refChannelFirstValue, yValue - actTrackData.C0refChannelFirstValue, dataIndex < rotation.Count ? rotation[dataIndex] : rotation.Last());
                }
            }

            SetAxisLimitsToAuto();
        }

        /// <summary>
        /// Pushes the <paramref name="list"/> with <paramref name="offset"/>.
        /// </summary>
        /// <param name="list">List to be shifted.</param>
        /// <param name="offset">Value to be shifted.</param>
        /// <returns>Shifted list based on <paramref name="offset"/>.</returns>
        private List<double> CreateOffset(double[] list, float offset)
        {
            var newList = new List<double>();
            foreach (var number in list)
            {
                newList.Add(number - offset);
            }

            return newList;
        }

        /// <summary>
        /// Creates a <see cref="Tracks.Classes.DriverlessTrack"/>-s plot.
        /// </summary>
        /// <param name="xAxisValues">Values on <b>horizontal</b> axis.</param>
        /// <param name="yAxisValues">Values on <b>vertical</b> axis.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="xValue">Value on the horizontal axis.</param>
        /// <param name="yValue">Value on the vertical axis.</param>
        /// <param name="yAxisLabel">Label on <b>vertical</b> axis. Default is an empty string.</param>
        /// <param name="xAxisLabel">Label on <b>horizontal</b> axis. Default is an empty string.</param>
        /// <param name="lineWidth">Width of the line. Default is <c>3</c>.</param>
        /// <param name="lineStyle">Style of the line. Default is <see cref="LineStyle.Solid"/>.</param>
        /// <param name="enableLabel">If true, the label is enabled on the line.</param>
        public void AddPlot(double[] xAxisValues,
                             double[] yAxisValues,
                             Color color,
                             double xValue = 0,
                             double yValue = 0,
                             string yAxisLabel = "",
                             string xAxisLabel = "x",
                             double lineWidth = 3,
                             LineStyle lineStyle = LineStyle.Solid,
                             bool enableLabel = false)
        {
            // Flips the value, because Gergő said
            // that the horizontal axis must be positive
            // to the left side and negativ to the right side.
            for (int i = 0; i < xAxisValues.Length; i++)
            {
                xAxisValues[i] *= -1;
            }

            if (enableLabel)
            {
                plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues,
                                                                                    yAxisValues,
                                                                                    markerShape: MarkerShape.none,
                                                                                    color: color,
                                                                                    lineWidth: lineWidth,
                                                                                    lineStyle: lineStyle,
                                                                                    label: $"{xAxisLabel}: {xValue:f3}\n{yAxisLabel}: {yValue:f3}");
            }
            else
            {
                plottableScatterHighlight = ScottPlotChart.plt.PlotScatterHighlight(xAxisValues,
                                                                                    yAxisValues,
                                                                                    markerShape: MarkerShape.none,
                                                                                    color: color,
                                                                                    lineWidth: lineWidth,
                                                                                    lineStyle: lineStyle);
            }

            ScottPlotChart.Render();
        }


        /// <summary>
        /// Plot a car image and rotates based on the <paramref name="rotation"/>.
        /// </summary>
        /// <param name="xValue">Image <b>x</b> value.</param>
        /// <param name="yValue">Image <b>y</b> value.</param>
        /// <param name="rotation">Image rotation.</param>
        public void PlotImage(double xValue, double yValue, double rotation)
        {
            ScottPlotChart.plt.PlotBitmap(carImage, xValue * -1 - .28f, yValue + 1.2f, rotation: rotation * -1);
        }

        /// <summary>
        /// Sets the axis limits to automatic.
        /// </summary>
        public void SetAxisLimitsToAuto()
        {
            ScottPlotChart.plt.AxisAuto();
            ScottPlotChart.Render();
        }

        /// <summary>
        /// Sets the frame border visible or not.
        /// </summary>
        /// <param name="left">Left border.</param>
        /// <param name="bottom">Bottom border.</param>
        /// <param name="top">Top border.</param>
        /// <param name="right">Right</param>
        public void SetFrameBorder(bool left = true, bool bottom = true, bool top = true, bool right = true)
        {
            ScottPlotChart.plt.Frame(left: left, bottom: bottom, top: top, right: right);
        }

        private void Grid_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                expanderWidth = grid.ColumnDefinitions[2].Width;
                grid.ColumnDefinitions[2].Width = GridLength.Auto;
            }
        }

        private void Grid_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                grid.ColumnDefinitions[2].Width = expanderWidth;
            }
        }
    }
}
