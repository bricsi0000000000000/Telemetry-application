using ART_TELEMETRY_APP.InputFiles;
using System.Collections.Generic;
using System.Windows.Media;

namespace ART_TELEMETRY_APP.Datas.Classes
{
    /// <summary>
    /// This class represents one single column, from an <seealso cref="InputFile"/>.
    /// </summary>
    public class Channel
    {
        public Channel(string channelName)
        {
            Name = channelName;
            Data = new List<double>();
            Color = ColorManager.GetChartColor;
        }
        public string Name { get; private set; }
        public List<double> Data { get; private set; }
        public bool IsActive { get; set; } = false;
        public Color Color { get; set; }
        public void AddChannelData(float data) => Data.Add(data);


       // public LineSerieOptions Option { get; set; }
       // public string InputFileName { get; set; }
      //  public string DriverName { get; set; }

        //private float filter_percent = .6f;

        /*public void MakeDatasInLaps(string attribute)
        {
            for (int i = 0; i < laps.Count; i++)
            {
                GetSingleData(attribute).DatasInLaps.Add(GetChartValues(attribute, i));
            }
        }*/

       /* ChartValues<double> filteredData(ChartValues<double> datas)
        {
            ChartValues<double> input_datas = new ChartValues<double>(datas);
            int total = input_datas.Count;
            Random rand = new Random(DateTime.Now.Millisecond);
            while (input_datas.Count / (double)total > filter_percent)
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
        }*/
    }
}
