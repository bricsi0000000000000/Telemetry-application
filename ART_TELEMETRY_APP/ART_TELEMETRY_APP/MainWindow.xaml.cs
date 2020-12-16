using ART_TELEMETRY_APP.Groups.Classes;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

             TrackManager.LoadTracks(ref ErrorSnackbar);
             GroupManager.InitGroups(ref ErrorSnackbar);
             MenuManager.InitMainMenuTabs(MainMenuTabControl);

            /*  PathFigure myPathFigure = new PathFigure
              {
                  StartPoint = new Point(10, 50)
              };

              LineSegment myLineSegment = new LineSegment();
              myLineSegment.Point = new Point(200, 70);
              LineSegment myLineSegment2 = new LineSegment();
              myLineSegment2.Point = new Point(50, 100);

              PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
              myPathSegmentCollection.Add(myLineSegment);
              myPathSegmentCollection.Add(myLineSegment2);

              myPathFigure.Segments = myPathSegmentCollection;

              PathFigureCollection myPathFigureCollection = new PathFigureCollection
              {
                  myPathFigure
              };

              PathGeometry myPathGeometry = new PathGeometry
              {
                  Figures = myPathFigureCollection
              };

              Path myPath = new Path
              {
                  Stroke = Brushes.Black,
                  StrokeThickness = 1,
                  Data = myPathGeometry
              };

              canvas.Children.Add(myPath);*/











            /*   using var reader = new StreamReader("alldistances.txt");
               var allData = new ChartValues<float>();
               while (!reader.EndOfStream)
               {
                   allData.Add(float.Parse(reader.ReadLine()));
               }

               var allDataFiltered = filteredData(allData, .05f);


               chart.DisableAnimations = true;
               chart.Series.Add(new LineSeries
               {
                   Values = allDataFiltered,
                   LineSmoothness = 0,
                   PointGeometrySize = 1
               });*/

            /* for (int i = 1; i < 11; i++)
             {
                 using var actReader = new StreamReader(string.Format("distance{0}.txt",i));

                 var actData = new ChartValues<float>();
                 while (!actReader.EndOfStream)
                 {
                     actData.Add(float.Parse(actReader.ReadLine()));
                 }

                 var actDataFiltered = filteredData(actData, .05f);

                 chart.Series.Add(new LineSeries
                 {
                     Values = actDataFiltered,
                     LineSmoothness = 0,
                     PointGeometrySize = 1,
                     Fill = Brushes.Transparent
                 });
             }*/
        }

        ChartValues<float> filteredData(ChartValues<float> datas, float filter)
        {
            var input_datas = new ChartValues<float>(datas);
            int total = input_datas.Count;
            Random rand = new Random(DateTime.Now.Millisecond);
            while (input_datas.Count / (float)total > filter)
            {
                try
                {
                    input_datas.RemoveAt(rand.Next(1, input_datas.Count - 1));
                }
                catch (Exception)
                {
                }
            }

            return input_datas;
        }
    }
}


