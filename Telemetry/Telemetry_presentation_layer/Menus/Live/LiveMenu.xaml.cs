﻿using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telemetry_data_and_logic_layer.Colors;

namespace Telemetry_presentation_layer.Menus.Live
{
    public class Section
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsLive { get; set; }
    }

    public partial class LiveMenu : UserControl
    {
        private static HttpClient client = new HttpClient();
        private readonly BrushConverter brushConverter = new BrushConverter();

        public LiveMenu()
        {
            InitializeComponent();

            InitilaizeHttpClient();
        }

        private void InitilaizeHttpClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

            client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1),
                BaseAddress = new Uri("http://192.168.1.33:5000")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void RefreshSectionsButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800));
        }

        private void RefreshSectionsButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary900));
        }

        private void RefreshSectionsButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary700));
        }

        private void RefreshSectionsButton_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RefreshSectionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorManager.Secondary800));

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            GetAllSectionsAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task<string> GetAllSectionsAsync()
        {

            var response = await client.GetAsync("/api/Section").ConfigureAwait(false);
            var result = response.Content.ReadAsStringAsync();
            string resultString = result.GetAwaiter().GetResult();
            var a = JsonConvert.DeserializeObject<Section>(resultString);

            return null;
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










        /*   private void NewLiveSectionButton_Click(object sender, RoutedEventArgs e)
      {
          PostNewSection();
      }

      private async void PostNewSection()
      {
          var response = await client.PostAsJsonAsync("/api/Section", NewLiveSectionNameTextBox.Text).ConfigureAwait(false);
          var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
          string resultString = result.GetAwaiter().GetResult();
          if (resultString.Equals("200"))
          {
              Application.Current.Dispatcher.Invoke(() =>
              {
                  ResultIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Approve;
                  ResultIcon.Foreground = (Brush)brushConverter.ConvertFromString(ColorManager.ApprovedColor);
              });
          }
          else
          {
              Application.Current.Dispatcher.Invoke(() =>
              {
                  ResultIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Denied;
                  ResultIcon.Foreground = (Brush)brushConverter.ConvertFromString(ColorManager.DeniedColor);
                  throw new ErrorException("Couldn't create new live section");
              });
          }
      }*/

        /*private async Task<Uri> PostNewSectionAsync(string sectionName)
        {
            var response = await client.PostAsJsonAsync("/api/Section", sectionName).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string resultString = result.GetAwaiter().GetResult();
                if (resultString.Equals("200"))
                {
                    ResultIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Approve;
                }
                else if (resultString.Equals("500"))
                {
                    ResultIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Denied;
                }
            }

            return response.Headers.Location;
        }*/
    }

}