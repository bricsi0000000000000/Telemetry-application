using ART_TELEMETRY_APP.Charts.Usercontrols;
using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using Newtonsoft.Json;
using ScottPlot.Drawing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ART_TELEMETRY_APP.Live.UserControls
{
    public partial class LiveMenu : UserControl
    {
        private readonly string url = "https://localhost:44332/";

        double[] data = new double[1000]; // start with all zeros
        int length = 1000;
        //double[] xValues;

        int step = 5;
        int maxRenderIndex = 5;
        int index = 0;

        double[] getValues;

        private readonly object lockObject = new object();
        private readonly Queue<double[]> queue = new Queue<double[]>();
        private readonly AutoResetEvent signal = new AutoResetEvent(false);

        public LiveMenu()
        {
            InitializeComponent();

           /* var thread = new Thread(GetValues);
            thread.Start();*/
        }

        private void GetValues()
        {
            do
            {
                getValues = GetData("proba", index).Result;

                Console.WriteLine(getValues.Length);

                if (getValues.Length > 0)
                {
                    if (maxRenderIndex <= data.Length)
                    {
                        int getValuesIndex = 0;
                        for (int i = index; i < index + step; i++)
                        {
                            if (index < length)
                            {
                                data[i] = getValues[getValuesIndex++];
                            }
                        }

                        lock (lockObject)
                        {
                            queue.Enqueue(data);
                        }
                        signal.Set();

                        Dispatcher.Invoke(() =>
                        {
                            InitPlot(maxRenderIndex);
                        });

                        index += step;
                        maxRenderIndex += step;
                    }
                }

                Console.WriteLine("waiting..");
                Thread.Sleep(100);
                Console.WriteLine("done");
            }
            while (true);
        }

        private async Task<double[]> GetData(string name, int lastIndex)
        {
            var result = new double[step];

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1),
                BaseAddress = new Uri(url)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync($"/api/Channels/?name={name}&lastIndex={lastIndex}&step={step}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                result = JsonConvert.DeserializeObject<double[]>(resultString);
            }
            client.Dispose();
            return result;
        }

        private void SendChannelsBtn_Click(object sender, RoutedEventArgs e)
        {
            do
            {
                getValues = GetData("proba", index).Result;
                if (getValues.Length > 0)
                {
                    if (maxRenderIndex <= data.Length)
                    {
                        int getValuesIndex = 0;
                        for (int i = index; i < index + step; i++)
                        {
                            if (index < length)
                            {
                                data[i] = getValues[getValuesIndex++];
                            }
                        }
                        index += step;

                        InitPlot(maxRenderIndex);

                        maxRenderIndex += step;
                    }
                }
            }
            while (getValues.Length > 0);
        }

        private void InitPlot(int maxRenderIndex)
        {
            double[] values = new double[0];

            lock (lockObject)
            {
                if (queue.Count > 0)
                {
                    values = queue.Dequeue();
                }
            }

            ScottPlotChart.plt.Clear();
            var s = ScottPlotChart.plt.PlotSignal(values);
            s.minRenderIndex = maxRenderIndex - step;
            s.maxRenderIndex = maxRenderIndex < values.Length ? maxRenderIndex : maxRenderIndex - 1;

            ScottPlotChart.plt.AxisAuto();

            ScottPlotChart.Render();
        }

        /* public void InitChannels()
         {
             ChannelsStackPanel.Children.Clear();

             foreach (var item in InputFileManager.InputFiles.First().Channels)
             {
                 ChannelsStackPanel.Children.Add(new CheckBox()
                 {
                     Content = item.Name
                 });
             }
         }

         private void SendChannelsBtn_Click(object sender, RoutedEventArgs e)
         {
             var inputFile = InputFileManager.InputFiles.First();
             var selectedChannels = new List<Channel>();

             foreach (CheckBox item in ChannelsStackPanel.Children)
             {
                 if (item.IsChecked == true)
                 {
                     selectedChannels.Add(inputFile.GetChannel(item.Content.ToString()));
                 }
             }

             foreach (var channel in selectedChannels)
             {
                 try
                 {
                     var uri = PostChannel(channel).Result;
                     return;
                 }
                 catch (Exception)
                 {
                     return;
                     throw;
                 }
             }
         }

         private HttpContent ConvertToHttpContent(Channel channel)
         {
             var myContent = JsonConvert.SerializeObject(channel);
             var buffer = Encoding.UTF8.GetBytes(myContent);
             var byteContent = new ByteArrayContent(buffer);
             byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

             return byteContent;
         }

         private async Task<Uri> PostChannel(Channel channel)
         {
             var client = new HttpClient
             {
                 Timeout = TimeSpan.FromMinutes(1),
                 BaseAddress = new Uri(url)
             };
             client.DefaultRequestHeaders.Accept.Clear();
             client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
             var response = await client.PostAsync("/api/Channels", ConvertToHttpContent(channel)).ConfigureAwait(false);
             response.EnsureSuccessStatusCode();
             client.Dispose();
             return response.Headers.Location;
         }

         /*    private async Task<Channel> GetChannel(string name)
             {
                 var result = new Channel(name);

                 var client = new HttpClient
                 {
                     Timeout = TimeSpan.FromMinutes(1),
                     BaseAddress = new Uri("https://localhost:44332/")
                 };
                 client.DefaultRequestHeaders.Accept.Clear();
                 client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                 var response = await client.GetAsync("/api/Channels/?name=" + name).ConfigureAwait(false);
                 if (response.IsSuccessStatusCode)
                 {
                     var resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                     result = JsonConvert.DeserializeObject<Channel>(resultString);
                 }
                 client.Dispose();
                 return result;
             }*/


    }
}
